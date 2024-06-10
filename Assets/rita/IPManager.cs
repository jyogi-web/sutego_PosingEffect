using UnityEngine;
using System.Net;
using System.Net.Sockets;

public class IPManager : MonoBehaviour
{
    public string localIP;
    void Start()
    {
        localIP = GetLocalIPAddress();
        Debug.Log("Local IP Address: " + localIP);
    }

    string GetLocalIPAddress()
    {
        var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                return ip.ToString();
            }
        }
        throw new System.Exception("Local IP Address Not Found!");
    }
}
