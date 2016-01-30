using UnityEngine;

using System;
using System.Net.Sockets;
using System.Net;
using System.Collections.Generic;

public enum ESocketBehaviour { None, Server, Client }

public class SocketComm : MonoBehaviour
{
    [Header("Socket Connection")]
    [SerializeField] private ESocketBehaviour socketBehaviour = ESocketBehaviour.None;

    [SerializeField] private string IP = "";
    [SerializeField] private int port = 0;

    private Socket mySocket = null;
    private byte[] buffer;

    [Header("Server Variables")] // Server Variables
    public GameObject potatoPrefab;
    private List<Socket> clients = null; // Only Initialize It If It's The Server
    private int potatoesToSpawn = 0;
    private Vector2 limits;

    /// <summary>
    /// Socket Initialization Place Any Other Code Before the Socket Initialization
    /// </summary>
    private void Start()
    {
        if (socketBehaviour == ESocketBehaviour.Server)
        {
            clients = new List<Socket>();

            buffer = new byte[1024];

            mySocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            IPEndPoint parsedIP = new IPEndPoint(IPAddress.Parse(IP), port);

            mySocket.Bind(parsedIP);
            mySocket.Listen(100);
            mySocket.BeginAccept( new AsyncCallback(AcceptCallback), null );

            return;
        }

        if (socketBehaviour == ESocketBehaviour.Client)
        {
            buffer = new byte[1];

            mySocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            mySocket.Connect(IPAddress.Parse(IP), port);

            return;
        }
    }

    /// <summary>
    /// Logic Code Here
    /// </summary>
    private void Update()
    {
        if (socketBehaviour == ESocketBehaviour.Server)
        {
            if (potatoesToSpawn > 0)
            {
                Vector3 position = new Vector3(UnityEngine.Random.Range(limits.x, limits.y), 4.5f, 0);
                Instantiate(potatoPrefab, position, Quaternion.identity);
                potatoesToSpawn--;
            }
        }

        if (socketBehaviour == ESocketBehaviour.Client)
        {
			Touch[] touches = Input.touches;
            if (touches[0].phase == TouchPhase.Ended || Input.GetKeyUp(KeyCode.Mouse0))
            {
                buffer[0] = 1; // Instantiate Message
                mySocket.Send(buffer, SocketFlags.None);
            }
        }
    }

    /// <summary>
    /// Closes Client On Application Quit So He Doesnt Spawn Garbage
    /// </summary>
    private void OnApplicationQuit()
    {
        if (socketBehaviour == ESocketBehaviour.Client)
        {
            buffer[0] = 0; // Quit Message
            mySocket.Send(buffer, SocketFlags.None);
            mySocket.Close();
        }

        if (socketBehaviour == ESocketBehaviour.Server)
        {
            for (int i = 0; i < clients.Count; i++)
            {
                clients[i].Close();
            }
            mySocket.Close();
        }
    }

    public void UpdateSpawnLimits(float x, float y)
    {
        limits.x = x;
        limits.y = y;
    }

    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //                                                  Server CallBacks                                                                    //
    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    private void AcceptCallback(IAsyncResult asyncResult)
    {
        // Get The Socket
        Socket socket = mySocket.EndAccept(asyncResult);
        // Add it to the list
        clients.Add(socket);
        // Receive info from the socket
        socket.BeginReceive(buffer, 0, 1024, SocketFlags.None, new AsyncCallback(ReceiveCallback), socket);
        // Return back to accepting
        mySocket.BeginAccept(new AsyncCallback(AcceptCallback), null);
    }
    private void ReceiveCallback(IAsyncResult asyncResult)
    {
        // Cache Socket
        Socket socket = (Socket)asyncResult.AsyncState;

        // Gets Socket
        try
        {
            socket = (Socket)asyncResult;
        }
        catch (Exception)
        {

            // Do Nothing MWAAHHAAHAHAHAHAHHAHAA! Take That EXCEPTIONS!
        }

        // Gets Bytes Received
        socket.EndReceive(asyncResult);

        if (buffer[0] == 0)
        {
            clients.Remove(socket);
            socket.Close();
            return;
        }

        // IncrementSpawning
        potatoesToSpawn++;

        // Returns Back To Receiving
        socket.BeginReceive(buffer, 0, 1024, SocketFlags.None, new AsyncCallback(ReceiveCallback), socket);
    }

}
