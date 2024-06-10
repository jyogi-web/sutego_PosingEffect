using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;

public class PlayPython : MonoBehaviour
{
    public int port = 65432;

    TcpListener server;
    TcpClient client;

    void Start()
    {
        ConnectToServer();
    }

    void ConnectToServer()
    {
        IPAddress ipAddress = IPAddress.Parse("192.168.1.16"); // Pythonスクリプトが実行されているコンピューターのIPアドレスに設定
        server = new TcpListener(ipAddress, port);
        server.Start();
        Debug.Log("Waiting for Python connection...");

        client = server.AcceptTcpClient();
        Debug.Log("Connected to Python.");

        ReceiveData();
    }

    void ReceiveData()
    {
        NetworkStream stream = client.GetStream();

        byte[] buffer = new byte[1024];
        int bytesRead = stream.Read(buffer, 0, buffer.Length);
        string jsonData = Encoding.ASCII.GetString(buffer, 0, bytesRead);
        Debug.Log("Received data from Python: " + jsonData);

        // データを処理する...

        // 次のデータを受信する
        ReceiveData();
    }
}
