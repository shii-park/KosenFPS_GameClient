using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using R3;

[RequireComponent(typeof(RectTransform))]
public class CrosshairMover : MonoBehaviour
{
    private enum AimState { Calibrating, Running, Failed }

    [Header("Dependencies")]
    public SerialReceive serialReceiver;

    [Header("Axis Sensitivity")]
    [Tooltip("左右(X)方向の感度")]
    public float sensitivityX = 220f; 
    [Tooltip("上下(Y)方向の感度")]
    public float sensitivityY = 200f; 

    [Header("Aim Settings")]
    [Tooltip("微小なノイズを無視する範囲")]
    public float deadZone = 0.1f;

    [Header("Axis Inversion")]
    public bool invertX = true;
    public bool invertY = false;

    [Header("Calibration Settings")]
    public float calibrationTime = 3.0f;
    public float stabilityThreshold = 10f;

    [Header("Smoothing")]
    public float smoothTime = 0.05f;

    private RectTransform rectTransform;
    private Vector2 currentAimPosition;
    private Vector2 smoothVelocity;

    private AimState currentState;

    private float calibrationTimer;
    private Vector2 gyroOffset = Vector2.zero;
    private List<Vector2> calibrationSamples = new List<Vector2>();

    private Rect parentRect;

    private ReactiveProperty<bool> _shot = new ReactiveProperty<bool>();
    public ReactiveProperty<bool> ShotReactiveProperty => _shot;
    
    private bool _canShot = true;

    [SerializeField] public float ReloadTime = 1f;
    
    private ReactiveProperty<float> _reloadTimer = new ReactiveProperty<float>();
    public ReactiveProperty<float> ReloadTimer => _reloadTimer;

    // -----------------------------
    // バッファ用（Windowsビルド用）
    private Vector2 gyroBuffer = Vector2.zero;
    private int bufferCount = 0;
    // -----------------------------

    void Start()
    {
        _shot.Value = false;
        _reloadTimer.Value = ReloadTime;
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
        
        _shot.Skip(1).Where(value => value).Subscribe(value => StartInterval());
    }

    void StartInterval()
    {
        _canShot = false;

        StartCoroutine(ShotCoroutine());
    }

    private IEnumerator ShotCoroutine()
    {
        _reloadTimer.Value = 0;

        while (_reloadTimer.Value < ReloadTime)
        {
            _reloadTimer.Value += 0.01f;
            yield return new WaitForSeconds(0.01f);
        }
        
        _reloadTimer.Value = ReloadTime; 
        
        _canShot = true;
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

                if (Mathf.Abs(correctedGyro.x) < deadZone) correctedGyro.x = 0;
                if (Mathf.Abs(correctedGyro.y) < deadZone) correctedGyro.y = 0;

                if (invertX) correctedGyro.x *= -1;
                if (invertY) correctedGyro.y *= -1;

#if UNITY_EDITOR
                // Editorではバッファ化せずそのまま
                gyroBuffer = correctedGyro;
                bufferCount = 1;
#else
                // Windowsビルドではバッファに追加
                gyroBuffer += correctedGyro;
                bufferCount++;
#endif

                _shot.Value = (values.Length > 6 && values[6] == "1" && _canShot);
            }
        }
        catch { /* 無視 */ }
    }

    void Update()
    {
        if (UnityEngine.InputSystem.Keyboard.current.rKey.wasPressedThisFrame)
        {
            rectTransform.anchoredPosition = Vector2.zero;
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
            Vector2 currentAngularVelocity = Vector2.zero;

#if UNITY_EDITOR
            // Editorではそのまま
            if (bufferCount > 0)
            {
                currentAngularVelocity = gyroBuffer;
                gyroBuffer = Vector2.zero;
                bufferCount = 0;
            }

            Vector2 deltaPosition = currentAngularVelocity * (sensitivityX + 1000) * Time.deltaTime;
            deltaPosition.y = currentAngularVelocity.y * (sensitivityY + 1000) * Time.deltaTime;

#else
            // Windowsビルドのみフレーム補正＆バッファ平均化
            if (bufferCount > 0)
            {
                currentAngularVelocity = gyroBuffer / bufferCount;
                gyroBuffer = Vector2.zero;
                bufferCount = 0;
            }

            float deltaTimeFactor = Time.deltaTime / 0.016f; // 60fps基準
            Vector2 deltaPosition = new Vector2(
                currentAngularVelocity.x * sensitivityX * deltaTimeFactor,
                currentAngularVelocity.y * sensitivityY * deltaTimeFactor
            );
#endif

            currentAimPosition = Vector2.SmoothDamp(rectTransform.anchoredPosition, currentAimPosition + deltaPosition, ref smoothVelocity, smoothTime);

            currentAimPosition.x = Mathf.Clamp(currentAimPosition.x, parentRect.xMin, parentRect.xMax);
            currentAimPosition.y = Mathf.Clamp(currentAimPosition.y, parentRect.yMin, parentRect.yMax);

            rectTransform.anchoredPosition = currentAimPosition;

            _shot.Value = false;
        }
    }

    public void StartCalibration()
    {
        currentState = AimState.Calibrating;
        calibrationTimer = calibrationTime;
        calibrationSamples.Clear();
        gyroBuffer = Vector2.zero;
        bufferCount = 0;
        Debug.Log("Calibration started...");
    }

    private void FinishCalibration()
    {
        if (calibrationSamples.Count == 0)
        {
            FailCalibration("No data received during calibration.");
            return;
        }

        Vector2 avg = new Vector2(calibrationSamples.Average(v => v.x), calibrationSamples.Average(v => v.y));
        gyroOffset = avg;

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
