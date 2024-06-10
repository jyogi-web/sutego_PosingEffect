using UnityEngine;
using System.Net.Sockets;
using System.Text;
using System.Threading;

public class PoseReceiver : MonoBehaviour
{
    TcpClient client;
    NetworkStream stream;
    Thread receiveThread;
    string receivedData;

    void Start()
    {
        client = new TcpClient("192.168.1.4", 50007);
        stream = client.GetStream();
        receiveThread = new Thread(new ThreadStart(ReceiveData));
        receiveThread.IsBackground = true;
        receiveThread.Start();
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
        // JSONデータを処理するコードをここに追加します。
        Debug.Log("Received data: " + jsonData);
    }

    void Update()
    {
        // 必要に応じて受信データを使用するコードをここに追加します。
    }

    void OnApplicationQuit()
    {
        receiveThread.Abort();
        client.Close();
    }
}
