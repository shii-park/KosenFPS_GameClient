using System;
using R3;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SelectButton : MonoBehaviour
{
    [SerializeField]
    private UnityEvent _event;
    
    [SerializeField]
    private Image _image;
    
    private AimCore _aimCore => AimCore.Instance;
    
    private ReactiveProperty<bool> _isSelected = new ReactiveProperty<bool>();
    
    void Start()
    {
        if(_aimCore == null)return;
        
        Observable.Interval(TimeSpan.FromSeconds(0.1)) // 0.5秒ごとにチェック
            .Select(_ => this.gameObject.GetComponent<RectTransform>().IsOverlapping(_aimCore.AimTransform)) // 任意の関数
            .DistinctUntilChanged()
            .Subscribe(isSelected =>
            {
                _isSelected.Value = isSelected;
            })
            .AddTo(this);

        _isSelected.Subscribe(isSelected =>
        {
            _image.enabled = isSelected;
        });
        
        _aimCore.IsShot.Where(_ => _isSelected.CurrentValue).Subscribe(_ =>
        {
            _event.Invoke();
        });
    }

}
