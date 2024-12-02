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

    public float maxMaxSpeedOutsideRange = 20f;

    private void Update()
    {
        yTargetAngle = UpdatePackage.globalPlattformRotation;

        // Use DeltaAngle to get the shortest distance between angles
        var distance = Mathf.Abs(Mathf.DeltaAngle(target.localRotation.eulerAngles.y, yTargetAngle));
        var multiplier = Mathf.InverseLerp(minDistance, maxDistance, distance);
        var speed = Mathf.Lerp(minSpeedMultiplier, maxSpeedMultiplier, multiplier);
        
        if(distance > maxDistance)
        {
            multiplier = Mathf.InverseLerp(maxDistance, 180f, distance);
            speed = Mathf.Lerp(maxSpeedMultiplier, maxMaxSpeedOutsideRange, multiplier);
        }

        var currentRotEuler = target.localRotation.eulerAngles;
        currentRotEuler.y = Mathf.MoveTowardsAngle(currentRotEuler.y, yTargetAngle,
            UpdatePackage.globalPlattformSpeed * Time.deltaTime * speed);

        target.localRotation = Quaternion.Euler(currentRotEuler);
    }
}