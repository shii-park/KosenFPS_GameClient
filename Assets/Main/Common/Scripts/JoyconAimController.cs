using System.Collections.Generic;
using UnityEngine;

public class JoyconAimController : MonoBehaviour
{
    private List<Joycon> joycons;
    private Joycon joycon;

    public Transform firePoint;
    public GameObject bulletPrefab;
    public RectTransform crosshairUI;
    public Camera mainCamera;
    private Quaternion joyconOffset = Quaternion.identity;

    void Start()
    {
        joycons = JoyconManager.Instance.j;
        if (joycons.Count < 1) return;
        joycon = joycons[0];
        if (mainCamera == null) mainCamera = Camera.main;
        joyconOffset = Quaternion.Inverse(joycon.GetVector());
    }

    void Update()
    {
        if (joycon == null) return;

        Quaternion rawOrientation = joycon.GetVector();
        Quaternion axisFix = Quaternion.Euler(90, 0, 0);
        Quaternion orientation = axisFix * (rawOrientation * joyconOffset);
        Vector3 direction = orientation * Vector3.forward;

        // --- UI位置更新 ---
        if (crosshairUI != null && mainCamera != null)
        {
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

        // --- 発射 ---
        if (joycon.GetButtonDown(Joycon.Button.SHOULDER_2))
        {
            FireBullet(direction);
        }

        // --- キャリブレーション（HOMEボタン）---
        if (joycon.GetButtonDown(Joycon.Button.HOME))
        {
            joyconOffset = Quaternion.Inverse(rawOrientation);
        }
    }

    void FireBullet(Vector3 direction)
    {
        if (bulletPrefab == null || firePoint == null) return;

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        rb.linearVelocity = direction.normalized * 25f;
    }
}
