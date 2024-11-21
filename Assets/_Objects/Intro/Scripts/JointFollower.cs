using System;
using Hand;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.XR.Hands;

namespace TinyFins.Hand
{
    public class JointFollower : MonoBehaviour
    {
        public Transform handOnPc;
        public bool teleportOnFirstFrame = false;
        public bool lerp = false;

        [ShowIf("lerp")] public float lerpSpeed = 8f;

        public Handedness handedness;
        [ReadOnly] public bool hasBeenInitialized = false;
        public HandManager handManager;
        public XRHandJointID jointID = XRHandJointID.BeginMarker;

        public bool onlyRotation = false;
        public bool onlyPosition = false;

        public MeshRenderer optionalColorShit;
        private bool teleported;

        private void OnEnable()
        {
            var handManagers = FindObjectsByType<HandManager>(FindObjectsSortMode.None);

            foreach (var handManager in handManagers)
            {
                if (handManager.handedness == handedness)
                {
                    this.handManager = handManager;
                    break;
                }
            }
        }

        void Update()
        {
            if(Application.isEditor && handOnPc != null)
            {
                transform.position = handOnPc.position;
                transform.rotation = handOnPc.rotation;
                return;
            }
            
            if (handManager.TryGetPoseForJoint(jointID, out var position, out var rotation))
            {
                hasBeenInitialized = true;

                if (onlyRotation)
                {
                    transform.rotation = rotation;
                    return;
                }

                if (teleportOnFirstFrame && !teleported)
                {
                    transform.SetPositionAndRotation(position, rotation);
                    teleported = true;
                }

                if (onlyPosition)
                {
                    rotation = transform.rotation;
                }

                if (lerp)
                    transform.SetPositionAndRotation(
                        Vector3.Lerp(transform.position, position, Time.deltaTime * lerpSpeed),
                        Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * lerpSpeed));
                else transform.SetPositionAndRotation(position, rotation);

                if (optionalColorShit != null)
                {
                    optionalColorShit.material.color = Color.green;
                }
            }
            else
            {
                if (optionalColorShit != null)
                {
                    optionalColorShit.material.color = Color.red;
                }
            }
        }
    }
}