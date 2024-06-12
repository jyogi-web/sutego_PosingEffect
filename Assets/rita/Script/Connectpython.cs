using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using UnityEngine;

public class Connectpython : MonoBehaviour
{
    // Start is called before the first frame update

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ConnectPython()
    {
        // エンドポイントを設定する
        IPEndPoint RemoteEP = new IPEndPoint(IPAddress.Any, 50007);

        // TcpListenerを作成する
        TcpListener Listener = new TcpListener(RemoteEP);

        // TCP接続を待ち受ける
        Listener.Start();
        TcpClient Client = Listener.AcceptTcpClient();

        // 接続ができれば、データをやり取りするストリームを保存する
        NetworkStream Stream = Client.GetStream();

        GetPID(Stream);

        // 接続を切る
        Client.Close();
    }


    void GetPID(NetworkStream Stream)
    {
        Byte[] data = new Byte[256];
        String responseData = String.Empty;

        // 接続先からデータを読み込む
        Int32 bytes = Stream.Read(data, 0, data.Length);

        // 読み込んだデータを文字列に変換する
        responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
        UnityEngine.Debug.Log("PID: " + responseData);

        // 受け取った文字列に文字を付け足して戻す
        Byte[] buffer = System.Text.Encoding.ASCII.GetBytes("responce: " + responseData);
        Stream.Write(buffer, 0, buffer.Length);
    }
}
