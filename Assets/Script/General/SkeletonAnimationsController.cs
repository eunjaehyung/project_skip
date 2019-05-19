using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

public class SkeletonAnimationsController : MonoBehaviour
{
    [SerializeField]
    private SkeletonAnimation[] _animations = new SkeletonAnimation[]{}; // 対象のSpineObject

    public void SetAnimation(int listIndex, string animationName, int animationTrackIndex = 0, bool isLoop = false)
    {
        if (_animations.Length < listIndex + 1) {
            Debug.LogFormat("Target element not found. index: {0}", listIndex);
            return;
        }

        Spine.AnimationState animationState = _animations[listIndex].state;
        animationState.SetAnimation(animationTrackIndex, animationName, isLoop);
    }

    public void AddAnimation(int animationIndex, string animationName)
    {
        
    }
}
