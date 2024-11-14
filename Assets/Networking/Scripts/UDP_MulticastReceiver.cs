using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class UDP_MulticastReceiver : MonoBehaviour
{
    public ChipState chipState;
    public uint udpIdWindowSize = 50;
    
    public string multicastAddress = "239.255.255.252";
    public int port = 62111;
    public int serverPort = 6457;
    
    public TMP_Text optionalDebugText;
    
    public float timeToReconnect = 1f;

    public UnityEvent<UpdatePackage> OnMessageReceived = new ();

    private UdpClient client;
    private bool isInitialized = false;
    private IPEndPoint remoteEndPoint;
    
    private DateTime lastReceived;
    private uint lastMsgId;
    

    private void Awake()
    {
        CreateConnection();
    }

    private void Update()
    {
        if (!isInitialized) return;
        
        try
        {
            while (client.Available > 0)
            {
                byte[] data = client.Receive(ref remoteEndPoint);
                var msg = UpdatePackage.FromBytes(data);
                
                var time = DateTime.Now.ToString("HH:mm:ss.fff");
                var diff = DateTime.Now - lastReceived;
                lastReceived = DateTime.Now;

                var log = $"[${time}] Received from {remoteEndPoint}: {msg.ToString()}. Diff: {diff.TotalMilliseconds}ms";
                Debug.Log(log);

                if (msg.msgType == MsgType.Ping)
                {
                    var ip = remoteEndPoint.Address.ToString();
                    var response = UpdatePackage.CreatePong().ToBytes();
                    client.Send(response, response.Length, new IPEndPoint(IPAddress.Parse(ip), serverPort));
                } else if (msg.msgType == MsgType.Update)
                {
                    var currMsgID = msg.id;
                    var diffId = Math.Abs(currMsgID - lastMsgId);
                    
                    if(diffId < udpIdWindowSize && currMsgID <= lastMsgId)
                    {
                        Debug.LogWarning($"Received old message: {msg.ToString()}");
                        continue;
                    }
                    
                    lastMsgId = currMsgID;
                    chipState.chipState = msg.chipState;
                    Debug.Log($"Received update package: {msg.ToString()}");
                }
                    
                if (optionalDebugText != null) optionalDebugText.text = log;
                OnMessageReceived.Invoke(msg);
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Error receiving message: {e.Message}");
        }

        
        if (lastReceived.AddSeconds(timeToReconnect) < DateTime.Now)
        {
            lastReceived = DateTime.Now;
            CreateConnection();
        }
    }

    private void CreateConnection()
    {
        TryDestroyConnection();
        
        try
        {
            client = new UdpClient();
            client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            client.Client.Bind(new IPEndPoint(IPAddress.Any, port));
            client.JoinMulticastGroup(IPAddress.Parse(multicastAddress));
            remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);

            isInitialized = true;
            lastReceived = DateTime.Now;
            Debug.Log($"[MULTICAST RECEIVER] Multicast receiver initialized on {multicastAddress}:{port}");
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to initialize multicast receiver: {e.Message}");
        }
        
    }

    private void OnDestroy()
    {
        TryDestroyConnection();
    }

    private void TryDestroyConnection()
    {
        if (client != null)
        {
            try
            {
                client.DropMulticastGroup(IPAddress.Parse(multicastAddress));
                client.Close();
                isInitialized = false;
            }
            catch (Exception e)
            {
                Debug.LogError($"Error cleaning up multicast receiver: {e.Message}");
            }
        }
    }
}