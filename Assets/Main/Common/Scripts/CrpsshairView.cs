using UnityEngine;
using UnityEngine.UI;
using R3;

public class CrpsshairView : MonoBehaviour
{
    [SerializeField]
    private Image _gaugeImage;
    
    [SerializeField]
    private CrosshairMover _crosshairMover;

    void Start()
    {
        _crosshairMover.ReloadTimer.Subscribe(seconds =>
        {
            _gaugeImage.fillAmount = 1 - seconds / _crosshairMover.ReloadTime;
        });
    }
}
