using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Spine.Unity;

[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(SkeletonGraphic))]
public class FukidashiController : MonoBehaviour
{
    // Spineアニメーション名に関する定数.
    private const string AnimNameFukidashi = "fukidashi";
    private const string AnimNameMaru = "maru";
    private const string AnimNameBatu = "batu";

    private uint _answerId = Int32.MaxValue;
    public uint AnswerId
    {
        get { return _answerId; }
    }

    private Action<FukidashiController> _touchCallback = null;
    public Action<FukidashiController> TouchCallback
    {
        set { _touchCallback = value; }
    }

    public void Start()
    {
        SkeletonGraphic skeletonGraphic = GetComponent<SkeletonGraphic>();
        
        InitAnimationStateCallbacks();

        skeletonGraphic.AnimationState.SetAnimation(0, AnimNameFukidashi, true);
    }

    // このオブジェクトに対する正解時の動作を適用する.
    public void Success()
    {
        GetComponent<SkeletonGraphic>().AnimationState.SetAnimation(0, AnimNameMaru, false);
    }

    // このオブジェクトに対する不正解時の動作を適用する.
    public void Fail()
    {
        GetComponent<SkeletonGraphic>().AnimationState.SetAnimation(0, AnimNameBatu, false);
    }

    public void SetData(Dictionary<string, object> dict)
    {
        GetComponent<RectTransform>().anchoredPosition = new Vector3((int)dict["x"], (int)dict["y"], 0);

        Text[] texts = this.GetComponentsInChildren<Text>();
        if (texts.Length >= 1) {
            texts[0].text = dict["title"].ToString();
        }

        _answerId = (uint)dict["answer_id"];
    }

    public void OnClick()
    {
        if (_touchCallback != null) {
            _touchCallback(this);
        }
    }

    // 各種アニメーションコールバックの初期化.
    private void InitAnimationStateCallbacks()
    {
        SkeletonGraphic skeletonGraphic = GetComponent<SkeletonGraphic>();

        skeletonGraphic.AnimationState.Start += (Spine.TrackEntry trackEntry) => {
            Debug.LogFormat("Animation start! Name:{0}", trackEntry.Animation.Name);
            if (trackEntry.Animation.Name == AnimNameMaru ||
                trackEntry.Animation.Name == AnimNameBatu) {
                transform.GetChild(0).gameObject.SetActive(false);
            }
        };

        // ※ ↓ 本来ならばCompleteではなくEndとしたい.
        // http://ja.esotericsoftware.com/spine-unity-events
        skeletonGraphic.AnimationState.Complete += (Spine.TrackEntry trackEntry) => {
            Debug.LogFormat("Animation complete! Name:{0}", trackEntry.Animation.Name);
            if (trackEntry.Animation.Name == AnimNameMaru ||
                trackEntry.Animation.Name == AnimNameBatu) {
                gameObject.SetActive(false);
            }
        };
    }
}
