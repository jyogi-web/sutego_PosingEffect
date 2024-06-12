using System;
using System.IO;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.UI;

public class TCPImageReceiver_copy : MonoBehaviour
{
    IPManager_copy Ipmanager_copy;
    public RawImage displayImage;
    public string serverIP;
    public int serverPort = 50007;
    private TcpClient client;
    private NetworkStream stream;
    private byte[] imageBytes;

    void Start()
    {
        Ipmanager_copy = GetComponent<IPManager_copy>();
        serverIP = Ipmanager_copy.localIP;
        try
        {
            Debug.Log("ImageReceiver:TCPconnect");
            client = new TcpClient(serverIP, serverPort);
            stream = client.GetStream();
        }
        catch (Exception e)
        {
            Debug.LogError($"TCPerror: {e.Message}");
        }
    }

    void Update()
    {
        if (client != null && stream != null && stream.DataAvailable)
        {
            try
            {
                Debug.Log("�摜�f�[�^���T�[�o�[�����M��...");
                // �摜�f�[�^�̒�������M�i4�o�C�g�̒������j
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
                    Array.Reverse(lengthBytes); // �r�b�O�G���f�B�A���ɕϊ�
                }
                int length = BitConverter.ToInt32(lengthBytes, 0);

                // �摜�f�[�^����M
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

                // �o�C�g�z���Texture2D�ɕϊ����ĕ\��
                Texture2D tex = new Texture2D(2, 2);
                tex.LoadImage(imageBytes);
                displayImage.texture = tex;
            }
            catch (Exception e)
            {
                Debug.LogError($"�摜�f�[�^�̎�M���ɃG���[���������܂���: {e.Message}");
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
