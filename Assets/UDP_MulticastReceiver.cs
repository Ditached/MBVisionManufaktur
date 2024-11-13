using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using UnityEngine.Events;

public class UDP_MulticastReceiver : MonoBehaviour
{
    public string multicastAddress = "239.255.255.252";
    public int port = 62111;

    public UnityEvent<string> OnMessageReceived = new UnityEvent<string>();

    private UdpClient client;
    private bool isInitialized = false;
    private IPEndPoint remoteEndPoint;
    
    private DateTime lastReceived;

    private void Awake()
    {
        try
        {
            client = new UdpClient();
            client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            client.Client.Bind(new IPEndPoint(IPAddress.Any, port));
            client.JoinMulticastGroup(IPAddress.Parse(multicastAddress));
            remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);

            isInitialized = true;
            Debug.Log($"[MULTICAST RECEIVER] Multicast receiver initialized on {multicastAddress}:{port}");
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to initialize multicast receiver: {e.Message}");
        }
    }

    private void Update()
    {
        if (!isInitialized) return;

        try
        {
            while (client.Available > 0)
            {
                byte[] data = client.Receive(ref remoteEndPoint);
                string message = Encoding.UTF8.GetString(data);
                
                // var get time with ms
                var time = DateTime.Now.ToString("HH:mm:ss.fff");
                var diff = DateTime.Now - lastReceived;
                lastReceived = DateTime.Now;
                
                Debug.Log($"[${time}] Received from {remoteEndPoint}: {message}. Diff: {diff.TotalMilliseconds}ms");
                
                OnMessageReceived.Invoke(message);
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Error receiving message: {e.Message}");
        }
    }

    private void OnDestroy()
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
                Debug.LogError($"Error cleaning up multicast receiver: {e.Message}");
            }
        }
    }
}