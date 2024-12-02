using System;
using UnityEngine;

public class SyncedRotator : MonoBehaviour
{
    public float yTargetAngle;
    public Transform target;

    public float minDistance = 0f;
    public float maxDistance = 10f;
    
    public float minSpeedMultiplier = 0.1f;
    public float maxSpeedMultiplier = 5f;

    private void Update()
    {
        yTargetAngle = UpdatePackage.globalPlattformRotation;
        
        var distance = Mathf.Abs(target.localRotation.eulerAngles.y - yTargetAngle);
        var multiplier = Mathf.InverseLerp(minDistance, maxDistance, distance);
        var speed = Mathf.Lerp(minSpeedMultiplier, maxSpeedMultiplier, multiplier);
        
        var currentRotEuler = target.localRotation.eulerAngles;
        currentRotEuler.y = Mathf.MoveTowardsAngle(currentRotEuler.y, yTargetAngle, UpdatePackage.globalPlattformSpeed * Time.deltaTime * speed);

        if (distance > maxDistance)
        {
            currentRotEuler.y = yTargetAngle;
            target.localRotation = Quaternion.Euler(currentRotEuler);
        }
        else
        {
            target.localRotation = Quaternion.Euler(currentRotEuler);
        }
    }
}
