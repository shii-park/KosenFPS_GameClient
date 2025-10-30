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

    public void SetMainTimer(float time)
    {
        if(_mainTimertext == null) return;
        _mainTimertext.text = time.ToString("F");
    }

    public void SetReadyTimerText(float time)
    {
        if (_readyTimerText == null) return;
        _readyTimerText.text = Mathf.Floor(time) == 0 ? "Go!!" : Mathf.Floor(time).ToString();
    }

    public void DisplayCountDownUI(bool show)
    {
        Debug.Log($"show:{show}");
        _countDownUI.gameObject.SetActive(show);
    }
}
