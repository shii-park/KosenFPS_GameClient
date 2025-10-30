using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

[RequireComponent(typeof(RectTransform))]
public class StableGyroAim_NoAudio_Flip : MonoBehaviour
{
    private enum AimState { Calibrating, Running, Failed }

    [Header("Dependencies")]
    public SerialReceive serialReceiver;

    [Header("Aim Settings")]
    public float sensitivity = 200f;
    [Tooltip("微小なノイズを無視する範囲")]
    public float deadZone = 0.1f;

    [Header("Axis Inversion")]
    [Tooltip("X軸を反転するか")]
    public bool invertX = true;
    [Tooltip("Y軸を反転するか")]
    public bool invertY = false;

    [Header("Calibration Settings")]
    public float calibrationTime = 3.0f;
    [Tooltip("キャリブレーション中のばらつきの許容値")]
    public float stabilityThreshold = 10f;

    [Header("Smoothing")]
    [Tooltip("小さいほど敏感、大きいほど滑らか")]
    public float smoothTime = 0.05f;

    private RectTransform rectTransform;
    private Vector2 currentAimPosition;
    private Vector2 currentAngularVelocity;
    private Vector2 smoothVelocity;

    private AimState currentState;

    // キャリブレーション用
    private float calibrationTimer;
    private Vector2 gyroOffset = Vector2.zero;
    private List<Vector2> calibrationSamples = new List<Vector2>();

    private Rect parentRect;
    private bool shot = false;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();

        if (serialReceiver == null)
        {
            Debug.LogError("SerialReceiverが設定されていません！");
            enabled = false;
            return;
        }

        RectTransform parentCanvas = transform.parent.GetComponent<RectTransform>();
        if (parentCanvas != null) parentRect = parentCanvas.rect;

        serialReceiver.serialHand.OnDataReceived += OnDataReceived;
        StartCalibration();
    }

    void OnDestroy()
    {
        if (serialReceiver != null)
            serialReceiver.serialHand.OnDataReceived -= OnDataReceived;
    }

    void OnDataReceived(string message)
    {
        try
        {
            string[] values = message.Split(',');
            if (values.Length < 6) return;

            if (!float.TryParse(values[4], out float gyroX)) return;
            if (!float.TryParse(values[5], out float gyroY)) return;
            Vector2 rawGyroInput = new Vector2(gyroX, gyroY);

            if (currentState == AimState.Calibrating)
            {
                calibrationSamples.Add(rawGyroInput);
                return;
            }

            if (currentState == AimState.Running)
            {
                Vector2 correctedGyro = rawGyroInput - gyroOffset;

                // デッドゾーン
                if (Mathf.Abs(correctedGyro.x) < deadZone) correctedGyro.x = 0;
                if (Mathf.Abs(correctedGyro.y) < deadZone) correctedGyro.y = 0;

                // 軸反転
                if (invertX) correctedGyro.x *= -1;
                if (invertY) correctedGyro.y *= -1;

                currentAngularVelocity = correctedGyro;

                if (values.Length > 6 && float.TryParse(values[6], out float shotVal))
                    shot = (shotVal == 1f);
            }
        }
        catch { /* 無視 */ }
    }

    void Update()
    {
        if (UnityEngine.InputSystem.Keyboard.current.rKey.wasPressedThisFrame)
        {
            StartCalibration();
            return;
        }

        if (currentState == AimState.Calibrating)
        {
            calibrationTimer -= Time.deltaTime;

            if (calibrationTimer <= 0f)
                FinishCalibration();
        }
        else if (currentState == AimState.Running)
        {
            Vector2 deltaPosition = currentAngularVelocity * sensitivity * Time.deltaTime;

            currentAimPosition = Vector2.SmoothDamp(rectTransform.anchoredPosition, currentAimPosition + deltaPosition, ref smoothVelocity, smoothTime);

            currentAimPosition.x = Mathf.Clamp(currentAimPosition.x, parentRect.xMin, parentRect.xMax);
            currentAimPosition.y = Mathf.Clamp(currentAimPosition.y, parentRect.yMin, parentRect.yMax);

            rectTransform.anchoredPosition = currentAimPosition;

            shot = false;
        }
    }

    public void StartCalibration()
    {
        currentState = AimState.Calibrating;
        calibrationTimer = calibrationTime;
        calibrationSamples.Clear();
        currentAngularVelocity = Vector2.zero;
        Debug.Log("Calibration started...");
    }

    private void FinishCalibration()
    {
        if (calibrationSamples.Count == 0)
        {
            FailCalibration("No data received during calibration.");
            return;
        }

        float avgX = calibrationSamples.Average(v => v.x);
        float avgY = calibrationSamples.Average(v => v.y);
        gyroOffset = new Vector2(avgX, avgY);

        float sumSq = calibrationSamples.Sum(v => (v - gyroOffset).sqrMagnitude);
        float stdDev = Mathf.Sqrt(sumSq / calibrationSamples.Count);

        Debug.LogWarning($"Calibration finished. Offset: {gyroOffset}, StdDev: {stdDev:F3}");

        if (stdDev > stabilityThreshold)
        {
            FailCalibration($"Device unstable (StdDev: {stdDev:F3} > Threshold: {stabilityThreshold:F3})");
        }
        else
        {
            SucceedCalibration();
        }
    }

    private void SucceedCalibration()
    {
        currentState = AimState.Running;
        Debug.Log("Calibration successful!");
        currentAimPosition = Vector2.zero;
    }

    private void FailCalibration(string reason)
    {
        currentState = AimState.Failed;
        Debug.LogError("Calibration FAILED! Reason: " + reason);
    }
}
