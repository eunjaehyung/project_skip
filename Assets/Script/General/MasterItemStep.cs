using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MasterItemStep
{
    [SerializeField]
    private string _title;
    public string Title
    {
        get { return _title; }
    }

    [SerializeField]
    private int _answerId;
    public int AnswerId
    {
        get { return _answerId; }
    }

    [SerializeField]
    private int _stage;
    public int Stage
    {
        get { return _stage; }
    }

    [SerializeField]
    private int _step;
    public int Step
    {
        get { return _step; }
    }

    [SerializeField]
    private int _score;
    public int Score
    {
        get { return _score; }
    }

    public MasterItemStep(Dictionary<string, object> dict)
    {
        Debug.Assert(dict.ContainsKey("title"));
        Debug.Assert(dict.ContainsKey("answer_id"));
        Debug.Assert(dict.ContainsKey("stage"));
        Debug.Assert(dict.ContainsKey("step"));
        Debug.Assert(dict.ContainsKey("score"));

        _title    = (string) dict["title"];
        _answerId = (int)    dict["answer_id"];
        _stage    = (int)    dict["stage"];
        _step     = (int)    dict["step"];
        _score    = (int)    dict["score"];
    }
}