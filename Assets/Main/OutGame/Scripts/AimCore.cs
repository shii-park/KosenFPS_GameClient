using System;
using R3;
using UnityEngine;

public class AimCore : MonoBehaviour
{
    public static AimCore Instance { get; private set; }


    [SerializeField]
    private CrosshairMover _crosshairMover;
    
    public RectTransform AimTransform;
    public ReadOnlyReactiveProperty<bool> IsShot => _crosshairMover.ShotReactiveProperty;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
