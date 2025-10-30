using System.Collections;
using UnityEngine;

public class ScoreTest : MonoBehaviour
{
    void Start()
    {
        ScorePresenter.Instance.Init();
        StartCoroutine(Test());
    }
    
    IEnumerator Test()
    {
        while (true)
        {
            ScorePresenter.Instance.AddScore();
            yield return new WaitForSeconds(1);
        }
    }
}
