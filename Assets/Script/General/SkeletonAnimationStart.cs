using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

// インスタンス生成時に再生してほしいループアニメーションを指定するスクリプト.
[RequireComponent(typeof(SkeletonAnimation))]
public class SkeletonAnimationStart : MonoBehaviour
{
    [SerializeField]
    private string _startAnimationName = "";

    void Start()
    {
        GetComponent<SkeletonAnimation>().AnimationState.SetAnimation(0, _startAnimationName, true);
    }
}
