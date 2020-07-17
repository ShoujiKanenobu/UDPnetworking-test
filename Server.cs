using Assets;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using UnityEngine;


[System.Serializable]
public struct SerializableVector3
{

    public float x;
    public float y;
    public float z;

    public SerializableVector3(float rX, float rY, float rZ)
    {
        x = rX;
        y = rY;
        z = rZ;
    }

    public SerializableVector3(Vector3 r)
    {
        x = r.x;
        y = r.y;
        z = r.z;
    }

    public static implicit operator Vector3(SerializableVector3 rValue)
    {
        return new Vector3(rValue.x, rValue.y, rValue.z);
    }

    public static implicit operator SerializableVector3(Vector3 rValue)
    {
        return new SerializableVector3(rValue.x, rValue.y, rValue.z);
    }
}

//Gloabl Position Packet struct
[Serializable]
public struct PosPacket
{
    public IPEndPoint Sender;
    public Guid userID;
    public SerializableVector3 Position;
    public SerializableVector3 Rotation;
}

//Position base
internal abstract class UdpPositionBase
{
    protected UdpClient Client;

    protected UdpPositionBase()
    {
        Client = new UdpClient();
    }

    //Deserialze packet and return our packet struct.
    public async Task<PosPacket> Recieve()
    {
        var result = await Client.ReceiveAsync();
        PosPacket temp = SerializationHandler.Deserialize<PosPacket>(result.Buffer);
        temp.Sender = result.RemoteEndPoint;
        return temp;
    }
}

//Listener
internal class UdpListener : UdpPositionBase
{
    private IPEndPoint ListenOn;

    //Constructor users any ip address from port 2437
    public UdpListener() : this(new IPEndPoint(IPAddress.Any, 2437))
    {
    }
    //Connected to the above
    public UdpListener(IPEndPoint endpoint)
    {
        ListenOn = endpoint;
        Client = new UdpClient(ListenOn);
    }

    //Reply by serializing the packet info and sending to everyone excpet the sender of the packet
    public void Reply(PosPacket packet, Dictionary<IPEndPoint, Guid> clients)
    {
        //Dont send back position values to the client who sent us the values
        foreach (KeyValuePair<IPEndPoint, Guid> client in clients)
        {
            if (client.Value != packet.userID)
            {
                var datagram = SerializationHandler.Serialize<PosPacket>(packet);
                Client.Send(datagram, datagram.Length, client.Key);
            }
        }
    }
}


public class Server : MonoBehaviour
{
    //This runs once before any updates are done to it.
    public void Start()
    {
        var server = new UdpListener();
        Dictionary<IPEndPoint, Guid> clients = new Dictionary<IPEndPoint, Guid>();

        //Using threading
        Task.Factory.StartNew(async () =>
        {
            //Some we want to keep this thread going forever.
            while (true)
            {
                var recieved = await server.Recieve();
                if(!clients.ContainsKey(recieved.Sender))
                    clients.Add(recieved.Sender, recieved.userID);   
                server.Reply(recieved, clients);
            }
        });
    }

}
