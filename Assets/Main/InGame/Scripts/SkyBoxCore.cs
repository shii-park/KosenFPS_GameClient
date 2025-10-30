using System.Collections;
using UnityEngine;

public class SkyBoxCore : MonoBehaviour
{
    [SerializeField,Range(0.01f,0.1f)]
    public float _rotateSpeed;

    [SerializeField]
    private Material _sky;

    float rotationRepeatValue;

    void Start()
    {
        StartCoroutine(MoveSkyBox());
    }

    IEnumerator MoveSkyBox()
    {
        while (true)
        {
            rotationRepeatValue = Mathf.Repeat(_sky.GetFloat("_Rotation") + _rotateSpeed , 360f);

            _sky.SetFloat("_Rotation",rotationRepeatValue);
            yield return new WaitForSeconds(0.1f);
        }
    }
}
