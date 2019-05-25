using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class LevelButton : MonoBehaviour
{
    [SerializeField]
    private Text _buttonLabel = null;

    [SerializeField]
    private string _htmlColorOn = "#00E4CA";

    [SerializeField]
    private string _htmlColorOff = "#404040";

    private Color _colorOn;
    private Color _colorOff;

    void Awake()
    {
        Debug.Assert(_buttonLabel != null);

        bool parseResult = true;
        parseResult = ColorUtility.TryParseHtmlString(_htmlColorOn,  out _colorOn)  && parseResult;
        parseResult = ColorUtility.TryParseHtmlString(_htmlColorOff, out _colorOff) && parseResult;
        Debug.Assert(parseResult);
    }

    void Start()
    {
        var toggle = GetComponent<Toggle>();
        toggle.onValueChanged.AddListener(Toggle);
    }

    public void Toggle(bool isOn)
    {
        GetComponent<Toggle>().isOn = isOn;
        _buttonLabel.color = (isOn) ? _colorOn : _colorOff;
    }
}
