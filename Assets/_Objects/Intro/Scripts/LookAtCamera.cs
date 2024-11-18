using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    public bool flip = false;

    void Update()
    {
        transform.LookAt(Camera.main.transform);
        
        if (flip)
        {
            transform.Rotate(0, 180, 0);
        }
    }
}