using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    private static LevelManager _instance;
    public static LevelManager Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject ins = new GameObject();
                _instance = ins.AddComponent<LevelManager>();
                DontDestroyOnLoad(ins);
            }

            return _instance;
        }
    }

    private int _gameLevel = 1;
    public int Level
    {
        get { return _gameLevel; }
        set { _gameLevel = value; }
    }
}