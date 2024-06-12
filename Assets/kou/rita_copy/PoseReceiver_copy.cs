using UnityEngine;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System;
using System.Collections.Generic;
using UnityEngine.UI;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;

//JSONの構造
/*
{
"0": [0.5728470683097839, 0.658988893032074, -1.3516395092010498], 
"1": [0.6165980100631714, 0.579612672328949, -1.3371552228927612], 
"2": [0.642914891242981, 0.5904691219329834, -1.3369053602218628], 
"3": [0.6680717468261719, 0.6010580658912659, -1.3368704319000244], 
"4": [0.5450809597969055, 0.5472546815872192, -1.3402270078659058], 
"5": [0.5158812403678894, 0.5362879633903503, -1.3390228748321533], 
"6": [0.4883791506290436, 0.5283602476119995, -1.3391835689544678], 
"7": [0.7001706957817078, 0.6139777302742004, -0.9582926630973816], 
"8": [0.4214741289615631, 0.5255637168884277, -0.9475886225700378], 
"9": [0.5982457399368286, 0.7541744112968445, -1.1843855381011963], 
"10": [0.5046695470809937, 0.7321945428848267, -1.1826176643371582], 
"11": [0.8231522440910339, 0.8946396708488464, -0.6195834875106812], 
"12": [0.2293272763490677, 0.882727861404419, -0.5662935972213745], 
"13": [0.9173325300216675, 1.3573248386383057, -0.3748088479042053], 
"14": [0.1479954570531845, 1.337624192237854, -0.2610817551612854], 
"15 : [0.8869015574455261, 1.7265560626983643, -0.48827657103538513], 
"16": [0.18917083740234375, 1.7135498523712158, -0.5074340105056763], 
"17": [0.9124473333358765, 1.8498746156692505, -0.5607846975326538], 
"18": [0.17318235337734222, 1.847868800163269, -0.5925077795982361], 
"19": [0.8741701245307922, 1.82888925075531, -0.6119747757911682], 
"20": [0.2105930596590042, 1.8117103576660156, -0.6588628888130188], 
"21": [0.846827507019043, 1.783263087272644, -0.5210943818092346], 
"22": [0.2343783676624298, 1.7611711025238037, -0.5465058088302612], 
"23": [0.6996433138847351, 1.7443820238113403, -0.07100453972816467], 
"24": [0.3316956162452698, 1.734089970588684, 0.0762101262807846], 
"25": [0.6682400107383728, 2.4772543907165527, -0.13404223322868347], 
"26": [0.336318701505661, 2.4713680744171143, 0.23658958077430725], 
"27": [0.6548304557800293, 3.155921697616577, 0.49448785185813904], 
"28": [0.3509563207626343, 3.121281147003174, 0.7942278981208801], 
"29": [0.6582980751991272, 3.258218288421631, 0.5153019428253174],
"30": [0.34676456451416016, 3.2134718894958496, 0.8337540626525879], 
"31": [0.6032864451408386, 3.390183448791504, -0.19453710317611694], 
"32": [0.4097873270511627, 3.358090877532959, 0.09582646191120148]
}
 */

[Serializable]
public class Vector3data
{
    public float x;
    public float y;
    public float z;
}

public class PoseReceiver_copy : MonoBehaviour
{
    IPManager_copy Ipmanager_copy;
    public string serverIP;
    public int serverPort = 50008;
    TcpClient client;
    NetworkStream stream;
    Thread receiveThread;
    string receivedData;
    public Vector3[] randmarkPosition;
    public Text text;

    void Start()
    {
        Ipmanager_copy = GetComponent<IPManager_copy>();
        serverIP = Ipmanager_copy.localIP;
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
            randmarkPosition = new Vector3[properties.Count];            int index = 0;
            foreach (var key in jObject.Properties())
            {
                var data = key.Value.ToObject<float[]>();
                if (data.Length == 3)
                {
                    randmarkPosition[index] = new Vector3(data[0], data[1], data[2]);
                    index++;
                }
                else
                {
                    Debug.LogError($"Invalid data length for key {key.Name}: expected 3 but got {data.Length}");
                }
            }
            Debug.Log("Received data: " + jsonData);
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



    void Update()
    {
        // 受け取ったデータを使用する処理をここに追加
        if (randmarkPosition != null && randmarkPosition.Length > 0)
        {
            text.text = randmarkPosition[0].x.ToString();
        }
        else
        {
            text.text = "No data received";
        }
    }

    void OnApplicationQuit()
    {
        receiveThread.Abort();
        client.Close();
    }
}