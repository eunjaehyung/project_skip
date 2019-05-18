using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Spine.Unity;

public class GameManager : MonoBehaviour
{
    // 次のステップ開始までの遅延時間 ※正否アニメーションの尺の違いにより､2パターン用意している.
    private const float NextStepDelayTimeSuccess = 3.5f;
    private const float NextStepDelayTimeFail = 1.5f;

    // このゲームの最大時間 ※この秒以上経過した場合､ゲーム終了.
    private const float InGameMaxTime = 20.0f;

    // 吹き出しプレハブ.
    [SerializeField]
    private GameObject _fukidashiPrefab = null;

    // 各種UIオブジェクト.
    [SerializeField]
    private Canvas _canvas = null;
    [SerializeField]
    private Text _textStepTitle = null;
    [SerializeField]
    private TimerWidget _timerWidget = null;
    [SerializeField]
    private GameResultPanel _gameResultPanel = null;


    // 現在のステップ.
    private uint _currentStep = 1;

    // 現在のステップにおける､正解のAnswerId.
    private uint _currentStepAnswerId;

    // 現在のステップにおける､吹き出しオブジェクトのリスト.
    private List<FukidashiController> _fukidashiList = new List<FukidashiController>(){};

    // 各ステップの結果情報のリスト.
    private List<StepResult> _stepResultList = new List<StepResult>();


    public void Start()
    {
        Debug.Assert(_fukidashiPrefab != null);
        Debug.Assert(_canvas          != null);
        Debug.Assert(_textStepTitle   != null);
        Debug.Assert(_timerWidget     != null);
        Debug.Assert(_gameResultPanel != null);
    }

    public void Awake()
    {
        MasterManager.Instance.LoadTableDatas();
        
        // TODO: タイムアウト時の終了処理を､後ほど実装する.
        _timerWidget.SetTime((int)InGameMaxTime, () => Debug.Log("Time Out"));
        
        _timerWidget.StartCountDown();
        
        StartStep(_currentStep);
    }

    // 各ステップの開始時処理を行う.
    private void StartStep(uint step)
    {
        if (step == 4)
        {
            _timerWidget.StopTime = true;
            _gameResultPanel.SetData(_timerWidget.GetRemainTime().ToString(), "0");
            InfoManager.Instance.SetRecord(_stepResultList);
            return;
        }
        
        object title = MasterManager.Instance.GetQuestionData((int)step, "title");
        
        _currentStepAnswerId = (uint)(int)(MasterManager.Instance.GetQuestionData((int)step, "answer_id"));

        _textStepTitle.text = title.ToString();
        
        CreateFukidashiObjects(step);
    }

    // 各ステップにおける､吹き出しオブジェクトの生成を行う.
    private void CreateFukidashiObjects(uint step)
    {
        List<Dictionary<string, object>> list = MasterManager.Instance.GetCulumnListForCulumnKey("stage", InfoManager.Instance.GameLevel);

        for (int i = 0; i < list.Count; ++i)
        {
            if (list[i]["step"].ToString() != step.ToString()) {
                continue;
            }

            
            GameObject fukidashiGameObj = Instantiate(_fukidashiPrefab, _canvas.transform);
            FukidashiController fukidashiObj = fukidashiGameObj.GetComponent<FukidashiController>();
            Debug.Assert(fukidashiObj != null);

            fukidashiObj.SetMasterData(list[i]);
            fukidashiObj.TouchCallback = OnTouchFukidashiObject;

            _fukidashiList.Add(fukidashiObj);
        }
    }

    // 吹き出しオブジェクトタッチ時処理.
    private void OnTouchFukidashiObject(FukidashiController fukidashiObj)
    {
        if (fukidashiObj.HasTouched) {
            return;
        }

        // 正否を判定.
        bool isSuccess = (_currentStepAnswerId == (int)fukidashiObj.AnswerId);

        // 正否に応じたアニメーションetcを実行.
        if (isSuccess) {
            fukidashiObj.Success();
        } else {
            fukidashiObj.Fail();
        }

        // 現在のステップの吹き出しは削除.
        foreach (FukidashiController fukidashi in _fukidashiList) {
            if (fukidashi.AnswerId == fukidashiObj.AnswerId) {
                // ※タッチした吹き出しは削除しない(各種アニメーションなどさせるため).
                continue;
            }
            Destroy(fukidashi.gameObject);
        }
        _fukidashiList.Clear();
        
        // 現在のステップの結果を保存しておく.
        StepResult packege = new StepResult()
        {
            Step       = _currentStep,
            IsSuccess  = isSuccess,
            RemainTime = _timerWidget.GetRemainTime(),
            AnswerId   = fukidashiObj.AnswerId,
        };
        _stepResultList.Add(packege);

        // 次のステップを開始する.
        _currentStep++;
        StartCoroutine(FuncDelayEvent(
            (isSuccess) ? NextStepDelayTimeSuccess : NextStepDelayTimeFail,
            () => StartStep(_currentStep)
        ));
    }

    private IEnumerator FuncDelayEvent(float delaySec, Action callback)
    {
        yield return new WaitForSeconds(delaySec);
        callback();
    }
}