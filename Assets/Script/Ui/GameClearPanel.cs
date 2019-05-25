using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameClearPanel : MonoBehaviour
{
    public Text txtTime;

    public Text txtScore;

    public void SetTexts(float time, int score)
    {
        txtTime.text = time.ToString("00.00").Replace(".", ":");
        txtScore.text = score.ToString();
    }

    public void OnTouchPanel()
    {
        SceneManager.LoadSceneAsync("SceneTitle");
    }
}
