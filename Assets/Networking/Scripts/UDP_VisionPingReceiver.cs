using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using UnityEngine;

public class UDP_VisionPingReceiver : MonoBehaviour
{
    public UDP_Connector udpConnector;
    public VisionConnectionUI visionConnectionPrefab;
    public Transform parent;

    private List<VisionConnection> _visionConnectionsData = new();

    private void Awake()
    {
        udpConnector.OnBytesReceivedWithEndPoint.AddListener(OnMessageReceived);
    }

    private void OnMessageReceived(byte[] msg, IPEndPoint endpoint)
    {
        try
        {
            var package = UpdatePackage.FromBytes(msg);

            if (package.msgType == MsgType.Pong)
            {
                Debug.Log($"Received Pong from {endpoint.Address}");
                foreach (var visionIP in _visionConnectionsData)
                {
                    if (visionIP.ip == endpoint.Address.ToString())
                    {
                        visionIP.lastPing = DateTime.Now;
                        visionIP.buildNumber = package.buildNumber;
                        
                        return;
                    }
                }

                var connectionData = new VisionConnection
                {
                    ip = endpoint.Address.ToString(),
                    lastPing = DateTime.Now,
                    buildNumber = package.buildNumber
                };

                _visionConnectionsData.Add(connectionData);

                var visionConnectionUI_gameobject = Instantiate(visionConnectionPrefab, parent);
                visionConnectionUI_gameobject.visionConnection = connectionData;
                visionConnectionUI_gameobject.udpVisionPingReceiver = this;
                visionConnectionUI_gameobject.ip = endpoint.Address.ToString();
            } else if (package.msgType == MsgType.RequestChange)
            {
                UpdatePackage.globalAppState = package.appState;
                FindFirstObjectByType<UDP_MulticastSender>().SendPing();
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to parse update package: {e.Message}");
            return;
        }
    }

    public DateTime GetLastPing(string ip)
    {
        foreach (var visionIP in _visionConnectionsData)
        {
            if (visionIP.ip == ip)
            {
                return visionIP.lastPing;
            }
        }

        return DateTime.MinValue;
    }
}