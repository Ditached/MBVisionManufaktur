using System;
using UnityEngine;

public class Sphere : MonoBehaviour
{
    public float speed;
    public Transform target;
    
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        var direction = (target.position - transform.position).normalized;
        rb.linearVelocity = direction * speed;
    }

}
