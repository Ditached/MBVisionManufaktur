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

    [Title("Send Message")]
    public string targetIP = "192.168.1.255";
    public ushort targetPort = 6455;

    [HideInInspector] public UnityEvent<string> OnMessageReceived;
    [HideInInspector] public UnityEvent<string, IPEndPoint> OnMessageReceivedWithEndPoint;
    [HideInInspector] public UnityEvent<byte[], IPEndPoint> OnBytesReceivedWithEndPoint;
    
    void Awake()
    {
        udpClient = new UdpClient(port); // Your port number here
        udpClient.Client.ReceiveTimeout = 100; // Add small timeout so we don't block Update
        udpClient.EnableBroadcast = true;
        
        Debug.Log($"UDP Listener started on port {port}");
    }

    public void SendUdpMsg(string message)
    {
        try
        {
            byte[] data = Encoding.UTF8.GetBytes(message);
            udpClient.Send(data, data.Length, targetIP, targetPort);
            Debug.Log($"Sent: {message}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error sending message: {e.Message}");
        }
    }
    
    public void SendGlowMessage(string macAdress, bool glow)
    {
        int glowInt = glow ? 1 : 0;
        var msg = "{\"WDMODE\":\"" + $"{glowInt}\",\"MACS\":[\"{macAdress}\"]" + "}";
        
        SendUdpMsg(msg);
    }

    void Update()
    {
        try
        {
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
        }
    }

    void OnDestroy()
    {
        if (udpClient != null)
        {
            udpClient.Close();
        }
    }
}