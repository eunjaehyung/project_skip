using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class FukidashisController : MonoBehaviour
{
    // 吹き出しプレハブ.
    [SerializeField]
    private GameObject _fukidashiPrefab = null;

    // キャンバス.
    [SerializeField]
    private Canvas _canvas = null;

    [SerializeField]
    private Dictionary<int, float> _levelAndShuffleFreqTime = new Dictionary<int, float>(){
        {1, 7.5f},
        {2, 5.0f},
        {3, 3.5f},
    };

    private enum State {
        NotStart,  // 起動前.
        StartStep, // ステップ開始後.
        Shuffling, // ステップ中(吹き出しシャッフル演出中).
        EndStep,   // ステップ終了.
        End,       // ゲーム終了.
    }
    private State _state = State.NotStart;

    private float _shuffleElapsedTime = 0.0f;
    private float _shuffleFreqTime = 4.0f;
    private float _hideElapsedTime = 0.0f;
    private float _hideEndTime = 0.25f;


    // 現在のステップにおける､吹き出しオブジェクトのリスト.
    private List<Fukidashi> _fukidashiList = new List<Fukidashi>(){};

    private void Awake()
    {
        Debug.Assert(_fukidashiPrefab != null);
        Debug.Assert(_canvas          != null);

        _shuffleFreqTime = _levelAndShuffleFreqTime[LevelManager.Instance.Level];
    }

    private void FixedUpdate()
    {
        if (_state == State.StartStep) {
            _shuffleElapsedTime += Time.fixedDeltaTime;
            if (_shuffleElapsedTime >= _shuffleFreqTime) {
                _shuffleElapsedTime = 0.0f;
                _state = State.Shuffling;
                foreach (var fukidashi in _fukidashiList) { fukidashi.gameObject.SetActive(false); }
                Shuffle();
            }
        }

        if (_state == State.Shuffling) {
            _hideElapsedTime += Time.fixedDeltaTime;
            if (_hideElapsedTime >= _hideEndTime) {
                _hideElapsedTime = 0.0f;
                _state = State.StartStep;
                foreach (var fukidashi in _fukidashiList) { fukidashi.gameObject.SetActive(true); }
            }
        }
    }

    public void InstantiateObjects(int step, Action<bool> onTouch)
    {
        _state = State.StartStep;
        _shuffleElapsedTime = 0.0f;
        _hideElapsedTime = 0.0f;

        // 現在のステップの正解id.
        MasterItemStep stepMaster = MasterStepHolder.Instance().GetOneOrFail(LevelManager.Instance.Level, step);
        int answerId = stepMaster.AnswerId;

        // タッチ時コールバック
        Action<Fukidashi> onTouchCallBack = (fukidashi) => {
            if (fukidashi.HasTouched) {
                return;
            }

            // 正否を判定.
            bool isSuccess = (fukidashi.AnswerId == answerId);

            // 正否に応じた吹き出しアニメーションetcを実行.
            if(isSuccess) { fukidashi.Success(); } else { fukidashi.Fail(); }

            // 現在のステップの吹き出しは削除.
            ClearObjects(fukidashi);

            // 呼び出し先のコールバックを実行.
            onTouch(isSuccess);
        };

        // 
        List<MasterItemFukidashi> masterList = MasterFukidashiHolder.Instance().GetList(
            (item) => { return item.Stage == LevelManager.Instance.Level && item.Step == step; }
        );
        foreach (var master in masterList) {
            Fukidashi fukidashi = InstantiateObject(master, onTouchCallBack);
            _fukidashiList.Add(fukidashi);
        }
    }

    private Fukidashi InstantiateObject(MasterItemFukidashi master, Action<Fukidashi> onTouch)
    {
        GameObject fukidashiGameObj = Instantiate(_fukidashiPrefab, _canvas.transform);
        Fukidashi fukidashi = fukidashiGameObj.GetComponent<Fukidashi>();
        Debug.Assert(fukidashi != null);

        fukidashi.SetMasterData(master);
        fukidashi.TouchCallback = onTouch;

        return fukidashi;
    }

    public void ClearObjects(Fukidashi touchedOne = null)
    {
        _state = State.EndStep;

        foreach (Fukidashi fukidashi in _fukidashiList) {
            if (touchedOne && touchedOne.AnswerId == fukidashi.AnswerId) {
                // ※タッチした吹き出しは削除しない(各種アニメーションなどさせるため).
                continue;
            }
            Destroy(fukidashi.gameObject);
        }
        _fukidashiList.Clear();
    }

    private void Shuffle()
    {
        int n = (int) Mathf.Ceil( _fukidashiList.Count / 2 );
        for (int i = 0; i < n; i++) {
            int i1 = UnityEngine.Random.Range(0, _fukidashiList.Count);
            int i2 = UnityEngine.Random.Range(0, _fukidashiList.Count);
            
            var item1 = _fukidashiList[i1];
            var item2 = _fukidashiList[i2];

            Vector2 tmpPos = item1.GetComponent<RectTransform>().anchoredPosition;
            item1.GetComponent<RectTransform>().anchoredPosition = item2.GetComponent<RectTransform>().anchoredPosition;
            item2.GetComponent<RectTransform>().anchoredPosition = tmpPos;
        }
    }
}
