using System;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Sirenix.OdinInspector;
using UnityEngine.Events;


public class UDP_Connector : MonoBehaviour
{
    public ushort port = 6457;
    private UdpClient udpClient;

    [Title("Send Message")] public string targetIP = "192.168.1.255";
    public ushort targetPort = 6455;

    [HideInInspector] public UnityEvent<string> OnMessageReceived;
    [HideInInspector] public UnityEvent<string, IPEndPoint> OnMessageReceivedWithEndPoint;
    [HideInInspector] public UnityEvent<byte[], IPEndPoint> OnBytesReceivedWithEndPoint;

    void Awake()
    {
        Application.targetFrameRate = 120;
        StartUDPClient();
    }

    private void StartUDPClient()
    {
        try
        {
            udpClient = new UdpClient(port); // Your port number here
            udpClient.Client.ReceiveTimeout = 100; // Add small timeout so we don't block Update
            udpClient.EnableBroadcast = true;
            Debug.Log($"UDP Listener started on port {port}");
        }
        catch (SocketException e)
        {
            if (e.SocketErrorCode == SocketError.AddressAlreadyInUse ||
                e.SocketErrorCode == SocketError.AddressNotAvailable)
            {
                SendToast($"Port {port} is already in use", ToastType.Error);
            }
        }
    }

    public void SendUdpMsgTo(string message, string ipAdress)
    {
        try
        {
            byte[] data = Encoding.UTF8.GetBytes(message);
            udpClient.Send(data, data.Length, ipAdress, targetPort);
            Debug.Log($"[UNICAST] Sent: {message} to {ipAdress}");
        }
        catch (System.Exception e)
        {
            if (udpClient == null)
            {
                StartUDPClient();
            }

            Debug.LogError($"Error sending message: {e.Message}");
        }
    }

    public void SendUdpMsg(string message)
    {
        try
        {
            byte[] data = Encoding.UTF8.GetBytes(message);
            udpClient.Send(data, data.Length, targetIP, targetPort);
            Debug.Log($"[BROADCAST] Sent: {message}");
        }
        catch (System.Exception e)
        {
            if (udpClient == null)
            {
                StartUDPClient();
            }

            Debug.LogError($"Error sending message: {e.Message}");
        }
    }

    public void SendGlowMessage(string macAdress, string ipAdress, bool glow)
    {
        if (ipAdress == "255.255.255.255" || ipAdress == "")
        {
            Debug.LogWarning("Invalid IP address");
            return;
        }
        
        int glowInt = glow ? 1 : 0;
        var msg = "{\"WDMODE\":\"" + $"{glowInt}\",\"MACS\":[\"{macAdress}\"]" + "}";

        SendUdpMsgTo(msg, ipAdress);
    }

    void Update()
    {
        try
        {
            if (udpClient == null)
            {
                Debug.LogWarning("UDP Client unavailable");
                return;
            }

            // Check if data is available
            if (udpClient.Available > 0)
            {
                IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, 0);
                byte[] data = udpClient.Receive(ref endPoint);
                string message = Encoding.UTF8.GetString(data);
                Debug.Log($"[{Time.realtimeSinceStartup}] Received from {endPoint} - size {data.Length}: {message}");

                OnBytesReceivedWithEndPoint.Invoke(data, endPoint);
                OnMessageReceived.Invoke(message);
                OnMessageReceivedWithEndPoint.Invoke(message, endPoint);
            }
        }
        catch (SocketException ex)
        {
            // Timeout is normal, ignore it
            if (ex.SocketErrorCode != SocketError.TimedOut)
            {
                Debug.LogError($"Socket error: {ex}");
            }
            else
            {
                SendToast("UDP Error: " + ex.Message, ToastType.Error);
            }
        }
    }

    void OnDestroy()
    {
        if (udpClient != null)
        {
            udpClient.Close();
        }
    }

    private void SendToast(string message, ToastType toastType)
    {
        try
        {
            ToastManager.GetInstance().AddToast(message, toastType);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error sending toast: {e.Message}");
        }
    }
}