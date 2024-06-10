using System;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class LandmarkReceiver : MonoBehaviour
{
    private TcpClient client;
    private NetworkStream stream;
    private byte[] buffer = new byte[1024];
    public string serverIp = "127.0.0.1";
    public int serverPort = 65433;

    void Start()
    {
        ConnectToServer();
    }

    void ConnectToServer()
    {
        try
        {
            client = new TcpClient(serverIp, serverPort);
            stream = client.GetStream();
            Debug.Log("Connected to the server");
            BeginRead();
        }
        catch (SocketException socketEx)
        {
            Debug.LogError($"SocketException: {socketEx.Message}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Exception: {ex.Message}");
        }
    }

    void BeginRead()
    {
        if (stream != null && stream.CanRead)
        {
            stream.BeginRead(buffer, 0, buffer.Length, new AsyncCallback(OnRead), null);
        }
        else
        {
            Debug.LogError("Network stream is not readable.");
        }
    }

    void OnRead(IAsyncResult ar)
    {
        try
        {
            int bytesRead = stream.EndRead(ar);
            if (bytesRead > 0)
            {
                string receivedData = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                Debug.Log($"Received data: {receivedData}");
                // 再度読み込みを開始
                BeginRead();
            }
            else
            {
                Debug.LogError("Server closed the connection.");
                CloseConnection();
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"OnRead Exception: {ex.Message}");
            CloseConnection();
        }
    }

    void CloseConnection()
    {
        if (stream != null)
            stream.Close();
        if (client != null)
            client.Close();
    }

    void OnApplicationQuit()
    {
        CloseConnection();
    }
}
