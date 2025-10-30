using System;
using System.Collections.Generic;
using R3;
using UnityEngine;

public enum GameState
{
    Ready,
    Playing,
    Result
}

public class GameProgresser : MonoBehaviour
{
    [SerializeField]
    private FadeAnimation _fadeAnimation;
    
    private TimerPresenter _timer;
    
    private ScorePresenter  _scorePresenter;
    
    [SerializeField]
    private List<ZonbieCore> _zonbieCores;
    
    public static GameProgresser  Instance { get; private set; }
    
    private ReactiveProperty<GameState> _gameState = new ReactiveProperty<GameState>(GameState.Ready);
    public ReactiveProperty<GameState> CurrentGameState => _gameState; 
    
    void Awake()
    {
        _gameState.Value = GameState.Ready;
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        _timer = TimerPresenter.Instance;
        _scorePresenter = ScorePresenter.Instance;
        
        _fadeAnimation.IsCompleteFadein.Where(value => value).Subscribe(_ =>
        {
            _timer.Init();
            _scorePresenter.Init();

            foreach (var zonbieCore in _zonbieCores)
            {
                zonbieCore.IsDead.Where(value => value).Subscribe(_ => _scorePresenter.AddScore());
            }
        });
        
        _timer.Model.ReadyGameTimer.Where(value => value <= 0).Skip(1).Subscribe(_ =>
        {
            _gameState.Value = GameState.Playing;
        });

        _scorePresenter.ScoreModel.Score.Where(value => value >= _zonbieCores.Count).Subscribe(_ =>
        {
            Debug.Log($"value:{_}");
            _gameState.Value = GameState.Result;
        });

        _timer.Model.MainGameTimer.Where(value => value <= 0).Subscribe(_ =>
        {
            Debug.Log("Game Over");
            _gameState.Value = GameState.Result;
        });
    }
}
