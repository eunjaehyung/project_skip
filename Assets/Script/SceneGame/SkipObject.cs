using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using UnityEngine;
using UnityEngine.UI;

public class SkipObject : MonoBehaviour
{
    public Text _text;
    private int answerId;

    public Action<int> ClickCallback;

    public void SetData(Dictionary<string, object> dicData)
    {
        transform.position = new Vector3((int)dicData["x"], (int)dicData["y"], 0);

        _text.text = dicData["title"].ToString();

        answerId = (int)dicData["answer_id"];
    }

    public void OnClick()
    {
        ClickCallback(answerId);
    }
}
