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

    private enum State {
        NotStart,  // 起動前.
        StartStep, // ステップ開始後.
        Shuffling, // ステップ中(吹き出しシャッフル演出中).
        EndStep,   // ステップ終了.
        End,       // ゲーム終了.
    }
    private State _state;


    // 現在のステップにおける､吹き出しオブジェクトのリスト.
    private List<Fukidashi> _fukidashiList = new List<Fukidashi>(){};

    private void Awake()
    {
        Debug.Assert(_fukidashiPrefab != null);
        Debug.Assert(_canvas          != null);
    }

    private void Start()
    {
        _state = State.NotStart;
    }

    private void Update()
    {
        
    }

    public void InstantiateObjects(int step, Action<bool> onTouch)
    {
        _state = State.StartStep;

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
}
