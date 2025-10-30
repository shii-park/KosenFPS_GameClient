using R3;
using UnityEngine;

public class ScoreModel
{
    public ScoreModel(int initialScore = 0)
    {
        _score.Value = initialScore;
    }
    
    private ReactiveProperty<int> _score = new ReactiveProperty<int>(0);
    public ReactiveProperty<int> Score => _score;

    public void AddScore(int addScore = 1)
    {
        _score.Value += addScore;
    }

    public void SubtractScore(int subtractScore = 1)
    {
        _score.Value -= subtractScore;
    }

    public void ResetScore()
    {
        _score.Value = 0;
    }
}
