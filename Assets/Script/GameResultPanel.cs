using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameResultPanel : MonoBehaviour
{
    public Text txtTime;

    public Text txtScore;

    public void SetData(string time, string score)
    {
        txtTime.text = "TIME : " + time;

        txtScore.text = "SCORE : " + score;
        
        gameObject.SetActive(true);
    }

    public void OnTouchPanel()
    {
        SceneManager.LoadSceneAsync("SceneTitle");
    }
}
