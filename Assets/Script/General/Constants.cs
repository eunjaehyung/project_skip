using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 各種定数管理用のファイル.

// 背景キャラ(ハッピーちゃん)のアニメ名一覧.
public class CharaAnimName
{
    // 通常時.
    public static readonly string Wait      = "wait";
    // 正解時.
    public static readonly string Success   = "happy";
    // 不正解時.
    public static readonly string Fail    = "strip";
    // ゲームクリア時.
    public static readonly string GameClear = "skip";
}

// ゲームクリア時パネルのアニメ名一覧.
public class GameClearPanelAnimName
{
    public static readonly string Start = "action";
}

// マスタファイルのパス一覧.
public class MasterFilePath
{
    public static readonly string Fukidashi = "/MasterTable/FukidashiTable.csv";
    public static readonly string Step = "/MasterTable/StepTable.csv";
}


