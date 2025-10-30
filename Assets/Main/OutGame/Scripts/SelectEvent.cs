using UnityEngine;
using UnityEngine.SceneManagement;

public class SelectEvent : MonoBehaviour
{
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
        SceneManager.LoadScene("InGame");
    }

    public void StopTimer()
    {
        Debug.Log("StopTimer");
        TimerPresenter.Instance.PauseMainGameTimer();
    }
}
