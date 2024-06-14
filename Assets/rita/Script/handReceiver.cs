using UnityEngine;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System;
using Newtonsoft.Json.Linq; // JSONデータのパースに使用

public class HandReceiver : MonoBehaviour
{
    IPManager Ipmanager;
    public string serverIP;
    public int serverPort = 50008;
    TcpClient client;
    NetworkStream stream;
    Thread receiveThread;
    string receivedData;

    void Start()
    {
        Ipmanager = GetComponent<IPManager>();
        serverIP = Ipmanager.localIP;
        try
        {
            Debug.Log("HandReceiver: TCP接続成功");
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
        // JSONデータをパース
        JObject json = JObject.Parse(jsonData);
        // ここで手のランドマークデータを処理
        // 例: json["hand_0_landmark_0"]["x"] でX座標を取得
        Debug.Log("Received hand data: " + jsonData);
    }

    void OnApplicationQuit()
    {
        receiveThread.Abort();
        client.Close();
    }
}
