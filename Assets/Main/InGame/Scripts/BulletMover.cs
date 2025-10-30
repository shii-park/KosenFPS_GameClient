using R3;
using UnityEngine;

public class BulletMover : MonoBehaviour
{
    [SerializeField]
    private Camera mainCamera;      // メインカメラ
    [SerializeField]
    private RectTransform aimUI;    // 狙い用UI
    [SerializeField]
    private GameObject projectilePrefab;  // 発射するオブジェクト
    [SerializeField]
    private float shootForce = 10f;       // 発射速度

    private void Start()
    {
        aimUI = AimCore.Instance.AimTransform;

        AimCore.Instance.IsShot.Where(value => value).Subscribe(_ => Shoot());
    }

    public void Shoot()
    {
        // UIのスクリーン座標を取得
        Vector3 screenPoint = RectTransformUtility.WorldToScreenPoint(null, aimUI.position);

        // カメラから見た方向を取得
        Ray ray = mainCamera.ScreenPointToRay(screenPoint);

        // 飛ばす位置（カメラ前に生成）
        Vector3 spawnPos = mainCamera.transform.position + mainCamera.transform.forward * 1f;

        // 弾を生成
        GameObject projectile = Instantiate(projectilePrefab, spawnPos, Quaternion.identity);

        // Rigidbody に力を加える
        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.AddForce(ray.direction * shootForce, ForceMode.Impulse);
        }

        // 弾をカメラの向きに向ける
        projectile.transform.forward = ray.direction;
    }
}