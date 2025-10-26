using System;
using R3;
using UnityEngine;

public class AimCore : MonoBehaviour
{
    public static AimCore Instance { get; private set; }


    [SerializeField]
    private AimInput _aimInput;
    
    public RectTransform AimTransform => this.gameObject.GetComponent<RectTransform>();
    public ReadOnlyReactiveProperty<bool> IsShot => _aimInput.OnShotButtonPushed;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        IsShot.Skip(1).Subscribe(_ =>
        {
            Debug.Log("Shot!!!!!!!!!!!");
        });
    }
}
