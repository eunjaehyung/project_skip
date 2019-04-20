using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public Canvas Canvas;
    public Text TextAnswer;
    private int questionId;
    public SkipObject SkipObject;
    private int StepCount = 1;
    private List<GameObject> listAnswer = new List<GameObject>();

    void Awake()
    {
        MasterManager.Instance.LoadTableDatas();
        
        StartStage();
    }

    private void StartStage()
    {
        if (StepCount == 4)
            StepCount = 1;
        
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
        List<Dictionary<string, object>> list = MasterManager.Instance.GetCulumnListForCulumnKey("stage", 1);

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
        bool isSuccess = CheckAnswer(result);
        
        Debug.Log(isSuccess);
        
        StepCount++;

        for (int i = 0; i < listAnswer.Count; ++i)
        {
            Destroy(listAnswer[i]);
        }
        
        listAnswer.Clear();
        
        StartStage();
    }
}
