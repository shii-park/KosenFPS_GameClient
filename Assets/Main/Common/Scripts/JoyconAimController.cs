using System.Collections.Generic;
using UnityEngine;

public class JoyconAimController : MonoBehaviour
{
    private List<Joycon> joycons;
    private Joycon joycon;
    public Joycon Joycon { get { return joycon; } }

    [Header("発射関連")]
    public Transform firePoint;
    public GameObject bulletPrefab;

    [Header("クロスヘアUI")]
    public RectTransform crosshairUI;
    public Camera mainCamera;

    [Header("手振れ補正")]
    public float smoothing = 5f; // 値を大きくすると追従速く、小さくすると揺れ減少

    // 内部状態
    private Vector3 initialEuler;   // リセット時の基準姿勢
    private Vector3 smoothedEuler;  // 手振れ補正後の姿勢

    private Quaternion initialRotation;   // リセット時の基準姿勢
    private Quaternion smoothedRotation;  // 手振れ補正後

    void Start()
    {
        joycons = JoyconManager.Instance.j;
        if (joycons.Count < 1) return;
        joycon = joycons[0];
        if (mainCamera == null) mainCamera = Camera.main;

        Quaternion rawOrientation = joycon.GetVector();
        initialRotation = rawOrientation;
        smoothedRotation = rawOrientation;
    }

    void Update()
    {
        if (joycon == null) return;

        Quaternion rawOrientation = joycon.GetVector();

        // HOMEボタンでリセット
        if (joycon.GetButtonDown(Joycon.Button.HOME))
        {
            initialRotation = rawOrientation;
            smoothedRotation = rawOrientation;
        }

        // --- 相対Quaternion（初期姿勢を基準にする） ---
        Quaternion relativeRotation = Quaternion.Inverse(initialRotation) * rawOrientation;

        // --- 手振れ補正（Quaternion.Slerp） ---
        smoothedRotation = Quaternion.Slerp(smoothedRotation, relativeRotation, Time.deltaTime * smoothing);

        // --- 照準方向ベクトル ---
        Vector3 direction = (Quaternion.Euler(90, 0, 0) * smoothedRotation) * Vector3.forward;

        UpdateCrosshair(direction);

        if (joycon.GetButtonDown(Joycon.Button.SHOULDER_2))
        {
            FireBullet(direction);
        }
    }


    void UpdateCrosshair(Vector3 direction)
    {
        if (crosshairUI == null || mainCamera == null || firePoint == null) return;

        Vector3 targetWorldPos = firePoint.position + direction * 10f;
        Vector3 screenPos = mainCamera.WorldToScreenPoint(targetWorldPos);

        Canvas parentCanvas = crosshairUI.GetComponentInParent<Canvas>();
        if (parentCanvas != null && parentCanvas.renderMode != RenderMode.WorldSpace)
        {
            RectTransform canvasRect = parentCanvas.GetComponent<RectTransform>();
            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvasRect, screenPos, mainCamera, out localPoint);
            crosshairUI.localPosition = localPoint;
        }
        else
        {
            crosshairUI.position = screenPos;
        }
    }

    void FireBullet(Vector3 direction)
    {
        if (bulletPrefab == null || firePoint == null) return;

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = direction.normalized * 25f;
        }
    }
}
