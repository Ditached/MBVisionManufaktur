using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

public class ChipMessageGenerator
{
    public static string GenerateSetupMessage(int timeStart, int timeEnd)
    {
        return
            $"{{\"SETUP\":\"1\",\"MACS\":[\"FF:FF:FF:FF:FF:FF\"],\"LED\":\"0\",\"TIME\":[\"{timeStart}\",\"{timeEnd}\"],\"BAT\":[\"3.5\",\"4.2\"]}}";
    }
    
    public static string GeneratePingMessage()
    {
        return "{\"PING\":\"1\",\"MACS\":[\"FF:FF:FF:FF:FF:FF\"]}";
    }

    public static string GenerateStatusPingMessage()
    {
        return "{\"STATUS\":\"1\",\"MACS\":[\"FF:FF:FF:FF:FF:FF\"]}";
    }
}

public enum PingStrategy
{
    Broadcast,
    Unicast
}

public class UDP_ChipSetup : MonoBehaviour
{
    public UDP_Connector udpConnector;

    public PingStrategy pingStrategy = PingStrategy.Broadcast;
    public float interval = 0.5f;
    public float intervalAfterSuccess = 20f;
    public float pingInterval = 2f;
    public float statusPingInterval = 30f;
    
    [Title("Setup Message")]
    public int timeStart = 100;
    public int timeEnd = 300;
    
    [Title("Debug")]
    [ReadOnly] public List<MicrochipConnector> chips;
    
    [Button("Send Setup Message")]
    public void SendSetupMessage()
    {
        udpConnector.SendUdpMsg(ChipMessageGenerator.GenerateSetupMessage(timeStart, timeEnd));
    }

    public void SendStatusPing()
    {
        udpConnector.SendUdpMsg(ChipMessageGenerator.GenerateStatusPingMessage());
    }
    
    public void SendPingMessage()
    {
        if(pingStrategy == PingStrategy.Broadcast)
            udpConnector.SendUdpMsg(ChipMessageGenerator.GeneratePingMessage());
        
        else if (pingStrategy == PingStrategy.Unicast)
        {
            foreach (var microchipConnector in chips)
            {
                if(microchipConnector.isAvailable && microchipConnector.ipAdress != "255.255.255.255")
                udpConnector.SendUdpMsgTo(ChipMessageGenerator.GeneratePingMessage(), microchipConnector.ipAdress);
            }
        }
    }

    private void Awake()
    {
        StartCoroutine(PingCoroutine());
        StartCoroutine(StatusPingCoroutine());
    }
    
    private IEnumerator StatusPingCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            SendStatusPing();
            yield return new WaitForSeconds(statusPingInterval);
        }
    }
    
    private IEnumerator PingCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(pingInterval);
            SendPingMessage();
        }
    }
    
    private IEnumerator Start()
    {
        bool allChipsReady = false;
        
        yield return new WaitForSeconds(1f);
        
        chips = new List<MicrochipConnector>(
            FindObjectsByType<MicrochipConnector>(FindObjectsInactive.Include, FindObjectsSortMode.None));

        while (!allChipsReady)
        {
            SendSetupMessage();

            yield return new WaitForSeconds(interval);

            allChipsReady = true;

            foreach (var microchipConnector in chips)
            {
                if(microchipConnector.macAdress == "00:00:00:00:00:00") continue;
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
