using UnityEngine;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;
using UnityEngine.UI;

[Serializable]
public class Vector3data_rita
{
    public float x;
    public float y;
    public float z;
}

public class PoseReceiver : MonoBehaviour
{
    IPManager Ipmanager;
    public string serverIP;
    public int serverPort = 50008;
    TcpClient client;
    NetworkStream stream;
    Thread receiveThread;
    string receivedData;
    public static Vector3[] landmarkPosition;   //ランドマークVector3配列

    void Start()
    {
        landmarkPosition = new Vector3[32];
        Ipmanager = GetComponent<IPManager>();
        serverIP = Ipmanager.localIP;
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
        // JSONデータをVector3の配列に変換
        try
        {
            var jObject = JObject.Parse(jsonData);
            var properties = jObject.Properties().ToList();
            landmarkPosition = new Vector3[properties.Count];
            int index = 0;
            foreach (var key in jObject.Properties())
            {
                var data = key.Value.ToObject<float[]>();
                if (data.Length == 3)
                {
                    landmarkPosition[index] = new Vector3(data[0], data[1], data[2]);
                    index++;
                }
                else
                {
                    Debug.LogError($"Invalid data length for key {key.Name}: expected 3 but got {data.Length}");
                }
            }
            Debug.Log("Received data: " + jsonData);
/*            Debug.Log(landmarkPosition.Length);
            for (int i = 0; i < landmarkPosition.Length; i++)
            {
                Debug.Log("currentpos at index " + i + ": " + landmarkPosition[i]);
            }*/
        }
        catch (JsonException e)
        {
            Debug.LogError($"JSON parse error: {e.Message}");
        }
        catch (ArgumentNullException e)
        {
            Debug.LogError($"Argument null error: {e.Message}");
        }
        catch (FormatException e)
        {
            Debug.LogError($"Format error: {e.Message}");
        }
        catch (Exception e)
        {
            Debug.LogError($"Unexpected error: {e.Message}");
        }
    }

    void OnApplicationQuit()
    {
        receiveThread.Abort();
        client.Close();
    }
}