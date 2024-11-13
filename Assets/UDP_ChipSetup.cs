using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

public class SetupMessage
{
    public static string GenerateMessage(int timeStart, int timeEnd)
    {
        return
            $"{{\"SETUP\":\"1\",\"MACS\":[\"FF:FF:FF:FF:FF:FF\"],\"LED\":\"1\",\"TIME\":[\"{timeStart}\",\"{timeEnd}\"],\"BAT\":[\"3.5\",\"4.2\"]}}";
    }
}

public class UDP_ChipSetup : MonoBehaviour
{
    public UDP_Connector udpConnector;
    [FormerlySerializedAs("chipsSetups")] public List<MicrochipConnector> chips;

    public float interval = 0.5f;
    public float intervalAfterSuccess = 20f;
    
    [Title("Setup Message")]
    public int timeStart = 100;
    public int timeEnd = 300;
    
    [Button("Send Setup Message")]
    public void SendSetupMessage()
    {
        udpConnector.SendUdpMsg(SetupMessage.GenerateMessage(timeStart, timeEnd));
    }

    private IEnumerator Start()
    {
        bool allChipsReady = false;
        
        chips = new List<MicrochipConnector>(
            FindObjectsByType<MicrochipConnector>(FindObjectsInactive.Include, FindObjectsSortMode.None));

        while (!allChipsReady)
        {
            SendSetupMessage();

            yield return new WaitForSeconds(interval);

            allChipsReady = true;

            foreach (var microchipConnector in chips)
            {
                allChipsReady &= microchipConnector.isAvailable;
            }
        }

        while (true)
        {
            SendSetupMessage();
            yield return new WaitForSeconds(intervalAfterSuccess);
        }
    }
}
