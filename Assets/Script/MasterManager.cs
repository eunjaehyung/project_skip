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
    
    const string table_Question = "/Resources/MasterTable/QuestionTable";
    List<Dictionary<string, object>> list_table_question = new List<Dictionary<string, object>>();

    public void LoadTableDatas()
    {
        list_table_question = LoadTable(table_Question);
    }
    
    private List<Dictionary<string, object>> LoadTable(string path)
    {
        string dir = Application.dataPath + path + ".csv";
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
}
