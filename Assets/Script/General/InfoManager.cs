using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfoManager : MonoBehaviour
{
    private static InfoManager instance;
    public static InfoManager Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject ins = new GameObject();
                instance = ins.AddComponent<InfoManager>();
                DontDestroyOnLoad(ins);
            }

            return instance;
        }
    }

    public int GameLevel = 1;
    public GameRecord GameRecord = new GameRecord();

    public void SetRecord(List<StepResult> data)
    {
        GameRecord.listRecord.AddRange(data);
    }
}

public class GameRecord
{
    public List<StepResult> listRecord = new List<StepResult>();
}
