using System;
using UnityEngine;

public class AttachToAnchor : MonoBehaviour
{
    private void Awake()
    {
        MoveToAnchor();
    }

    private void Start()
    {
        MoveToAnchor();
    }

    private void OnEnable()
    {
        MoveToAnchor();
    }

    void Update()
    {
        MoveToAnchor();
    }

    void LateUpdate()
    {
        MoveToAnchor();
    }

    private void MoveToAnchor()
    {
        var anchor = FindFirstObjectByType<Anchor>();
        if (anchor == null)
        {
            transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
            return;
        }

        transform.SetPositionAndRotation(anchor.transform.position, anchor.transform.rotation);
    }
}