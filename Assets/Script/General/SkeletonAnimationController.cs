using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Spine;
using Spine.Unity;

// アニメ情報定義用構造体.
[System.Serializable]
public class AnimationInfo
{
    [SerializeField]
    private string _animationName = "";
    public string AnimationName
    {
        get { return _animationName; }
    }

    [SerializeField]
    private string _nextAnimationName = "";
    public string NextAnimationName
    {
        get { return _nextAnimationName; }
    }

    [SerializeField]
    private bool _isStartAnimation = false;
    public bool IsStart
    {
        get { return _isStartAnimation; }
    }

    [SerializeField]
    private int _trackIndex = 0;
    public int TrackIndex
    {
        get { return _trackIndex; }
    }

    [SerializeField]
    private bool _isLoopAnimation = false;
    public bool IsLoop
    {
        get { return _isLoopAnimation; }
    }
}

// Spineアニメーション管理用クラス. 主な責務は以下.
// ・アニメーションの実行(SkeletonAnimationのラッパ).
// ・起動時に指定したアニメーションの発火.
// ・アニメーション終了後､別のアニメーションを再生する.
[RequireComponent(typeof(SkeletonAnimation))]
public class SkeletonAnimationController : MonoBehaviour
{
    [SerializeField]
    private AnimationInfo[] _animationInfoArray = new AnimationInfo[]{};

    private SkeletonAnimation _skeletonAnimation = null;

    public void Start()
    {
        _skeletonAnimation = GetComponent<SkeletonAnimation>();

        // 起動時アニメーションの設定.
        var startAnimInfo = GetInfoWithCheck( info => info.IsStart );
        _skeletonAnimation.AnimationState.SetAnimation(
            startAnimInfo.TrackIndex,
            startAnimInfo.AnimationName,
            startAnimInfo.IsLoop);

        // アニメーション遷移用のコールバックの登録.
        _skeletonAnimation.AnimationState.Complete += (TrackEntry trackEntry) => {
            var animInfo = GetInfoWithCheck( info => info.AnimationName == trackEntry.Animation.Name );
            
            // ループだったり､遷移先の無いアニメーションならreturn.
            if (animInfo.IsLoop) {
                return;
            }
            if (animInfo.NextAnimationName == "") {
                return;
            }

            // 次のアニメーションを再生する.
            var nextAnimInfo = GetInfoWithCheck( info => info.AnimationName == animInfo.NextAnimationName);
            _skeletonAnimation.AnimationState.SetAnimation(
                nextAnimInfo.TrackIndex,
                nextAnimInfo.AnimationName,
                nextAnimInfo.IsLoop);
        };
    }

    // SkeletonAnimationの同名メソッドのラッパー処理.
    public void SetAnimation(string animationName)
    {
        var animInfo = GetInfoWithCheck( info => info.AnimationName == animationName );
        _skeletonAnimation.AnimationState.SetAnimation(
            animInfo.TrackIndex,
            animInfo.AnimationName,
            animInfo.IsLoop);
    }

    // SkeletonAnimationの同名メソッドのラッパー処理.
    public void AddAnimation(string animationName, float delayTime = 0.0f)
    {
        var animInfo = GetInfoWithCheck( info => info.AnimationName == animationName );
        _skeletonAnimation.AnimationState.AddAnimation(
            animInfo.TrackIndex,
            animInfo.AnimationName,
            animInfo.IsLoop,
            delayTime);
    }

    private AnimationInfo GetInfoWithCheck(Func<AnimationInfo, bool> filterFunc)
    {
        var infoArray = _animationInfoArray.Where(filterFunc);
        if (infoArray.Count() <= 0 || infoArray.Count() >= 2) {
            Debug.LogFormat("Elements num was infalid. num:{0}", infoArray.Count());
            Debug.Assert(false);
        }
        return infoArray.ToArray()[0];
    }
}
