using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Spine;
using Spine.Unity;

[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(SkeletonGraphic))]
[RequireComponent(typeof(AudioSource))]
public class FukidashiController : MonoBehaviour
{
    // Spineアニメーション名に関する定数.
    [SerializeField]
    private const string AnimNameFukidashi = "fukidashi";
    [SerializeField]
    private const string AnimNameMaru = "maru";
    [SerializeField]
    private const string AnimNameBatu = "batu";

    [SerializeField]
    private const int FontSize = 60;

    // 正解・不正解時SE
    [SerializeField]
    private AudioClip _seSuccess = null;
    [SerializeField]
    private AudioClip _seFail = null;

    private int _answerId = Int32.MaxValue;
    public int AnswerId
    {
        get { return _answerId; }
    }

    // 既にタッチ済みか? (※複数回タップ防止用)
    private bool _hasTouched = false;
    public bool HasTouched
    {
        get { return _hasTouched; }
    }

    // タッチ時コールバック処理.
    private Action<FukidashiController> _touchCallback = null;
    public Action<FukidashiController> TouchCallback
    {
        set { _touchCallback = value; }
    }

    public void Start()
    {
        Debug.Assert(_seSuccess != null);
        Debug.Assert(_seFail != null);

        SkeletonGraphic skeletonGraphic = GetComponent<SkeletonGraphic>();
        
        InitAnimationStateCallbacks();

        TrackEntry trackEntry = skeletonGraphic.AnimationState.SetAnimation(0, AnimNameFukidashi, true);
        float randStartTime = UnityEngine.Random.Range(trackEntry.AnimationStart, trackEntry.AnimationEnd);
        trackEntry.TrackTime = randStartTime;
    }

    // このオブジェクトに対する正解時の動作を適用する.
    public void Success()
    {
        _hasTouched = true;
        GetComponent<SkeletonGraphic>().AnimationState.SetAnimation(0, AnimNameMaru, false);
        GetComponent<AudioSource>().PlayOneShot(_seSuccess);
    }

    // このオブジェクトに対する不正解時の動作を適用する.
    public void Fail()
    {
        _hasTouched = true;
        GetComponent<SkeletonGraphic>().AnimationState.SetAnimation(0, AnimNameBatu, false);
        GetComponent<AudioSource>().PlayOneShot(_seFail);
    }

    public void SetMasterData(MasterItemFukidashi master)
    {
        GetComponent<RectTransform>().anchoredPosition = new Vector3((float)master.X, (float)master.Y, 0.0f);

        TextMeshProUGUI[] texts = this.GetComponentsInChildren<TextMeshProUGUI>();
        if (texts.Length >= 1) {
            texts[0].text = master.Title;
            texts[0].fontSize = FontSize;
        } else {
            Debug.AssertFormat(false, "Text Object was not set.");
        }

        _answerId = master.AnswerId;
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
            //Debug.LogFormat("Animation start! Name:{0}", trackEntry.Animation.Name);
            if (trackEntry.Animation.Name == AnimNameMaru ||
                trackEntry.Animation.Name == AnimNameBatu) {
                transform.GetChild(0).gameObject.SetActive(false);
            }
        };

        // ※ ↓ 本来ならばCompleteではなくEndとしたい.
        // http://ja.esotericsoftware.com/spine-unity-events
        skeletonGraphic.AnimationState.Complete += (Spine.TrackEntry trackEntry) => {
            //Debug.LogFormat("Animation complete! Name:{0}", trackEntry.Animation.Name);
            if (trackEntry.Animation.Name == AnimNameMaru ||
                trackEntry.Animation.Name == AnimNameBatu) {
                Destroy(gameObject);
            }
        };
    }
}
