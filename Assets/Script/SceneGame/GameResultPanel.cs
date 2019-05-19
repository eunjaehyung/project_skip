using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameResultPanel : MonoBehaviour
{
    public Text txtTime;

    public Text txtScore;

    public void SetTexts(float time, int score)
    {
        txtTime.text = Math.Floor(time).ToString() + " sec";
        txtScore.text = score.ToString();
    }

    public void OnTouchPanel()
    {
        SceneManager.LoadSceneAsync("SceneTitle");
    }
}
