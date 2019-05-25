using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitlePanel : MonoBehaviour
{
    public enum LEVEL { EASY = 1, NORMAL, HARD }

    private LEVEL _level;

    public void SetLevel(int level)
    {
        LevelManager.Instance.Level = level;
        
        _level = (LEVEL) level;
    }

    public void OnClickSkip()
    {
        SceneManager.LoadSceneAsync("SceneGame");
    }
}
