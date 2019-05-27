using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using Spine;
using Spine.Unity;

public class GameSceneController : MonoBehaviour
{
    // 次のステップ開始までの遅延時間 ※正否アニメーションの尺の違いにより､2パターン用意している.
    // TODO: アニメーション終了時間をハードコーディングするのではなく､雛形となるSkeletonGraphicから値を動的に取得したい.
    private const float NextStepDelayTimeSuccess = 2.23f; // 吹き出し正解アニメの終了時間.
    private const float NextStepDelayTimeFail = 2.67f;    // 背景キャラ失敗アニメの終了時間.

    // このゲームの最大時間 ※この秒以上経過した場合､ゲーム終了.
    private const float InGameMaxTime =40.0f;

    [SerializeField]
    private FukidashisController _fukidashisController = null;

    // 背景キャラ.
    [SerializeField]
    private SkeletonAnimationController _animCharaController = null;

    // クリア時パネル
    [SerializeField]
    private GameObject _gameResultPanel = null;

    [SerializeField]
    private TextMeshProUGUI _textStepTitle = null;
    [SerializeField]
    private TimerWidget _timerWidget = null;
    [SerializeField]
    private Text _currentStepText = null;
    [SerializeField]
    private List<TextMeshProUGUI> _stageLabelList = new List<TextMeshProUGUI>();


    // 今回のゲームのレベル(難易度).
    private int _stageLevel = 1;
    // 現在のステップ.
    private int _currentStep = 1;
    // 最大ステップ.
    private int _maxStep = Int32.MaxValue;


    // 各ステップの結果情報のリスト.
    private List<StepResult> _stepResultList = new List<StepResult>();

    // 各種マスタ保持クラス,
    MasterStepHolder _masterStepHolder = null;

    public void Awake()
    {
        Debug.Assert(_fukidashisController != null);
        Debug.Assert(_animCharaController  != null);
        Debug.Assert(_gameResultPanel      != null);
        Debug.Assert(_textStepTitle        != null);
        Debug.Assert(_timerWidget          != null);
        Debug.Assert(_currentStepText      != null);
        Debug.Assert(_stageLabelList.Count == LevelManager.Instance.MaxLevel);

        _masterStepHolder     = MasterStepHolder.Instance();

        _stageLevel = LevelManager.Instance.Level;
        _currentStep = 1;
        _maxStep = _masterStepHolder.GetMaxStep(_stageLevel);
    }

    private void Start()
    {
        // UI関連の初期化.
        _timerWidget.Initialize((int)InGameMaxTime, () => GameEnd() );
        _timerWidget.Start();
        UpdateCurrentStepText(_currentStep);
        ToggleStageLabelImage(_stageLevel);

        StartStep(_currentStep);

        // 以下のコードで､アニメーションの再生時間を取得できる.
        // var track1 = _fukidashiPrefab.GetComponent<SkeletonGraphic>().AnimationState.SetAnimation(0, "maru", false);
        // var track2 = _fukidashiPrefab.GetComponent<SkeletonGraphic>().AnimationState.SetAnimation(0, "batu", false);
        // Debug.LogFormat("maru animation time: {0}", track1.AnimationEnd);
        // Debug.LogFormat("batu animation time: {0}", track2.AnimationEnd);
    }

    // 各ステップの開始時処理を行う.
    private void StartStep(int step)
    {
        if (step > _maxStep) {
            // 最大ステップ数を超えたらゲーム終了処理.
            GameEnd();
            return;
        }

        MasterItemStep stepMaster = _masterStepHolder.GetOneOrFail(_stageLevel, _currentStep);
        _textStepTitle.text  = stepMaster.Title;
        UpdateCurrentStepText(_currentStep);
        
        _fukidashisController.InstantiateObjects(step, OnTouchFukidashiObject);
    }

    // ゲーム終了時処理を行う.
    private void GameEnd()
    {
        // タイマーを止める(タイムアウト処理が発生しないように).
        _timerWidget.Stop();

        // 残っている吹き出しを削除.
        _fukidashisController.ClearObjects();

        // キャラのクリア演出.
        _animCharaController.SetAnimation(CharaAnimName.GameClear);

        // クリアパネルの表示.
        int score = SumUpScore();
        _gameResultPanel.SetActive(true);
        _gameResultPanel.GetComponent<SkeletonGraphic>().AnimationState.SetAnimation(0, GameClearPanelAnimName.Start, false);
        _gameResultPanel.GetComponent<GameClearPanel>().SetTexts(_timerWidget.GetRemainTime(), score);
    }

    private void OnTouchFukidashiObject(bool isSuccess)
    {
        // 背景キャラに指定のアニメーションをさせる.
        string animationName = (isSuccess) ? CharaAnimName.Success : CharaAnimName.Fail;
        _animCharaController.SetAnimation(animationName);

        // 現在のステップの結果を保存しておく.
        int score = (isSuccess)
            ? _masterStepHolder.GetOneOrFail(_stageLevel, _currentStep).Score
            : 0;
        StepResult packege = new StepResult()
        {
            Step       = _currentStep,
            IsSuccess  = isSuccess,
            RemainTime = _timerWidget.GetRemainTime(),
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

    private void UpdateCurrentStepText(int step)
    {
        _currentStepText.text = step.ToString("000");
    }

    private void ToggleStageLabelImage(int stage)
    {
        _stageLabelList.Select( (label)  => { label.gameObject.SetActive(false); return label; } );
        _stageLabelList[stage - 1].gameObject.SetActive(true);
    }
}