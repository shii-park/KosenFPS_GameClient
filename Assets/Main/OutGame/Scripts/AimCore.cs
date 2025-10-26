using System;
using R3;
using UnityEngine;

public class AimCore : MonoBehaviour
{
    static private AimCore _aimCore;

    static public AimCore Aim
    {
        get
        {
            return _aimCore;
        }
    }

    [SerializeField]
    private AimInput _aimInput;
    
    public RectTransform AimTransform => this.gameObject.GetComponent<RectTransform>();
    public ReadOnlyReactiveProperty<bool> IsShot => _aimInput.OnShotButtonPushed;

    private void Start()
    {
        IsShot.Skip(1).Subscribe(_ =>
        {
            Debug.Log("Shot!!!!!!!!!!!");
        });
    }
}
