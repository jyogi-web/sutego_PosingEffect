using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class TCPServer : MonoBehaviour
{
    private TcpListener tcpListener;
    private TcpClient tcpClient;
    private NetworkStream networkStream;

    // JSONデータを格納するためのクラス
    [Serializable]
    public class Data
    {
        public string name;
        public string type;
        public float version;
    }

    void Start()
    {
        // サーバーのエンドポイントを設定
        IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Any, 50007);

        // TcpListenerを作成
        tcpListener = new TcpListener(localEndPoint);

        // 接続の待ち受けを開始
        tcpListener.Start();

        Debug.Log("Waiting for a connection...");

        // クライアントからの接続を受け入れる
        tcpClient = tcpListener.AcceptTcpClient();

        Debug.Log("Connected!");

        // ネットワークストリームを取得
        networkStream = tcpClient.GetStream();

        // メッセージを受信
        byte[] bytes = new byte[1024];
        int bytesRead = networkStream.Read(bytes, 0, bytes.Length);
        string jsonData = Encoding.UTF8.GetString(bytes, 0, bytesRead);

        // JSONデータをデシリアライズ
        Data receivedData = JsonUtility.FromJson<Data>(jsonData);

        Debug.Log($"Received JSON: Name - {receivedData.name}, Type - {receivedData.type}, Version - {receivedData.version}");

        // クライアントに応答を送信
        byte[] responseBytes = Encoding.UTF8.GetBytes("JSON received successfully!");
        networkStream.Write(responseBytes, 0, responseBytes.Length);

        // 接続を閉じる
        tcpClient.Close();
        tcpListener.Stop();
    }
}
