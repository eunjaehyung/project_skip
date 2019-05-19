using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 各種定数管理用のファイル.

// 背景キャラ(ハッピーちゃん)のアニメ名管理クラス.
public class CharaAnimName
{
    // 通常時.
    public static readonly string Wait      = "wait";
    // 正解時.
    public static readonly string Success   = "happy";
    // 不正解時.
    public static readonly string Fail      = "pants";
    // public static readonly string Fail    = "strip";
    // ゲームクリア時.
    public static readonly string GameClear = "skip";
}
