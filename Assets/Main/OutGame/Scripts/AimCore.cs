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
    
    public RectTransform AimTransform => this.gameObject.GetComponent<RectTransform>();
    private ReactiveProperty<bool> isShot = new ReactiveProperty<bool>(false);
}
