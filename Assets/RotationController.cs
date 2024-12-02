using System;
using UnityEngine;

public class RotationController : MonoBehaviour
{
    public bool running;
    public bool direction;
    public float speed;
    public float speedClient;

    private void Update()
    {
        if(!running) return;
        
        var currentRot = UpdatePackage.globalPlattformRotation + (direction ? speed * Time.deltaTime : -speed * Time.deltaTime);
        UpdatePackage.globalPlattformRotation = Mathf.Repeat(currentRot, 360);

        UpdatePackage.globalPlattformSpeed = speedClient;
    }
}
