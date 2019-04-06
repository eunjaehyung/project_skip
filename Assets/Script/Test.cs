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
        string file = "/Resources/MasterTable/MasterTable";
        
        string dir = Application.dataPath + file + ".csv";
        if (!File.Exists(dir))
        {
            Debug.Log(file + "파일이 존재하지않습니다.");
        }
        
        string source;
        StreamReader sr = new StreamReader(dir);
        source = sr.ReadToEnd();
        sr.Close();
        List<Dictionary<string, object>> contain = CSVReader.Read(source);
        
        Debug.Log(contain);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
