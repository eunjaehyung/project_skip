using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using UnityEngine;
using UnityEngine.UI;

public class SkipObject : MonoBehaviour
{
    public int PositionKey;
    public Text _text;
    private int answerId;

    public Action<int> ClickCallback;

    public void SetData(Dictionary<string, object> dicData)
    {
        _text.text = dicData["title"].ToString();

        answerId = (int)dicData["answer_id"];
    }

    public void SetPosition(int key, Vector3 pos)
    {
        PositionKey = key;
        
        transform.localPosition = pos;
    }

    public void OnClick()
    {
        ClickCallback(answerId);
    }
}
