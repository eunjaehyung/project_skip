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

public class GameManager : MonoBehaviour
{
    // 次のステップ開始までの遅延時間 ※正否アニメーションの尺の違いにより､2パターン用意している.
    // TODO: アニメーション終了時間をハードコーディングするのではなく､雛形となるSkeletonGraphicから値を動的に取得したい.
    private const float NextStepDelayTimeSuccess = 2.23f;
    private const float NextStepDelayTimeFail = 1.5f;

    // このゲームの最大時間 ※この秒以上経過した場合､ゲーム終了.
    private const float InGameMaxTime = 99.0f;

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
    private TextMeshProUGUI _textStepTitle = null;
    [SerializeField]
    private TimerWidget _timerWidget = null;


    // 今回のゲームのレベル(難易度).
    private int _stageLevel = 1;
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

    // 各種マスタ保持クラス,
    MasterFukidashiHolder _masterFukidashHolder = null;
    MasterStepHolder _masterStepHolder = null;

    public void Awake()
    {
        Debug.Assert(_animCharaController != null);
        Debug.Assert(_gameResultPanel     != null);
        Debug.Assert(_fukidashiPrefab     != null);
        Debug.Assert(_canvas              != null);
        Debug.Assert(_textStepTitle       != null);
        Debug.Assert(_timerWidget         != null);

        _masterFukidashHolder = MasterFukidashiHolder.Instance();
        _masterStepHolder     = MasterStepHolder.Instance();

        _stageLevel = InfoManager.Instance.GameLevel;
        _currentStep = 1;
        _maxStep = _masterStepHolder.GetMaxStep(_stageLevel);

        _timerWidget.Initialize((int)InGameMaxTime, () => GameEnd() );
        _timerWidget.Start();

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
        _currentStepAnswerId = stepMaster.AnswerId;
        _textStepTitle.text  = stepMaster.Title;
        
        CreateFukidashiObjects(step);
    }

    // TODO: このメソッドは､吹き出し管理クラスを作って､そちらに処理を委譲するべきです.
    // 各ステップにおける､吹き出しオブジェクトの生成を行う.
    private void CreateFukidashiObjects(int step)
    {
        List<MasterItemFukidashi> masterList = _masterFukidashHolder.GetList(
            (item) => { return item.Stage == _stageLevel && item.Step == _currentStep; }
        );

        foreach (var master in masterList) {
            GameObject fukidashiGameObj = Instantiate(_fukidashiPrefab, _canvas.transform);
            FukidashiController fukidashiObj = fukidashiGameObj.GetComponent<FukidashiController>();
            Debug.Assert(fukidashiObj != null);

            fukidashiObj.SetMasterData(master);
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