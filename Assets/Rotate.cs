using UnityEngine;

public class Rotate : MonoBehaviour
{
    public float speed = 10f;
    public Vector3 axis = Vector3.forward;
    
    void Update()
    {
        transform.Rotate(axis, speed * Time.deltaTime);
    }
}
