using UnityEngine;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System;

public class PoseReceiver_copy : MonoBehaviour
{
    IPManager_copy Ipmanager_copy;
    public string serverIP;
    public int serverPort =50008;
    TcpClient client;
    NetworkStream stream;
    Thread receiveThread;
    string receivedData;

    void Start()
    {
        Ipmanager_copy = GetComponent<IPManager_copy>();
        serverIP=Ipmanager_copy.localIP;
        try
        {
            Debug.Log("PoseReceiver:TCP接続成功");
            client = new TcpClient(serverIP, serverPort);
            stream = client.GetStream();
            receiveThread = new Thread(new ThreadStart(ReceiveData));
            receiveThread.IsBackground = true;
            receiveThread.Start();
        }
        catch (Exception e)
        {
            Debug.LogError($"TCP接続の開始中にエラーが発生しました: {e.Message}");
        }


    }

    void ReceiveData()
    {
        byte[] bytes = new byte[1024];
        while (client.Connected)
        {
            int length = stream.Read(bytes, 0, bytes.Length);
            if (length <= 0) continue;
            receivedData = Encoding.UTF8.GetString(bytes, 0, length);
            ProcessData(receivedData);
        }
    }

    void ProcessData(string jsonData)
    {
        // JSON�f�[�^����������R�[�h�������ɒǉ����܂��B
        Debug.Log("Received data: " + jsonData);
    }

    void Update()
    {
        // �K�v�ɉ����Ď�M�f�[�^���g�p����R�[�h�������ɒǉ����܂��B
    }

    void OnApplicationQuit()
    {
        receiveThread.Abort();
        client.Close();
    }
}
