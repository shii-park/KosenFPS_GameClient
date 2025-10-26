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
        Debug.Log("シーンが変わるよ");
    }
}
