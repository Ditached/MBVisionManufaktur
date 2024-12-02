using System;
using UnityEngine;

public class RotationController : MonoBehaviour
{
    public float baseRotation = 0f;
    public bool running;
    public bool direction;
    public float speed;
    public float speedClient;

    private void Update()
    {
        running = UpdatePackage.globalRotationRunning;
        
        if(!running) return;

        if (UpdatePackage.globalAppState == AppState.Waiting)
        {
            UpdatePackage.globalPlattformRotation = baseRotation;
        }
        else
        {
            var currentRot = UpdatePackage.globalPlattformRotation + (direction ? speed * Time.deltaTime : -speed * Time.deltaTime);
            UpdatePackage.globalPlattformRotation = Mathf.Repeat(currentRot, 360);
        }

        UpdatePackage.globalPlattformSpeed = speedClient;
    }
}
