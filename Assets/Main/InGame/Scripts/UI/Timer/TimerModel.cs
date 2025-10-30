using R3;
using UnityEngine;

public class TimerModel
{
    public const float DefaultGameSeconds = 60f;
    public const float DefaultReadySeconds = 3f;

    private readonly ReactiveProperty<float> _mainGameTimer = new(DefaultGameSeconds);
    public ReactiveProperty<float> MainGameTimer => _mainGameTimer;

    private readonly ReactiveProperty<float> _readyGameTimer = new(DefaultReadySeconds);
    public ReactiveProperty<float> ReadyGameTimer => _readyGameTimer;

    private float _currentGameSeconds = DefaultGameSeconds;

    public void ResetMainGameTimer()
    {
        _mainGameTimer.Value = DefaultGameSeconds;
    } 

    public void UpdateMainGameTimer(float delta)
    {
        _mainGameTimer.Value = Mathf.Max(0, _mainGameTimer.Value - delta);
    }

    public void StopMainGameTimer()
    {
        _currentGameSeconds = _mainGameTimer.Value;
    }

    public void ResumeMainGameTimer()
    {
        _mainGameTimer.Value = _currentGameSeconds;
    }

    public void ResetReadyGameTimer()
    {
        _readyGameTimer.Value = DefaultReadySeconds;
    }

    public void UpdateReadyGameTimer(float delta)
    {
        _readyGameTimer.Value = Mathf.Max(0, _readyGameTimer.Value - delta);
    }

    public void StopReadyGameTimer()
    {
        _readyGameTimer.Value = 0;
    }

    public void ResumeReadyGameTimer()
    {
        _readyGameTimer.Value = 0;
    }
}