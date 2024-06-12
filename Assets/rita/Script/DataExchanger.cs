using System.IO;
using UnityEngine;

public class DataExchanger : MonoBehaviour
{
    int count = 0;
    string sendDataPath;
    string receiveDataPath;

    void Start()
    {
        sendDataPath = "\"C:\\Users\\RICK\\Desktop\\MyFolder\\python\\connectUnity\\Unity2Python.csv\"";
        receiveDataPath = "\"C:\\Users\\RICK\\Desktop\\MyFolder\\python\\connectUnity\\Python2Unity.csv\"";
    }

    void FixedUpdate()
    {
        WriteData(count.ToString(), sendDataPath);
        ReadData(receiveDataPath);
        count++;
    }

    void WriteData(string data, string filePath)
    {
        StreamWriter sw = new StreamWriter(filePath, false);
        sw.WriteLine(data);
        sw.Close();
    }

    void ReadData(string filePath)
    {
        if (File.Exists(filePath))
        {
            string data = File.ReadAllText(filePath);
            Debug.Log(data);
        }
    }
}
