using UnityEngine;
using System.Net;
using System.Net.Sockets;

public class IPManager_copy : MonoBehaviour
{
    public static string localIP_copy;
    void Start()
    {
        localIP_copy = GetLocalIPAddress();
        Debug.Log("Local IP Address: " + localIP_copy);
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
