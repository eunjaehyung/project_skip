using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MasterStepHolder : MonoBehaviour
{
    private static MasterStepHolder _instance = null;
    private List<MasterItemStep> _items = new List<MasterItemStep>();

    private List<string> _masterKeys = new List<string>{
        "id",
        "title",
        "answer_id",
        "stage",
        "step",
        "score",
    };

    // TODO: 抽象クラスにまとめるために､ジェネリックな型を指定できるようにする.
    public static MasterStepHolder Instance()
    {
        if (_instance != null) {
            return _instance;
        }

        var gameObject = new GameObject("MasterStepHolder");
        _instance = gameObject.AddComponent<MasterStepHolder>();
        _instance.ReadMasterDataFromFile();

        DontDestroyOnLoad(_instance);

        return _instance;
    }

    public MasterItemStep GetOneOrFail(Func<MasterItemStep, bool> predicate)
    {
        foreach (var item in _items) {
            if (predicate(item)) {
                return item;
            }
        }
        Debug.Assert(false);
        return null;
    }

    public List<MasterItemStep> GetList(Func<MasterItemStep, bool> predicate)
    {
        return _items.Where(predicate).ToList();
    }

    private void ReadMasterDataFromFile()
    {
        List<Dictionary<string, object>> rawDataList = CsvParser.ReadAndParse(MasterFilePath.Step);
        foreach (var rawData in rawDataList) {
            ParseMasterData(rawData);
            var item = new MasterItemStep(rawData);
            _items.Add(item);
        }
    }

    private void ParseMasterData(Dictionary<string, object> rawData)
    {
        foreach (string key in _masterKeys) {
            Debug.AssertFormat(
                rawData.ContainsKey(key),
                "Key is not defined. Key name:{0}",
                key
            );
        }
    }

    public MasterItemStep GetOneOrFail(int stage, int step)
    {
        return GetOneOrFail( (item) => { return item.Stage == stage && item.Step == step; } );
    }

    public int GetMaxStep(int stage)
    {
        int step = 0;
        step = _items
            .Where( (item) => { return item.Stage == stage; } )
            .Max( (item) => { return item.Step; });
        return step;
    }
}
