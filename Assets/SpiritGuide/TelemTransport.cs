//*****************************************************************************
//* File: TelemTransport.cs
//* Author: Mark West (markbrianwest@hotmail.com, mark.west@microsoft.com)
//* Project: SpirtGuide (Microsoft Hackathon 2020)
//* Description: Class to send UDP bropadcast packets
//*****************************************************************************

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
//* 3) To do that, simply refer them to:
//*    "..\Assets\JsonDotNet\Assemblies\Windows\Newtonsoft.Json.dll"
//*****************************************************************************

//*****************************************************************************
//* Base class to add JSON serialization to inheriter
//*****************************************************************************

class Message
{
    //*********************************************************************
    /// <summary>
    /// https://docs.microsoft.com/en-us/dotnet/standard/serialization/system-text-json-how-to
    /// </summary>
    /// <returns></returns>
    //*********************************************************************
    public string Serialize()
    {
        var settings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
        //settings.Converters.Add(new StringEnumConverter(new CamelCaseNamingStrategy()));
        settings.Converters.Add(new StringEnumConverter(true));
        return JsonConvert.SerializeObject(this, settings);
    }

    //*********************************************************************
    /// <summary>
    /// https://docs.microsoft.com/en-us/dotnet/standard/serialization/system-text-json-how-to
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    //*********************************************************************
    public static SampleClass DeSerialize(string message)
    {
        var settings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            MissingMemberHandling = MissingMemberHandling.Ignore
        };
        settings.Converters.Add(new StringEnumConverter(true));

        SampleClass ret = null;

        try
        {
            ret = JsonConvert.DeserializeObject<SampleClass>(message, settings);
        }
        catch (System.Exception ex)
        {
            //T1.CLogger.LogThis("DeSerialize() " + message);
        }

        return ret;
    }
}

//*****************************************************************************
//* Sample class to demo serialization
//*****************************************************************************

class SampleClass : Message
{
    public string stringData = "Don't surround yourself with yourself, move on back two squares.";
    public int intData = 255;
}

//*****************************************************************************
//* Class to send UDP bropadcast packets
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
    /// https://docs.microsoft.com/en-us/dotnet/standard/serialization/system-text-json-how-to
    /// </summary>
    /// <returns></returns>
    //*********************************************************************
    public string Serialize()
    {
        var settings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
        //settings.Converters.Add(new StringEnumConverter(new CamelCaseNamingStrategy()));
        settings.Converters.Add(new StringEnumConverter(true));
        return JsonConvert.SerializeObject(this, settings);
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

    //*********************************************************************
    /// <summary>
    /// Send string over the socket
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    //*********************************************************************

    public async Task<bool> Send(string message)
    {
        var bytesSent = sock.SendTo(Encoding.ASCII.GetBytes(message), endpoint);
        return true;
    }

    //*********************************************************************
    /// <summary>
    /// Send message over the socket
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    //*********************************************************************

    public async Task<bool> Send(Message message)
    {
        var bytesSent = sock.SendTo(Encoding.ASCII.GetBytes(message.Serialize()), endpoint);
        return true;
    }

    //*********************************************************************
    /// <summary>
    /// Demo to send packets in three different ways
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    //*********************************************************************

    public static bool Demo()
    {
        byte[] bites = { 0, 1, 2, 3, 4};
        string sturing = "";
        var sampleClass = new SampleClass();

        var transport = new TransportUdp("192.168.1.255", 45678);

        // Byte array
        transport.Send(bites);

        // String
        transport.Send(sturing);

        // Class (get serialized before transmission)
        transport.Send(sampleClass);

        return true;
    }

}

