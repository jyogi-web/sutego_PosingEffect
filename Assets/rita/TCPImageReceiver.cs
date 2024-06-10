using System;
using System.IO;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.UI;

public class TCPImageReceiver : MonoBehaviour
{
    IPManager Ipmanager;
    public RawImage displayImage;
    public string serverIP;
    public int serverPort = 50007;
    private TcpClient client;
    private NetworkStream stream;
    private byte[] imageBytes;

    void Start()
    {
        Ipmanager = GetComponent<IPManager>();
        serverIP = Ipmanager.localIP;
        try
        {
            client = new TcpClient(serverIP, serverPort);
            stream = client.GetStream();
        }
        catch (Exception e)
        {
            Debug.LogError($"TCP接続の開始中にエラーが発生しました: {e.Message}");
        }
    }

    void Update()
    {
        if (client != null && stream != null && stream.DataAvailable)
        {
            try
            {
                // 画像データの長さを受信（4バイトの長さ情報）
                byte[] lengthBytes = new byte[4];
                int totalBytesRead = 0;
                while (totalBytesRead < 4)
                {
                    int bytesRead = stream.Read(lengthBytes, totalBytesRead, 4 - totalBytesRead);
                    if (bytesRead == 0)
                    {
                        throw new IOException("Socket closed unexpectedly");
                    }
                    totalBytesRead += bytesRead;
                }

                if (BitConverter.IsLittleEndian)
                {
                    Array.Reverse(lengthBytes); // ビッグエンディアンに変換
                }
                int length = BitConverter.ToInt32(lengthBytes, 0);

                // 画像データを受信
                imageBytes = new byte[length];
                totalBytesRead = 0;
                while (totalBytesRead < length)
                {
                    int bytesRead = stream.Read(imageBytes, totalBytesRead, length - totalBytesRead);
                    if (bytesRead == 0)
                    {
                        throw new IOException("Socket closed unexpectedly");
                    }
                    totalBytesRead += bytesRead;
                }

                // バイト配列をTexture2Dに変換して表示
                Texture2D tex = new Texture2D(2, 2);
                tex.LoadImage(imageBytes);
                displayImage.texture = tex;
            }
            catch (Exception e)
            {
                Debug.LogError($"画像データの受信中にエラーが発生しました: {e.Message}");
            }
        }
    }


    void OnApplicationQuit()
    {
        if (stream != null)
        {
            stream.Close();
        }
        if (client != null)
        {
            client.Close();
        }
    }
}
