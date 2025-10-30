using UnityEngine;
using R3;

public class ScorePresenter : MonoBehaviour
{
    private ScoreModel _scoreModel = new ScoreModel();
    public ScoreModel ScoreModel => _scoreModel;
    
    [SerializeField]
    private ScoreView _scoreView;
    
    public static ScorePresenter Instance { get; private set; }

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
        _scoreModel.ResetScore();
        _scoreModel.Score.Subscribe(score =>
        {
            _scoreView.SetScore(score);
        });
    }

    public void AddScore(int score = 1)
    {
        _scoreModel.AddScore(score);
    }
    
    public void SubtractScore(int subtractScore = 1)
    {
        _scoreModel.SubtractScore(subtractScore);
    }

    public void Reset()
    {
        _scoreModel.ResetScore();
    }
}
