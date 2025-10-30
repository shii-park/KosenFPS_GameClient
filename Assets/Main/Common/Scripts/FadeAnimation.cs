using DG.Tweening;
using R3;
using UnityEngine;
using UnityEngine.UI;

public class FadeAnimation : MonoBehaviour
{
    [SerializeField]
    private Image _image;
    
    private ReactiveProperty<bool> _isCompleteFadein = new ReactiveProperty<bool>(false);
    public ReactiveProperty<bool> IsCompleteFadein => _isCompleteFadein;
    
    private ReactiveProperty<bool> _isCompleteFadeout = new ReactiveProperty<bool>(false);
    public ReactiveProperty<bool> IsCompleteFadeout => _isCompleteFadeout;
    
    void Start()
    {
        _isCompleteFadein.Value = false;
        _isCompleteFadeout.Value  = false;
        FadeIn();
    }

    public void FadeIn()
    {
        _isCompleteFadein.Value = false;
        _image.DOFade(0, 1).OnComplete(() => _isCompleteFadein.Value = true);
    }

    public void FadeOut()
    {
        _isCompleteFadeout.Value = false;
        _image.DOFade(1, 3).OnComplete(() => _isCompleteFadeout.Value = true);
    }
}
