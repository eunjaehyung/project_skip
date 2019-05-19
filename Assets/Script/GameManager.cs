using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public Canvas Canvas;
    public Text TextAnswer;
    public Text TextResult;
    public TimerWidget timerWidget;
    public GameResultPanel GameResultPanel;
    private int questionId;
    public SkipObject SkipObject;
    private int StepCount = 1;
    private List<GameObject> listAnswer = new List<GameObject>();
    private List<GameResultPackege> ResultPackeges = new List<GameResultPackege>();
    public ActorScript actor;

    void Awake()
    {
        MasterManager.Instance.LoadTableDatas();
        
        timerWidget.SetTime(20, () =>
        {
            Debug.Log("Time Out");
        });
        
        timerWidget.StartCountDown();
        
        StartStage();
    }

    private void StartStage()
    {
        if (StepCount == 4)
        {
            timerWidget.StopTime = true;
            GameResultPanel.SetData(timerWidget.GetRemainTime().ToString(), "0");
            InfoManager.Instance.SetRecord(ResultPackeges);
            return;
        }
        
        object title = MasterManager.Instance.GetQuestionData(StepCount, "title");
        
        questionId = (int)MasterManager.Instance.GetQuestionData(StepCount, "answer_id");

        TextAnswer.text = title.ToString();
        
        CreateSkipObject();
    }
    
    private bool CheckAnswer(int answer)
    {
        return questionId == answer;
    }

    private void GetAnswerList()
    {
        MasterManager.Instance.GetQuestionData(StepCount, "title");
    }

    private void CreateSkipObject()
    {
        List<Dictionary<string, object>> list = MasterManager.Instance.GetCulumnListForCulumnKey("stage", InfoManager.Instance.GameLevel);

        for (int i = 0; i < list.Count; ++i)
        {
            if (list[i]["step"].ToString() != StepCount.ToString())
                continue;
            
            GameObject skipObj = Instantiate(SkipObject.gameObject, Canvas.transform);
        
            SkipObject skip = skipObj.GetComponent<SkipObject>();

            skip.SetData(list[i]);

            skip.ClickCallback = OnClickSkipEvent;
            
            listAnswer.Add(skipObj);
        }
    }

    private void OnClickSkipEvent(int result)
    {
        for (int i = 0; i < listAnswer.Count; ++i)
        {
            Destroy(listAnswer[i]);
        }
        
        listAnswer.Clear();
        
        bool isSuccess = CheckAnswer(result);

        GameResultPackege packege = new GameResultPackege()
        {
            Step = StepCount,
            IsSuccess = isSuccess,
            RemainTime = timerWidget.GetRemainTime(),
            AnswerId = questionId,
        };
        
        ResultPackeges.Add(packege);
        
        StepCount++;

        TextResult.text = isSuccess ? "正解" : "失敗";

        if (isSuccess)
        {
            actor.SetAnimation("skip");
        }
        else
        {
            actor.SetAnimation("pants");
        }
        
        StartCoroutine(FuncDelayEvent(1.5f, () =>
        {
            TextResult.text = "";
            
            StartStage();
        }));
    }

    private IEnumerator FuncDelayEvent(float delaySec, Action callback)
    {
        yield return new WaitForSeconds(delaySec);

        callback();
    }
}

public class GameResultPackege
{
    public int Step;
    public bool IsSuccess;
    public int AnswerId;
    public float RemainTime;
    public int Score;
}