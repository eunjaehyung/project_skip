using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MasterManager
{
    private static MasterManager instance;
    public static MasterManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new MasterManager();
            }

            return instance;
        }
    }

    const string table_Question = "/MasterTable/QuestionTable";
    const string table_answer = "/MasterTable/AnswerTable";
    List<Dictionary<string, object>> list_table_question = new List<Dictionary<string, object>>();
    List<Dictionary<string, object>> list_table_answer = new List<Dictionary<string, object>>();

    public void LoadTableDatas()
    {
        list_table_question = LoadTable(table_Question);
        list_table_answer = LoadTable(table_answer);
    }
    
    private List<Dictionary<string, object>> LoadTable(string path)
    {
        string dir = Application.streamingAssetsPath + path + ".csv";

        if (!File.Exists(dir))
        {
            Debug.Log(path + "파일이 존재하지않습니다.");
        }

        string source;
        StreamReader sr = new StreamReader(dir);
        source = sr.ReadToEnd();
        return CSVReader.Read(source);
    }

    public object GetQuestionData(int id, string key)
    {
        if (list_table_question.Count < id - 1)
            return null;

        Dictionary<string, object> dic = list_table_question[id - 1];

        return dic[key];
    }

    public object GetAnswerData(int id, string key)
    {
        if (list_table_answer.Count < id - 1)
            return null;
        
        Dictionary<string, object> dic = list_table_answer[id - 1];

        return dic[key];
    }

    public int GetScoreFromQuestionData(int stage, int step)
    {
        for (int i = 0; i < list_table_question.Count; ++i) {
            int curStage =  (int)list_table_question[i]["stage"];
            int curStep  =  (int)list_table_question[i]["step"];
            if (stage == curStage && step == curStep) {
                return (int)list_table_question[i]["score"];
            }
        }
        return 0;
    }

    public int GetMaxStepFromQuestionData()
    {
        int maxStep = 0;
        for (int i = 0; i < list_table_question.Count; ++i) {
            int step =  (int)list_table_question[i]["step"];
            maxStep = Math.Max(step, maxStep);
        }
        return maxStep;
    }

    public List<Dictionary<string, object>> GetCulumnListForCulumnKeyFromAnswerData(string key, object value)
    {
        List<Dictionary<string, object>> resultData = new List<Dictionary<string, object>>();
            
        for (int i = 0; i < list_table_answer.Count; ++i)
        {
            Dictionary<string, object> dic = list_table_answer[i];

            object result = dic[key];
            if (result.ToString() != value.ToString())
                continue;
            
            resultData.Add(list_table_answer[i]);
        }

        return resultData;
    }
}
