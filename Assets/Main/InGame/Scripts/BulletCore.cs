using UnityEngine;

public class BulletCore : MonoBehaviour
{
    private float destroyTime = 5f;  // 弾の自動消滅時間

    void Start()
    {
        Destroy(gameObject, destroyTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        // 衝突先に ZonbieCore があるかチェック
        ZonbieCore zombie = other.GetComponent<ZonbieCore>();
        if (zombie != null)
        {
            // 弾の進行方向だけ渡す（大きさは無視）
            Vector3 direction = -1*transform.forward.normalized;

            zombie.Dead(direction);
        }

        Destroy(gameObject); // 弾を消す
    }
}