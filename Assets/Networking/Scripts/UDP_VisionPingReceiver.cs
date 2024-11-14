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

    private List<VisionConnection> _visionIPs = new();

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
                visionConnection.udpVisionPingReceiver = this;
                visionConnection.ip = endpoint.Address.ToString();
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