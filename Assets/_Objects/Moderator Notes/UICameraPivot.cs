using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Playables;

public class UICameraPivot : MonoBehaviour
{
    public float maxAngle = 100f;
    public Transform minDistanceMeasureDevice;
    public float timeToRecenter = 1f;
    public Ease ease;
    public float maxDistance = 4f;
    public float minDistance = 0.5f;
    private bool tweenRunning = false;


    private IEnumerator Start()
    {
        yield return false;
        CenterOnCamera();
        yield return new WaitForSeconds(0.5f);
        CenterOnCamera();
    }

    private void Update()
    {
        if (tweenRunning) return;


        //If the angle between the camera and the pivot is too big, recenter
        var angle = Vector3.Angle(transform.forward, Camera.main.transform.forward);
        

        if (angle > maxAngle)
        {
            CenterOnCamera();
        }


        var distanceToCamera = Vector3.Distance(transform.position, Camera.main.transform.position);

        if (distanceToCamera > maxDistance)
        {
            CenterOnCamera();
        }

        var distanceToMinDistanceDevice =
            Vector3.Distance(Camera.main.transform.position, minDistanceMeasureDevice.position);

        if (distanceToMinDistanceDevice < minDistance)
        {
            CenterOnCamera();
        }
    }

    public void CenterOnCamera()
    {
        tweenRunning = true;

        transform.DOMove(Camera.main.transform.position, timeToRecenter).SetEase(ease);

        var targetRotation = Quaternion.Euler(0, Camera.main.transform.eulerAngles.y, 0);

        transform.DORotateQuaternion(targetRotation, timeToRecenter).SetEase(ease).OnComplete(() =>
        {
            tweenRunning = false;
        });
    }
}