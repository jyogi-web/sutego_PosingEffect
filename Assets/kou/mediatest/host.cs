using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Net.Sockets;
using System.IO;

public class WebcamImageReceiver : MonoBehaviour
{
    public RawImage rawImage;
    public string serverIP = "192.168.1.16"; // Python サーバーの IP アドレスを設定
    public int serverPort = 65432;
    private Texture2D texture;

    IEnumerator Start()
    {
        texture = new Texture2D(640, 480); // 画像のサイズに合わせて適切なサイズを設定してください
        while (true)
        {
            yield return StartCoroutine(ReceiveImage());
        }
    }

    IEnumerator ReceiveImage()
    {
        TcpClient client = new TcpClient();
        yield return client.ConnectAsync(serverIP, serverPort);
        NetworkStream stream = client.GetStream();
        BinaryReader reader = new BinaryReader(stream);

        try
        {
            while (true)
            {
                // サイズ情報を受信する
                byte[] sizeBytes = new byte[4];
                int bytesRead = 0;
                while (bytesRead < sizeBytes.Length)
                {
                    int read = stream.Read(sizeBytes, bytesRead, sizeBytes.Length - bytesRead);
                    if (read == 0)
                    {
                        Debug.LogError("Size header is incomplete");
                        yield break;
                    }
                    bytesRead += read;
                }

                // 受信したサイズ情報を整数に変換する
                int size = BitConverter.ToInt32(sizeBytes, 0);
                if (size <= 0)
                {
                    Debug.LogError("Invalid image size received: " + size);
                    yield break;
                }

                // 画像データを受信する
                byte[] imageData = new byte[size];
                bytesRead = 0;
                while (bytesRead < size)
                {
                    int read = stream.Read(imageData, bytesRead, size - bytesRead);
                    if (read == 0)
                    {
                        Debug.LogError("Image data is incomplete");
                        yield break;
                    }
                    bytesRead += read;
                }

                // 画像データを Texture2D にロード
                texture.LoadImage(imageData);
                rawImage.texture = texture;
            }
        }
        catch (Exception e)
        {
            Debug.LogError("SocketException: " + e);
        }
        finally
        {
            stream.Close();
            client.Close();
        }

        yield return null;
    }
}
