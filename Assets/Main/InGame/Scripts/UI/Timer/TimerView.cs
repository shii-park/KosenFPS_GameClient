using UnityEngine;
using UnityEngine.UI;

public class TimerView : MonoBehaviour
{    
    [SerializeField]
    private Text _mainTimertext;
    
    [SerializeField]
    private GameObject _countDownUI;
    
    [SerializeField]
    private Text _readyTimerText;

    public void SetMainTimer(int time)
    {
        if(_mainTimertext == null) return;
        _mainTimertext.text = time.ToString();
    }

    public void SetReadyTimerText(int time)
    {
        if (_readyTimerText == null) return;
        _readyTimerText.text = time.ToString();
    }

    public void DisplayCountDownUI(bool show)
    {
        _countDownUI.gameObject.SetActive(show);
    }
}
