using UnityEngine;

public class AimCollider : MonoBehaviour
{
    //重なっているか判定したいRectTransform(Inspectorから設定)
    [SerializeField]
    private RectTransform _rectTransform, _targetRectTransform;
    
    private void Update() {
        //重なっているか判定
        if (_rectTransform.IsOverlapping(_targetRectTransform)) {
            Debug.Log($"重なっている");
        }
        else {
            Debug.Log($"重なっていない");
        }
    }
}
