using System.Collections;
using UnityEngine;

public class ZonbieMover : MonoBehaviour
{ // 移動にかかる時間
    private Coroutine moveCoroutine;

    public void StartMove(Transform destination, float time)
    {
        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
        }

        moveCoroutine = StartCoroutine(MoveToTarget(destination.position, time));
    }

    public void StopMove()
    {
        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
            moveCoroutine = null;
        }
    }

    // コルーチンで移動
    private IEnumerator MoveToTarget(Vector3 destination, float time)
    {
        Vector3 start = transform.position;
        float elapsed = 0f;

        while (elapsed < time)
        {
            transform.position = Vector3.Lerp(start, destination, elapsed / time);
            elapsed += Time.deltaTime;
            yield return null; // 1フレーム待機
        }

        transform.position = destination;
        moveCoroutine = null; // コルーチン終了
    }
}
