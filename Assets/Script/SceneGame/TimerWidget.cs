using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class TimerWidget : MonoBehaviour
{
    [SerializeField]
    private Text _remainTimeText = null;
    private Slider _progressTimer = null;

    private float _maxTime;
    private float _currentTime;
    private Action _timeOutCallback;
    private State _state = State.Stop;

    private enum State {
        Stop,
        Start,
        End,
    }

    public void Initialize(int time, Action timeOutCallback)
    {
        _currentTime = time;
        _maxTime = time;

        _timeOutCallback = timeOutCallback;

        _progressTimer = GetComponent<Slider>();
        _progressTimer.maxValue = _maxTime;

        Debug.Assert(_remainTimeText != null);
        _remainTimeText.text = CreateRemainTimeStr(_maxTime);
    }

    public void Start()
    {
        Resume();
    }

    public void Resume()
    {
        if (_state == State.End) { return; }
        _state = State.Start;
    }

    public void Stop()
    {
        if (_state == State.End) { return; }
        _state = State.Stop;
    }

    public bool IsTimeOut()
    {
        return _state == State.End;
    }

    public float GetRemainTime()
    {
        return _currentTime;
    }

    public void FixedUpdate()
    {
        if (_state == State.Start) {
            _currentTime -= Time.fixedDeltaTime;
            _currentTime = Math.Max(_currentTime, 0.0f);

            if (_currentTime <= 0.0f) {
                _state = State.End;
            }

            _remainTimeText.text = CreateRemainTimeStr(_currentTime);

            _progressTimer.value = _currentTime;
        }

        if (_state == State.End) {
            _timeOutCallback();
            // ※ SliderのValueが0になっても､ゲージがわずかに残り続けてしまうので､非Activeにしている.
            _progressTimer.gameObject.SetActive(false); 
        }
    }

    private string CreateRemainTimeStr(float time)
    {
        return time.ToString("00.00").Replace(".", ":");
    }
}
