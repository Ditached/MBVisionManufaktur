using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class UDP_MulticastSender : MonoBehaviour
{
    public float pingInterval = 1f;
    public float chipStateInterval = 0.25f;
    
    public bool multicastLoopback = true;
    public int port = 62111; // Using a safer high port number
    public string multicastAddress = "239.255.255.252";
    
    public ChipState chipState;

    private UdpClient client;
    private IPEndPoint multicastEndPoint;
    private long count;

    private void Awake()
    {
        try
        {
            client = new UdpClient();
            client.Ttl = 2;
            client.MulticastLoopback = multicastLoopback;
            multicastEndPoint = new IPEndPoint(IPAddress.Parse(multicastAddress), port);
            client.JoinMulticastGroup(IPAddress.Parse(multicastAddress));

            Debug.Log($"[MULTICAST SENDER] Multicast sender initialized on {multicastAddress}:{port}");

            InvokeRepeating(nameof(SendPing), 0, pingInterval);
            //InvokeRepeating(nameof(SendChipState), 0, chipStateInterval);
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to initialize multicast sender: {e.Message}");
        }
    }

    public void SendPing()
    {
        var package = UpdatePackage.CreatePing();
        SendUpdatePackage(package);
    }
    
    public void SendChipState()
    {
        var package = UpdatePackage.CreateChipStatePackage();
        SendUpdatePackage(package);
    }

    public void SendUpdatePackage(UpdatePackage updatePackage)
    {
        try
        {
            var bytes = updatePackage.ToBytes();
            client.Send(bytes, bytes.Length, multicastEndPoint);
            Debug.Log($"[MULTICAST SENDER] Sent update package: {updatePackage.ToString()}");
        } catch (Exception e)
        {
            Debug.LogError($"Failed to send update package: {e.Message}");
        }
    }

    void OnDestroy()
    {
        if (client != null)
        {
            try
            {
                client.DropMulticastGroup(IPAddress.Parse(multicastAddress));
                client.Close();
            }
            catch (Exception e)
            {
                Debug.LogError($"Error cleaning up multicast sender: {e.Message}");
            }
        }
    }
}