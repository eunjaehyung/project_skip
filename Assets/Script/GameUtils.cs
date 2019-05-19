using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameUtils
{
    public static Vector3 GetRandomPosition()
    {
        Vector3 vec;
        float x = Random.Range(0, Screen.width);
        float y = Random.Range(0, Screen.height);

        vec.x = x;
        vec.y = y;
        vec.z = 0;

        return vec;
    }
}
