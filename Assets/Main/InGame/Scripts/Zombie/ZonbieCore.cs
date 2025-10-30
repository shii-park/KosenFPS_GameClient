using System.Collections;
using R3;
using UnityEngine;

public class ZonbieCore : MonoBehaviour
{
    private ReactiveProperty<bool> _isDead = new ReactiveProperty<bool>();
    public ReactiveProperty<bool> IsDead => _isDead;
    
    [SerializeField]
    private ZonbieMover _zonbieMover;

    [SerializeField]
    private Animator _animator; // Animator をアサイン

    private static readonly int IdleHash = Animator.StringToHash("Zombie@Idle01");
    private static readonly int DeadHash = Animator.StringToHash("Zombie@Death01_A");

    [SerializeField]
    private Transform _first;
    [SerializeField]
    private Transform _end;
    private float _time = 2;

    public void Start()
    {
        ResetZombie(_first, _end, _time);
        StartCoroutine(TestCoroutine());
    }

    private IEnumerator TestCoroutine()
    {
        yield return new WaitForSeconds(3);
        Dead(new Vector3(1,1,1));
    }
    
    public void ResetZombie(Transform firstTransform ,Transform destination, float time)
    {
        _isDead.Value = false;
        this.transform.position = firstTransform.position;
        this.gameObject.SetActive(true);
        
        _animator.SetBool("Dead", false);
        
        _zonbieMover.StartMove(destination, time);
    }

    public void Dead(Vector3 powerVector)
    {
        _isDead.Value = true;

        // 水平方向のみを考慮（Y軸回転）
        Vector3 flatDirection = new Vector3(powerVector.x, 0f, powerVector.z);

        if (flatDirection.sqrMagnitude > 0.0001f) // 0ベクトル回避
        {
            // Y軸回転だけにして向きを変更
            transform.rotation = Quaternion.LookRotation(flatDirection, Vector3.up);
        }
        
        _animator.SetBool("Dead", true);
        
        StartCoroutine(HideZombie());
    }

    private IEnumerator HideZombie()
    {
        yield return new WaitForSeconds(1f);
        this.gameObject.SetActive(false);
    }
}
