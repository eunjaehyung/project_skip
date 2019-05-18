using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Spine;
using Spine.Unity;
using UnityEngine;

public class Test : MonoBehaviour
{
    public SkeletonAnimation anim;
    
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<SkeletonAnimation>();
        
        
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            anim.AnimationName = "";
            anim.AnimationName = "happy";   
        }
        
        if (Input.GetKeyDown(KeyCode.B))
        {
            anim.AnimationName = "skip";   
        }
    }
}
