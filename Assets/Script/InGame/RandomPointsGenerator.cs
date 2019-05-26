using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;


[System.Serializable]
public class RandomPointStruct
{
    [SerializeField]
    private Vector2 _point = new Vector2(0.0f, 0.0f);
    public Vector2 Point
    {
        get { return _point; }
    }

    [SerializeField]
    private int _weight = 1;
    public int Weight
    {
        get { return _weight; }
    }

    // 有効フラグ.
    // (これが折れているインスタンスは使用済みと判断し､座標のランダム生成時に無視します.)
    private bool _isActive = true;
    public bool Active
    {
        get { return _isActive; }
        set { _isActive = value; }
    }

    public override string ToString()
    {
        return String.Format("Point:{0}, Weight:{1}, IsActive:{2}", _point, _weight, _isActive);
    }
}

public class RandomPointsGenerator : MonoBehaviour
{
    [SerializeField]
    private Canvas _canvas = null;

    [SerializeField]
    private GameObject _debugPointPrefab = null;
    
    [SerializeField]
    private bool _isDebugPrintEnable = false;

    [SerializeField]
    private int _level = 1;
    public int Level
    {
        get { return _level; }
    }

    [SerializeField]
    private List<RandomPointStruct> _pointsList = new List<RandomPointStruct>();

    private void Start()
    {
        Debug.Assert(_canvas != null);
        Debug.Assert(_debugPointPrefab != null);
        Debug.Assert(_level <= LevelManager.Instance.MaxLevel);

        // デバッグ用に､座標情報を可視化する.
        InstantiateDebugPoints();

        // 念の為､全座標を有効化.
        Reset();
    }

    public Vector2 PickPointRandomly()
    {
        int sum = CalcWeightSum() + 1;
        int rand = UnityEngine.Random.Range(1, sum);

        int current = 0;
        foreach (var point in _pointsList) {
            if (! point.Active) { 
                continue;
            }

            current += point.Weight;
            if (current >= rand) {
                point.Active = false;
                Debug.Log(point.ToString());
                return point.Point;
            }
        }

        Debug.AssertFormat(false, "All Points had already been picked.");
        return new Vector2(0.0f, 0.0f);
    }

    // 座標情報をリセットする.
    public void Reset()
    {
        foreach (var point in _pointsList) {
            point.Active = true;
        }
    }

    private int CalcWeightSum()
    {
        int weightSum = 0;
        weightSum = _pointsList
            .Where( (item) => { return item.Active; } )
            .Select( (item) => { return item.Weight; } )
            .Sum();
        return weightSum;
    }

    private void InstantiateDebugPoints()
    {
        if (! _isDebugPrintEnable) { return; }

        foreach (var point in _pointsList) {
            GameObject debugPointObj = Instantiate(_debugPointPrefab, _canvas.transform);
            debugPointObj.GetComponent<Text>().text = point.Weight.ToString();
            debugPointObj.GetComponent<RectTransform>().anchoredPosition = point.Point;
        }
    }
}
