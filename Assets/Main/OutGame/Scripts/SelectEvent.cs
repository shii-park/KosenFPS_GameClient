using R3;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SelectEvent : MonoBehaviour
{
    [SerializeField]
    FadeAnimation _fadeAnimation;

    private bool _isChangeScene = false;
    public void Test()
    {
        Debug.Log("Test");
    }
    
    public void QuitApplication()
    {
        Debug.Log("ゲーム終了");
        Application.Quit();
    }

    public void ChangeInGameScene()
    {
        if (_isChangeScene == true) return;
        _isChangeScene = true;
        
        _fadeAnimation.FadeOut();
        _fadeAnimation.IsCompleteFadeout.Where(value => value).Subscribe(_ => SceneManager.LoadScene("InGame"));
    }
    public void ChangeOutGameScene()
    {
        if (_isChangeScene == true) return;
        _isChangeScene = true;
        
        _fadeAnimation.FadeOut();
        _fadeAnimation.IsCompleteFadeout.Where(value => value).Subscribe(_ => SceneManager.LoadScene("OutGame"));
    }

    public void StopTimer()
    {
        Debug.Log("StopTimer");
        TimerPresenter.Instance.PauseMainGameTimer();
    }
}
