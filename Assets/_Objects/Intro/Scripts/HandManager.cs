using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.XR.Hands;
using UnityEngine.XR.Management;

namespace Hand
{
    public class HandManager : MonoBehaviour
    {
        public static Action HandSystemIsActive;

        public Handedness handedness; //Left or Right
        XRHandSubsystem m_HandSubsystem;

        void Start()
        {
            GetHandSubsystem();
        }

        [Button]
        [ContextMenu("Get Hand Subsystem")]
        void GetHandSubsystem()
        {
            Debug.Log("Getting Hand Subsystem");
            
            var xrGeneralSettings = XRGeneralSettings.Instance;
            if (xrGeneralSettings == null)
            {
                Debug.LogError("XR general settings not set");
            }

            var manager = xrGeneralSettings.Manager;
            if (manager != null)
            {
                var loader = manager.activeLoader;
                if (loader != null)
                {
                    m_HandSubsystem = loader.GetLoadedSubsystem<XRHandSubsystem>();
                    if (!CheckHandSubsystem())
                    {
                        Debug.LogWarning("Hand Subsystem not found");
                        return;
                    }

                    Debug.Log("Hand Subsystem found");
                    m_HandSubsystem.Start();
                }
                else
                {
                    Debug.LogError("Loader not set");
                }
            }
            else
            {
                Debug.LogError("XR Manager not set");
            }
        }

        bool CheckHandSubsystem()
        {
            if (m_HandSubsystem == null)
            {
                // Debug.LogError("Could not find Hand Subsystem");
                //enabled = false;
                return false;
            }

            return true;
        }

        public bool TryGetPoseForJoint(XRHandJointID jointID, out Vector3 position, out Quaternion rotation)
        {
            position = Vector3.zero;
            rotation = Quaternion.identity;

            if (!CheckHandSubsystem())
            {
                return false;
            }

            var updateSuccessFlags = m_HandSubsystem.TryUpdateHands(XRHandSubsystem.UpdateType.Dynamic);

            if (updateSuccessFlags != XRHandSubsystem.UpdateSuccessFlags.None)
                HandSystemIsActive?.Invoke();

            var successflagHand = handedness == Handedness.Left
                ? XRHandSubsystem.UpdateSuccessFlags.LeftHandJoints
                : XRHandSubsystem.UpdateSuccessFlags.RightHandJoints;

            if (jointID == XRHandJointID.Wrist)
            {
                successflagHand = handedness == Handedness.Left
                    ? XRHandSubsystem.UpdateSuccessFlags.LeftHandRootPose
                    : XRHandSubsystem.UpdateSuccessFlags.RightHandRootPose;
            }

            if ((updateSuccessFlags & successflagHand) !=
                XRHandSubsystem.UpdateSuccessFlags.None)
            {
                var hand = handedness == Handedness.Left
                    ? m_HandSubsystem.leftHand
                    : m_HandSubsystem.rightHand;


                if (jointID == XRHandJointID.Wrist)
                {
                    position = hand.rootPose.position;
                    rotation = hand.rootPose.rotation;
                    return true;
                }

                var joint = hand.GetJoint(jointID);

                if (joint.TryGetPose(out var pose))
                {
                    position = pose.position;
                    rotation = pose.rotation;
                    return true;
                }
            }

            return false;
        }
    }
}