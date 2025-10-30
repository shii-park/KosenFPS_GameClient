using UnityEngine;
using UnityEngine.UI;

public class ResultView : MonoBehaviour
{
    [SerializeField]
    private GameObject _resultUI;
    [SerializeField]
    private Text _scoreText;
    [SerializeField]
    private Text _timerText;
    
    public void SetActiveUI(bool active)
    {
        _resultUI.SetActive(active);
    }

    public void SetText(int score, float timer)
    {
        _scoreText.text = score.ToString() + " / 12";
        _timerText.text = timer.ToString("F");
    }
}
