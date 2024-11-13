using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class UDP_MulticastSender : MonoBehaviour
{
    public bool multicastLoopback = true;
    public int port = 62111;  // Using a safer high port number
    public string multicastAddress = "239.255.255.252";
    
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
            
            InvokeRepeating(nameof(SendPing), 0, 0.5f);
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to initialize multicast sender: {e.Message}");
        }
    }
    
    public void SendPing()
    {
        SendUDPMsg($"Ping [{count++}]");
    }

    public void SendUDPMsg(string message)
    {
        try
        {
            var data = Encoding.UTF8.GetBytes(message);
            client.Send(data, data.Length, multicastEndPoint);
            Debug.Log($"[MULTICAST SENDER] Sent multicast: {message}");
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to send message: {e.Message}");
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
