using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

//*****************************************************************************
//* The reference to Newtonsoft.JSON here requires special handling in order
//* to function correctly when building for Unity IL2CPP. Using Nuget to import
//* Newtonsoft.JSON does NOT work, it will fail at runtime due to reflection.
//* 1) In Unity, import the 'JSON.NET for Unity' asset.
//* 2) If the solution refers to other projects which themselves refer to
//*    Newtonsoft.JSON, then they must be also refer to 'JSON.NET for Unity'.
//* 3) To do that, simply refer them to refer to:
//*    "..\Assets\JsonDotNet\Assemblies\Windows\Newtonsoft.Json.dll"
//*****************************************************************************

class TransportUdp
{
    private System.Net.Sockets.Socket sock;
    private System.Net.IPAddress ipaddr;
    private System.Net.IPEndPoint endpoint;

    //*********************************************************************
    /// <summary>
    /// Create a UDP broadcast socket
    /// </summary>
    /// <param name="address">use "192.168.1.255" for broadcast</param>
    /// <param name="port">any value you want, just make sure its not 
    /// already in use locally</param>
    //*********************************************************************

    public TransportUdp(string address, int port)
    {
        sock = new System.Net.Sockets.Socket(
            System.Net.Sockets.AddressFamily.InterNetwork,
            System.Net.Sockets.SocketType.Dgram,
            System.Net.Sockets.ProtocolType.Udp)
        { ExclusiveAddressUse = false, EnableBroadcast = true };

        ipaddr = System.Net.IPAddress.Parse(address);
        endpoint = new System.Net.IPEndPoint(ipaddr, port);
    }

    //*********************************************************************
    /// <summary>
    /// Send bytes over the socket
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    //*********************************************************************

    public async Task<bool> Send(System.Byte[] message)
    {
        var bytesSent = sock.SendTo(message, endpoint);
        return true;
    }
}

