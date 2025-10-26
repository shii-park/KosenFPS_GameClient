using UnityEngine;
using UnityEngine.Events;

public class SelectButton : MonoBehaviour
{
    [SerializeField]
    private UnityEvent _event;
    
    private AimCore _aimCore => AimCore.Instance;
    
    void Update()
    {
        if (this.gameObject.GetComponent<RectTransform>().IsOverlapping(AimCore.Instance.AimTransform))
        {
            _event.Invoke();
        }
    }
}
