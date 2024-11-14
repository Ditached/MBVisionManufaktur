using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Events;

public struct SetupMesssage
{
    public string mac;
}

public struct EventMessage
{
    public string mac;
}

public class MicrochipConnector : MonoBehaviour
{
    public UDP_Connector udpConnector;

    public string macAdress = "FF:FF:FF:FF:FF:FF";
    public bool isAvailable;

    [HideInInspector] public UnityEvent OnTriggered;

    private void Awake()
    {
        udpConnector.OnMessageReceived.AddListener(OnMessageReceived);
    }

    private void OnMessageReceived(string msg)
    {
        try
        {
            if (msg.Contains("setup"))
            {
                var json = JsonConvert.DeserializeObject<SetupMesssage>(msg);

                if (json.mac == macAdress)
                {
                    isAvailable = true;
                }
            }
            else if (msg.Contains("event"))
            {
                var json = JsonConvert.DeserializeObject<EventMessage>(msg);

                if (json.mac == macAdress)
                {
                    Debug.Log($"Sensor {json.mac} triggered");
                    OnTriggered.Invoke();
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }
}