using System;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

public class ChipState : MonoBehaviour
{
    public bool notifyClientsOnChange;
    public UDP_MulticastSender udpMulticastSender;
    
    public LackConfigCollection lackConfigCollection;
    public bool updateGlobalState;

    public ushort chipState;
    public string chipStateString;
    public TMP_Text chipStateText;
    public TMP_Text lackNameText;

    public void SetSensor(int sensorIndex, bool state)
    {
        var prevState = GetSensor(sensorIndex);
        
        if (state)
            chipState |= (ushort) (1 << sensorIndex);
        else
            chipState &= (ushort) ~(1 << sensorIndex);
        
        //Has the state changed?
        if (prevState != state)
        {
            if (notifyClientsOnChange && udpMulticastSender != null)
            {
                udpMulticastSender.SendChipState();
            }
        }
    }

    public int GetFirstActiveSensor()
    {
        for (int i = 0; i < 16; i++)
        {
            if (GetSensor(i))
            {
                return i;
            }
        }

        return -1;
    }

    public bool IsAnySensorActive()
    {
        return chipState != 0;
    }

    public int[] GetActiveSensors()
    {
        int[] activeSensors = new int[16];
        int activeSensorsCount = 0;

        for (int i = 0; i < 16; i++)
        {
            if (GetSensor(i))
            {
                activeSensors[activeSensorsCount] = i;
                activeSensorsCount++;
            }
        }

        Array.Resize(ref activeSensors, activeSensorsCount);
        return activeSensors;
    }

    public bool AreMultipleSensorsActive()
    {
        return chipState != 0 && (chipState & (chipState - 1)) != 0;
    }

    public void SetOnlyThisSensor(int sensorIndex)
    {
        chipState = (ushort) (1 << sensorIndex);
    }

    public bool GetSensor(int sensorIndex)
    {
        return (chipState & (1 << sensorIndex)) != 0;
    }

    public void ToggleSensor(int sensorIndex)
    {
        chipState ^= (ushort) (1 << sensorIndex);
    }

    public string GetBinaryString()
    {
        return Convert.ToString(chipState, 2).PadLeft(16, '0');
    }


    private void Update()
    {
        UpdatePackage.globalChipState = chipState;

        chipStateString = GetBinaryString();
        chipStateText.text = chipStateString;

        if (lackNameText != null && lackConfigCollection != null)
        {
            if (AreMultipleSensorsActive())
            {
                lackNameText.text = "Multiple sensors active";
            }
            else
            {
                var index = GetFirstActiveSensor();
                if (index == -1)
                    lackNameText.text = "No sensors active";
                else
                    lackNameText.text = lackConfigCollection.lackConfigs[index].name;
            }
        }

        if (Input.GetKeyDown(KeyCode.F1))
        {
            UpdatePackage.globalAppState = AppState.Waiting;
        }

        if (Input.GetKeyDown(KeyCode.F2))
        {
            UpdatePackage.globalAppState = AppState.Running;
        }
    }

    [Title("Toggle Sensor Controls")]
    [Button]
    public void ToggleSensor0() => ToggleSensor(0);

    [Button]
    public void ToggleSensor1() => ToggleSensor(1);

    [Button]
    public void ToggleSensor2() => ToggleSensor(2);

    [Button]
    public void ToggleSensor3() => ToggleSensor(3);

    [Button]
    public void ToggleSensor4() => ToggleSensor(4);

    [Button]
    public void ToggleSensor5() => ToggleSensor(5);

    [Button]
    public void ToggleSensor6() => ToggleSensor(6);

    [Button]
    public void ToggleSensor7() => ToggleSensor(7);

    [Button]
    public void ToggleSensor8() => ToggleSensor(8);

    [Button]
    public void ToggleSensor9() => ToggleSensor(9);

    [Button]
    public void ToggleSensor10() => ToggleSensor(10);

    [Button]
    public void ToggleSensor11() => ToggleSensor(11);

    [Button]
    public void ToggleSensor12() => ToggleSensor(12);

    [Button]
    public void ToggleSensor13() => ToggleSensor(13);

    [Button]
    public void ToggleSensor14() => ToggleSensor(14);

    [Button]
    public void ToggleSensor15() => ToggleSensor(15);

    public void SetToWaiting() => UpdatePackage.globalAppState = AppState.Waiting;
    public void SetToRunning() => UpdatePackage.globalAppState = AppState.Running;
}