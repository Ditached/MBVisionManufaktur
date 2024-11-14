using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using UnityEngine;

public class UDP_VisionConnector : MonoBehaviour
{
    public UDP_Connector udpConnector;
    public VisionConnectionUI visionConnectionPrefab;
    public Transform parent;

    private List<VisionConnection> _visionIPs = new();

    private void Awake()
    {
        udpConnector.OnMessageReceivedWithEndPoint.AddListener(OnMessageReceived);
    }

    private void OnMessageReceived(string msg, IPEndPoint endpoint)
    {
        if (msg.Contains("Pong"))
        {
            foreach (var visionIP in _visionIPs)
            {
                if (visionIP.ip == endpoint.Address.ToString())
                {
                    visionIP.lastPing = DateTime.Now;
                    return;
                }
            }

            _visionIPs.Add(new VisionConnection
            {
                ip = endpoint.Address.ToString(),
                lastPing = DateTime.Now
            });
            
            var visionConnection = Instantiate(visionConnectionPrefab, parent);
            visionConnection.udpVisionConnector = this;
            visionConnection.ip = endpoint.Address.ToString();
        }
    }

    public DateTime GetLastPing(string ip)
    {
        foreach (var visionIP in _visionIPs)
        {
            if (visionIP.ip == ip)
            {
                return visionIP.lastPing;
            }
        }

        return DateTime.MinValue;
    }
}