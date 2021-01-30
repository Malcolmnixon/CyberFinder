using System;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public struct TimeAction
{
    public float start;
    public UnityEvent actions;
}

public class TimeTrigger : MonoBehaviour
{
    public bool autoStart;
    public bool loop;
    public float period;
    public TimeAction[] actions;

    private bool _running;
    private double _currentTime;

    public void StartTimer()
    {
        _running = true;
    }

    public void PauseTimer()
    {
        _running = false;
    }

    public void StopTimer()
    {
        _running = false;
        _currentTime = 0;
    }

    private void Start()
    {
        _running = autoStart;
    }

    private void Update()
    {
        // Skip if not running
        if (!_running)
            return;

        // Update time
        var before = _currentTime;
        _currentTime += Time.deltaTime;

        // Trigger actions as time passes
        foreach (var action in actions)
            if (before <= action.start && _currentTime > action.start)
                action.actions?.Invoke();

        // Loop time
        if (loop && _currentTime >= period)
            _currentTime = 0;
    }
}
