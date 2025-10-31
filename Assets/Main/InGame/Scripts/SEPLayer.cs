using R3;
using UnityEngine;

public class SEPLayer : MonoBehaviour
{
    [SerializeField] 
    private AudioClip shotSE;
    [SerializeField] 
    private AudioSource audioSource;
    
    private void Start()
    {
        AimCore.Instance.IsShot.Where(value => value).Subscribe(value => audioSource.PlayOneShot(shotSE));
    }

    public void PlayShotSE()
    {
        audioSource.PlayOneShot(shotSE);
    }
}
