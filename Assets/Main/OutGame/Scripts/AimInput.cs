using R3;
using R3.Triggers;
using UnityEngine;
using UnityEngine.InputSystem;

public class AimInput : MonoBehaviour
{
    [SerializeField]
    private JoyconAimController  joyconAimController;
    private ReactiveProperty<bool> _onShotButtonPushed = new ReactiveProperty<bool>();
    
    public ReadOnlyReactiveProperty<bool> OnShotButtonPushed { get { return _onShotButtonPushed; } }
    
    void Start()
    {
        this.UpdateAsObservable()
            .Select(_ => joyconAimController.Joycon.GetButtonDown(Joycon.Button.SHOULDER_1) ||
                         joyconAimController.Joycon.GetButtonDown(Joycon.Button.SHOULDER_2))
            .DistinctUntilChanged()
            .Subscribe(x => _onShotButtonPushed.Value = x);
    }
}
