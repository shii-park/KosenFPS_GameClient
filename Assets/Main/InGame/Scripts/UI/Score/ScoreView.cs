using UnityEngine;
using UnityEngine.UI;

public class ScoreView : MonoBehaviour
{
    [SerializeField]
    private Text _text;

    public void SetScore(int score)
    {
        if(_text == null) return;
        _text.text = score.ToString();
    }
}
