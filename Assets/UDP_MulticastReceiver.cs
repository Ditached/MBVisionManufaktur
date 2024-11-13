using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using System.Collections.Concurrent;

public class UDP_MulticastReceiver : MonoBehaviour
{
    public string multicastAddress = "239.255.255.252";
    public int port = 62111;
    public TMP_Text optionalDebugText;

    public UnityEvent<string> OnMessageReceived = new UnityEvent<string>();

    private UdpClient client;
    private bool isInitialized = false;
    private bool isRunning = false;
    private Thread receiveThread;
    private DateTime lastReceived;
    
    // Thread-safe queue for messages
    private ConcurrentQueue<(string message, string logMessage)> messageQueue = new ConcurrentQueue<(string, string)>();

    private void Awake()
    {
        try
        {
            client = new UdpClient();
            client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            client.Client.Bind(new IPEndPoint(IPAddress.Any, port));
            client.JoinMulticastGroup(IPAddress.Parse(multicastAddress));

            isInitialized = true;
            isRunning = true;
            Debug.Log($"[MULTICAST RECEIVER] Multicast receiver initialized on {multicastAddress}:{port}");

            // Start receive thread
            receiveThread = new Thread(ReceiveThread);
            receiveThread.IsBackground = true;
            receiveThread.Start();
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to initialize multicast receiver: {e.Message}");
        }
    }

    private void ReceiveThread()
    {
        IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);

        while (isRunning && client != null)
        {
            try
            {
                byte[] data = client.Receive(ref remoteEndPoint);
                string message = Encoding.UTF8.GetString(data);
                
                var time = DateTime.Now.ToString("HH:mm:ss.fff");
                var diff = DateTime.Now - lastReceived;
                lastReceived = DateTime.Now;

                var logMessage = $"[{time}] Received from {remoteEndPoint}: {message}. Diff: {diff.TotalMilliseconds}ms";
                
                // Queue the message for processing in Update
                messageQueue.Enqueue((message, logMessage));
            }
            catch (SocketException ex)
            {
                // Ignore expected exceptions when closing
                if (isRunning && ex.SocketErrorCode != SocketError.Interrupted)
                {
                    Debug.LogError($"Socket error in receive thread: {ex.Message}");
                }
            }
            catch (Exception e)
            {
                if (isRunning)
                {
                    Debug.LogError($"Error in receive thread: {e.Message}");
                }
            }
        }
    }

    private void Update()
    {
        // Process any queued messages
        while (messageQueue.TryDequeue(out var messageData))
        {
            string message = messageData.message;
            string logMessage = messageData.logMessage;
            
            Debug.Log(logMessage);
            if (optionalDebugText != null) optionalDebugText.text = logMessage;
            OnMessageReceived.Invoke(message);
        }
    }

    private void OnDestroy()
    {
        // Signal thread to stop
        isRunning = false;

        if (client != null)
        {
            try
            {
                // Close the client to interrupt any blocking Receive call
                client.DropMulticastGroup(IPAddress.Parse(multicastAddress));
                client.Close();
                
                // Wait for thread to finish (with timeout)
                if (receiveThread != null && receiveThread.IsAlive)
                {
                    receiveThread.Join(1000);
                    if (receiveThread.IsAlive)
                    {
                        receiveThread.Abort();
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Error cleaning up multicast receiver: {e.Message}");
            }
        }
    }
}