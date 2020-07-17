using Assets;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

internal class UdpUser : UdpPositionBase
{
    private UdpUser()
    { }

    //Connect to the host and port
    public static UdpUser ConnectTo(string hostname, int port)
    {
        var connection = new UdpUser();
        connection.Client.Connect(hostname, port);
        return connection;
    }

    //Serialze and send off our packet
    public void Send(PosPacket packet)
    {
        var datagram = SerializationHandler.Serialize<PosPacket>(packet);
        Client.Send(datagram, datagram.Length);
    }
}

public class Client : MonoBehaviour
{
    [SerializeField]
    private UdpUser client;
    //If we ever encounter and Endpoint that is 1,1 that is considered the local client.
    public IPEndPoint selfPoint = new IPEndPoint(1, 1);

    public Dictionary<Guid, GameObject> playerObjects = new Dictionary<Guid, GameObject>();

    public GameObject playerPrefab;

    public Guid userGUID;
    //Start up connection and listen loop
    public InputField inputIP;
    public InputField inputPort;

 
    private void Start()
    {
        //All of these Find functions are very very low performance, and is awful code. Too bad!
        if (inputIP == null)
            inputIP = GameObject.Find("IPField").GetComponent<InputField>();
        if (inputPort == null)   
            inputPort = GameObject.Find("PortField").GetComponent<InputField>();

        //This is painful to see and is a freak of nature. Too bad!
        GameObject.Find("Canvas").SetActive(false);

        //Try connecting to our server and give ourselves a GUID. If we cared a lot more, we could save this guid for future use. 
        string ip = inputIP.text;
        int port = Int32.Parse(inputPort.text);
        userGUID = System.Guid.NewGuid(); 
        client = UdpUser.ConnectTo(ip, port);
    }

    public async void UpdatePlayers()
    {
        Debug.Log("Updating player positions");
        try
        {
            var recieved = await client.Recieve();
            //Process all info recieved (connect each player gameobject to their respective endpoints)
            if (playerObjects.ContainsKey(recieved.userID))
            {
                playerObjects[recieved.userID].transform.SetPositionAndRotation(recieved.Position, Quaternion.Euler(recieved.Rotation));
            }
            else
            {
                //Create new player if it doesnt exist in our dictionary
                playerObjects.Add(recieved.userID, Instantiate(playerPrefab));
                playerObjects[recieved.userID].transform.SetPositionAndRotation(recieved.Position, Quaternion.Euler(recieved.Rotation));
            }
        }
        catch (Exception ex)
        {
            Debug.Log(ex);
        }
    }

    public void Update()
    {
        //Send our info to the server!
        PosPacket pac = new PosPacket()
        {
            Sender = new IPEndPoint(1, 1),
            userID = userGUID,
            Position = new SerializableVector3(this.transform.position),
            Rotation = new SerializableVector3(this.transform.rotation.eulerAngles)
        };

        client.Send(pac);

        //Call the update players method.
        UpdatePlayers();
    }
}
