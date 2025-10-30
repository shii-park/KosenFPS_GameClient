using R3;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SelectEvent : MonoBehaviour
{
    [SerializeField]
    FadeAnimation _fadeAnimation;
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
        _fadeAnimation.FadeOut();
        _fadeAnimation.IsCompleteFadeout.Where(value => value).Subscribe(_ => SceneManager.LoadScene("InGame"));
    }
    public void ChangeOutGameScene()
    {
        _fadeAnimation.FadeOut();
        _fadeAnimation.IsCompleteFadeout.Where(value => value).Subscribe(_ => SceneManager.LoadScene("OutGame"));
    }

    public void StopTimer()
    {
        Debug.Log("StopTimer");
        TimerPresenter.Instance.PauseMainGameTimer();
    }
}
