using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MasterItemFukidashi
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
    private int _x;
    public int X
    {
        get { return _x; }
    }

    [SerializeField]
    private int _y;
    public int Y
    {
        get { return _y; }
    }

    public MasterItemFukidashi(Dictionary<string, object> dict)
    {
        Debug.Assert(dict.ContainsKey("title"));
        Debug.Assert(dict.ContainsKey("answer_id"));
        Debug.Assert(dict.ContainsKey("stage"));
        Debug.Assert(dict.ContainsKey("step"));
        Debug.Assert(dict.ContainsKey("x"));
        Debug.Assert(dict.ContainsKey("y"));

        _title    = (string) dict["title"];
        _answerId = (int)    dict["answer_id"];
        _stage    = (int)    dict["stage"];
        _step     = (int)    dict["step"];
        _x        = (int)    dict["x"];
        _y        = (int)    dict["y"];
    }
}
