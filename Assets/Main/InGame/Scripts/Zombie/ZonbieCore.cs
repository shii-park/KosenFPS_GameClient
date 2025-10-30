using R3;
using UnityEngine;

public class ZonbieCore : MonoBehaviour
{
    private ReactiveProperty<bool> _isDead = new ReactiveProperty<bool>();
    public ReactiveProperty<bool> IsDead => _isDead;
}
