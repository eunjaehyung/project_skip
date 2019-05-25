using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// TODO: 
// MasterFukidashiHolderクラスと､MasterStepHolderクラスの実装が重複してしまっています.
// 抽象マスタ保持クラスを上手く定義できれば､重複を排除できそうです.
// 以下のエントリが参考になりそうです.
// http://esprog.hatenablog.com/entry/2016/02/06/153832
public class MasterFukidashiHolder : MonoBehaviour
{
    private static MasterFukidashiHolder _instance = null;

    // TODO: 抽象クラスにまとめるために､保持するマスタの型を､ジェネリックな型とする.
    private List<MasterItemFukidashi> _items = new List<MasterItemFukidashi>();

    private List<string> _masterKeys = new List<string>{
        "id",
        "title",
        "answer_id",
        "stage",
        "step",
        "x",
        "y",
        // 以下キーは､必要になったらコメントインする.
        // "se",
        // "anim_id",
    };

    // TODO: 抽象クラスにまとめるために､ジェネリックな型を指定できるようにする.
    public static MasterFukidashiHolder Instance()
    {
        if (_instance != null) {
            return _instance;
        }

        var gameObject = new GameObject("MasterFukidashiHolder");
        _instance = gameObject.AddComponent<MasterFukidashiHolder>();
        _instance.ReadMasterDataFromFile();

        DontDestroyOnLoad(_instance);

        return _instance;
    }

    // TODO: 抽象クラスにまとめるために､この箇所はジェネリックな型を返すようにする.
    public MasterItemFukidashi GetOneOrFail(Func<MasterItemFukidashi, bool> predicate)
    {
        foreach (var item in _items) {
            if (predicate(item)) {
                return item;
            }
        }
        Debug.Assert(false);
        return null;
    }

    private void ReadMasterDataFromFile()
    {
        // TODO: 抽象クラスにまとめるために､マスタファイルパス取得処理を､抽象メソッド化する.
        List<Dictionary<string, object>> rawDataList = CsvParser.ReadAndParse(MasterFilePath.Fukidashi);
        foreach (var rawData in rawDataList) {
            ParseMasterData(rawData);
            var item = new MasterItemFukidashi(rawData);
            _items.Add(item);
        }
    }

    // TODO: 抽象クラスにまとめるために､マスタのパース処理を抽象メソッドにして､オーバーライド必須にする.
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
}
