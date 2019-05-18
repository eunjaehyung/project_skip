using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

[RequireComponent(typeof(SkeletonGraphic))]
public class FukidashiTest : MonoBehaviour
{
    [SerializeField]
    private string _animationName = "fukidashi";

    void Start()
    {
        SkeletonGraphic skeletonGraphic = GetComponent<SkeletonGraphic>();
        skeletonGraphic.AnimationState.SetAnimation(0, _animationName, true);
        Debug.LogFormat("Start Animation ! {0}", _animationName);
    }

    public void OnClick()
    {
        Debug.Log("clicked.");
    }
}
