using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class TitleSceneController : MonoBehaviour
{
    [SerializeField]
    private List<LevelButton> _levelButtons;

    // Start is called before the first frame update
    void Start()
    {
        // 現在のレベルに対応する難易度ボタンを有効にする.
        int level = LevelManager.Instance.Level;
        Debug.Assert(_levelButtons.Count >= level);
        _levelButtons.Select ( (levelButton) => { levelButton.Toggle(false); return levelButton;} );
        _levelButtons[level -1].Toggle(true);
    }
}
