using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class TimerWidget : MonoBehaviour
{
    private Slider ProgressTimer;
    private float maxTime;
    private float currentTime;
    private Action TimeOutCallback;
    public bool StopTime;
    
    public void SetTime(int time, Action timeOutCallback)
    {
        ProgressTimer = GetComponent<Slider>();
        
        maxTime = time;
        
        currentTime = time;
        
        TimeOutCallback = timeOutCallback;
    }

    public void StartCountDown()
    {
        StopTime = false;
        
        StartCoroutine(CorCountDown());
    }

    public float GetRemainTime()
    {
        return currentTime;
    }

    IEnumerator CorCountDown()
    {
        while (!StopTime)
        {
            yield return new WaitForSeconds(0.1f);

            currentTime -= 0.1f;

            StopTime = currentTime == 0;

            ProgressTimer.maxValue = maxTime;

            ProgressTimer.value = currentTime;
        }

        TimeOutCallback();
    }
}
