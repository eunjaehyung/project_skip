using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;

public class ActorScript : MonoBehaviour
{
    private SkeletonAnimation _anim;
    void Start()
    {
        _anim = GetComponent<SkeletonAnimation>();
    }

    public void SetAnimation(string animName)
    {
        Spine.AnimationState state = _anim.state;
        state.SetAnimation(0, animName, false);
    }
}


