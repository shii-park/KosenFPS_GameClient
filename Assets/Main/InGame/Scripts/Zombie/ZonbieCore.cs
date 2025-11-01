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
    private float _time = 2;

    public void Start()
    {
        ResetZombie(_first, _first, _time);
    }
    
    public void ResetZombie(Transform firstTransform, Transform destination, float time)
    {
        _isDead.Value = false;

        // 位置
        this.transform.position = firstTransform.position;

        // 初期向き（Y軸のみ）
        Vector3 flatForward = new Vector3(firstTransform.forward.x, 0f, firstTransform.forward.z);
        if (flatForward.sqrMagnitude > 0.0001f)
        {
            this.transform.rotation = Quaternion.LookRotation(flatForward, Vector3.up);
        }

        this.gameObject.SetActive(true);

        _animator.SetBool("Dead", false);
        _animator.Play(_animator.GetCurrentAnimatorStateInfo(0).shortNameHash, 0, Random.Range(0f, 1f));

        //_zonbieMover.StartMove(destination, time);
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
