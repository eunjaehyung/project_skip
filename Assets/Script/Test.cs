using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;

public class Test : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        MasterManager.Instance.LoadTableDatas();

        object value = MasterManager.Instance.GetQuestionData(1, "title");
        
        Debug.Log(value);
        
        object value1 = MasterManager.Instance.GetQuestionData(2, "title");
        
        Debug.Log(value1);
        
        object value2 = MasterManager.Instance.GetQuestionData(3, "title");
        
        Debug.Log(value2);
    }
}
