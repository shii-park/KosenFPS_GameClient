using UnityEngine;
using System.Collections;
using R3;

public class TimerPresenter : MonoBehaviour
{
    [SerializeField] private TimerView _view;
    private TimerModel _model = new TimerModel();
    public TimerModel Model => _model;
    private Coroutine _timerCoroutine;
    
    private bool _isMainTimerRunning = false;
    private bool _isReadyTimerRunning = false;
    private bool _startGame;
    
    private float _countDownSeconds = 0.01f;
    
    public static TimerPresenter Instance { get; private set; }
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Init()
    {
        _model.MainGameTimer.Subscribe(value =>
        {
            _view.SetMainTimer(value);
        });


        _model.ReadyGameTimer.Subscribe(value =>
        {
            _view.SetReadyTimerText(value);
        });

        _startGame = false;

        StartReadyTimer();
    }

    public void StartMainGameTimer()
    {
        _startGame = true;
        StopMainGameTimer();
        _model.ResetMainGameTimer();
        _isMainTimerRunning = true;
        StartCoroutine(MainTimerRoutine());
    }

    public void StopMainGameTimer()
    {
        _isMainTimerRunning = false;
        _model.StopMainGameTimer();
    }

    public void ResumeMainGameTimer()
    {
        _model.ResumeMainGameTimer();
        _isMainTimerRunning = true;
        StartCoroutine(MainTimerRoutine());
    }

    public void PauseMainGameTimer()
    {
        StopMainGameTimer();
        StartCoroutine(Pause());
    }

    private IEnumerator Pause()
    {
        yield return new WaitForSeconds(3f);
        StartReadyTimer();
    }

    private IEnumerator MainTimerRoutine()
    {
        while (_model.MainGameTimer.Value > 0 && _isMainTimerRunning)
        {
            _model.UpdateMainGameTimer(_countDownSeconds);
            yield return new WaitForSeconds(_countDownSeconds);
        }

        _isMainTimerRunning = false;
    }
    
    public void StartReadyTimer()
    {
        StopReadyTimer();
        _model.ResetReadyGameTimer();
        _isReadyTimerRunning = true;
        _view.DisplayCountDownUI(true);
        
        _model.ReadyGameTimer
            .Skip(1).Where(value => value <= 0).Take(1)
            .Subscribe(value =>
            {
                if (_startGame == false) StartMainGameTimer();
                else ResumeMainGameTimer();
            });
        
        StartCoroutine(ReadyTimerRoutine());
    }

    public void StopReadyTimer()
    {
        _isReadyTimerRunning = false;
        _model.StopReadyGameTimer();
    }

    public void ResumeReadyTimer()
    {
        _model.ResumeReadyGameTimer();
        _isReadyTimerRunning = true;
        StartCoroutine(ReadyTimerRoutine());
    }

    private IEnumerator ReadyTimerRoutine()
    {
        while (_model.ReadyGameTimer.Value > 0 && _isReadyTimerRunning)
        {
            _model.UpdateReadyGameTimer(_countDownSeconds);
            yield return new WaitForSeconds(_countDownSeconds);
        }
        
        _view.DisplayCountDownUI(false);

        _isReadyTimerRunning = false;
    }
}