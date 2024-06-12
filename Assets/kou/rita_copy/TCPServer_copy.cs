using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class TCPServer_copy : MonoBehaviour
{
    private TcpListener tcpListener;
    private TcpClient tcpClient;
    private NetworkStream networkStream;

    // JSON�f�[�^���i�[���邽�߂̃N���X
    [Serializable]
    public class Data
    {
        public string name;
        public string type;
        public float version;
    }

    void Start()
    {
        // �T�[�o�[�̃G���h�|�C���g��ݒ�
        IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Any, 50007);

        // TcpListener���쐬
        tcpListener = new TcpListener(localEndPoint);

        // �ڑ��̑҂��󂯂��J�n
        tcpListener.Start();

        Debug.Log("Waiting for a connection...");

        // �N���C�A���g����̐ڑ����󂯓����
        tcpClient = tcpListener.AcceptTcpClient();

        Debug.Log("Connected!");

        // �l�b�g���[�N�X�g���[�����擾
        networkStream = tcpClient.GetStream();

        // ���b�Z�[�W����M
        byte[] bytes = new byte[1024];
        int bytesRead = networkStream.Read(bytes, 0, bytes.Length);
        string jsonData = Encoding.UTF8.GetString(bytes, 0, bytesRead);

        // JSON�f�[�^���f�V���A���C�Y
        Data receivedData = JsonUtility.FromJson<Data>(jsonData);

        Debug.Log($"Received JSON: Name - {receivedData.name}, Type - {receivedData.type}, Version - {receivedData.version}");

        // �N���C�A���g�ɉ����𑗐M
        byte[] responseBytes = Encoding.UTF8.GetBytes("JSON received successfully!");
        networkStream.Write(responseBytes, 0, responseBytes.Length);

        // �ڑ������
        tcpClient.Close();
        tcpListener.Stop();
    }
}
