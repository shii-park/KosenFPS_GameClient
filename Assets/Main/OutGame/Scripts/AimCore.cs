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
}
