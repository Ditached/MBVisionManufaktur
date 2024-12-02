using System;
using TMPro;
using UnityEngine;

public class RotationControlBtn : MonoBehaviour
{
    public TMP_Text label;

    private void Update()
    {
        label.text = UpdatePackage.globalRotationRunning ? "Stop Rotation" : "Start Rotation";
    }
    
    public void OnBtnPress()
    {
        FindFirstObjectByType<UDP_MulticastReceiver>().RequestRotationRunningChange(!UpdatePackage.globalRotationRunning);
    }
}
