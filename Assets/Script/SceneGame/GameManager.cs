using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Spine;
using Spine.Unity;

public class GameManager : MonoBehaviour
{
    // 次のステップ開始までの遅延時間 ※正否アニメーションの尺の違いにより､2パターン用意している.
    // TODO: アニメーション終了時間をハードコーディングするのではなく､雛形となるSkeletonGraphicから値を動的に取得したい.
    private const float NextStepDelayTimeSuccess = 2.23f;
    private const float NextStepDelayTimeFail = 1.5f;

    // このゲームの最大時間 ※この秒以上経過した場合､ゲーム終了.
    private const float InGameMaxTime = 20.0f;

    // 背景キャラ.
    [SerializeField]
    private SkeletonAnimationController _animCharaController = null;

    // クリア時パネル
    [SerializeField]
    private GameObject _gameResultPanel = null;

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


    // 現在のステップ.
    private int _currentStep = 1;
    // 最大ステップ.
    private int _maxStep = Int32.MaxValue;

    // 現在のステップにおける､正解のAnswerId.
    private int _currentStepAnswerId;

    // 現在のステップにおける､吹き出しオブジェクトのリスト.
    private List<FukidashiController> _fukidashiList = new List<FukidashiController>(){};

    // 各ステップの結果情報のリスト.
    private List<StepResult> _stepResultList = new List<StepResult>();

    public void Awake()
    {
        MasterManager.Instance.LoadTableDatas();
        _currentStep = 1;
        _maxStep = MasterManager.Instance.GetMaxStepFromQuestionData();

        _timerWidget.Initialize((int)InGameMaxTime, () => GameEnd() );
        _timerWidget.Start();
        
        StartStep(_currentStep);

        // 以下のコードで､アニメーションの再生時間を取得できる.
        // var track1 = _fukidashiPrefab.GetComponent<SkeletonGraphic>().AnimationState.SetAnimation(0, "maru", false);
        // var track2 = _fukidashiPrefab.GetComponent<SkeletonGraphic>().AnimationState.SetAnimation(0, "batu", false);
        // Debug.LogFormat("maru animation time: {0}", track1.AnimationEnd);
        // Debug.LogFormat("batu animation time: {0}", track2.AnimationEnd);
    }

    public void Start()
    {
        Debug.Assert(_animCharaController != null);
        Debug.Assert(_gameResultPanel          != null);
        Debug.Assert(_fukidashiPrefab     != null);
        Debug.Assert(_canvas              != null);
        Debug.Assert(_textStepTitle       != null);
        Debug.Assert(_timerWidget         != null);
    }

    // 各ステップの開始時処理を行う.
    private void StartStep(int step)
    {
        if (step > _maxStep) {
            // 最大ステップ数を超えたらゲーム終了処理.
            GameEnd();
            return;
        }
        
        object title = MasterManager.Instance.GetQuestionData((int)step, "title");
        
        _currentStepAnswerId = (int)(MasterManager.Instance.GetQuestionData((int)step, "answer_id"));

        _textStepTitle.text = title.ToString();
        
        CreateFukidashiObjects(step);
    }

    // 各ステップにおける､吹き出しオブジェクトの生成を行う.
    private void CreateFukidashiObjects(int step)
    {
        List<Dictionary<string, object>> list = MasterManager.Instance.GetCulumnListForCulumnKeyFromAnswerData("stage", InfoManager.Instance.GameLevel);

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

    // ゲーム終了時処理を行う.
    private void GameEnd()
    {
        // タイマーを止める(タイムアウト処理が発生しないように).
        _timerWidget.Stop();

        // 残っている吹き出しを削除.
        ClearFukidashiList();

        // キャラのクリア演出.
        _animCharaController.SetAnimation(CharaAnimName.GameClear);

        // クリアパネルの表示.
        int score = SumUpScore();
        _gameResultPanel.SetActive(true);
        _gameResultPanel.GetComponent<SkeletonGraphic>().AnimationState.SetAnimation(0, GameClearPanelAnimName.Start, false);
        _gameResultPanel.GetComponent<GameClearPanel>().SetTexts(_timerWidget.GetRemainTime(), score);
    }

    // 吹き出しオブジェクトタッチ時処理.
    private void OnTouchFukidashiObject(FukidashiController fukidashiObj)
    {
        if (fukidashiObj.HasTouched) {
            return;
        }

        // 正否を判定.
        bool isSuccess = (_currentStepAnswerId == (int)fukidashiObj.AnswerId);

        // 正否に応じた吹き出しアニメーションetcを実行.
        if (isSuccess) {
            fukidashiObj.Success();
        } else {
            fukidashiObj.Fail();
        }

        // 現在のステップの吹き出しは削除.
        ClearFukidashiList(fukidashiObj);
        // foreach (FukidashiController fukidashi in _fukidashiList) {
        //     if (fukidashi.AnswerId == fukidashiObj.AnswerId) {
        //         // ※タッチした吹き出しは削除しない(各種アニメーションなどさせるため).
        //         continue;
        //     }
        //     Destroy(fukidashi.gameObject);
        // }
        // _fukidashiList.Clear();

        // 背景キャラに指定のアニメーションをさせる.
        string animationName = (isSuccess) ? CharaAnimName.Success : CharaAnimName.Fail;
        _animCharaController.SetAnimation(animationName);

        // 現在のステップの結果を保存しておく.
        int stage = (int)InfoManager.Instance.GameLevel;
        int score = (isSuccess) ? MasterManager.Instance.GetScoreFromQuestionData(stage, _currentStep) : 0;
        StepResult packege = new StepResult()
        {
            Step       = _currentStep,
            IsSuccess  = isSuccess,
            RemainTime = _timerWidget.GetRemainTime(),
            AnswerId   = fukidashiObj.AnswerId,
            Score      = score,
        };
        _stepResultList.Add(packege);

        // 次のステップを開始する.
        _currentStep++;
        StartCoroutine(FuncDelayEvent(
            (isSuccess) ? NextStepDelayTimeSuccess : NextStepDelayTimeFail,
            () => StartStep(_currentStep)
        ));
    }

    private void ClearFukidashiList(FukidashiController touchedOne = null)
    {
        // 現在のステップの吹き出しは削除.
        foreach (FukidashiController fukidashi in _fukidashiList) {
            if (touchedOne && touchedOne.AnswerId == fukidashi.AnswerId) {
                // ※タッチした吹き出しは削除しない(各種アニメーションなどさせるため).
                continue;
            }
            Destroy(fukidashi.gameObject);
        }
        _fukidashiList.Clear();
    }

    private int SumUpScore()
    {
        // ※ 失敗の場合はスコア0なので､実はWhereは不要.
        int score = _stepResultList
                        .Where( result => result.IsSuccess )
                        .Select( result => (int)result.Score )
                        .Sum();
        return score;
    }

    private IEnumerator FuncDelayEvent(float delaySec, Action callback)
    {
        yield return new WaitForSeconds(delaySec);
        callback();
    }
}