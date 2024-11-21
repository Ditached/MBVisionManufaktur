using DG.Tweening;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    
    public float speed = 10f;
    public Vector3 axis = Vector3.forward;
    public bool rotationEnabled = true;
    
    void Update()
    {
        if(!rotationEnabled) return;
        transform.Rotate(axis, speed * Time.deltaTime);
    }
    
    public void RotateToZero()
    {
        rotationEnabled = false;
        transform.DORotate(Vector3.zero, 2f).SetEase(Ease.OutSine);
    }
}