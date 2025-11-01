using R3;
using UnityEngine;

public class ZombieMusic : MonoBehaviour
{
    [SerializeField] 
    private AudioClip _deadSE;
    [SerializeField] 
    private AudioSource _audioSource;
    [SerializeField]
    private ZonbieCore _zombieCore;
    private void Start()
    {
        _zombieCore.IsDead.Where(value => value).Subscribe(value => _audioSource.PlayOneShot(_deadSE));
    }
}
