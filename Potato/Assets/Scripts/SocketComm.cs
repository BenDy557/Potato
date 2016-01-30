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
    private Vector3 limits;

    private bool isFocused = true;

    /// <summary>
    /// Socket Initialization Place Any Other Code Before the Socket Initialization
    /// </summary>
    private void Start()
    {
        limits.x = -4;
        limits.y =  4;
        limits.z =  4;

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
                Vector3 position = new Vector3(UnityEngine.Random.Range(limits.x, limits.z), limits.y, 0);
                Quaternion startRotation = Quaternion.Euler(new Vector3(0, 0, UnityEngine.Random.Range(0, 360)));
                Instantiate(potatoPrefab, position, startRotation);
                potatoesToSpawn--;
            }
        }

        if (socketBehaviour == ESocketBehaviour.Client)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                RequestPotatoCreation();
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

    // Controls The Bugs On Minimizing And Focusing
    private void OnApplicationFocus()
    {
        if (socketBehaviour == ESocketBehaviour.Client)
        {
            isFocused = !isFocused;

            if (!isFocused) // Lost focus
            {


                mySocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                mySocket.Connect(IPAddress.Parse(IP), port);
            }
            else // is focused
            {
                buffer[0] = 0; // Quit Message
                mySocket.Send(buffer, SocketFlags.None);
                mySocket.Close();
            }
        }
    }
    public void UpdateSpawnLimits( float y )
    {
        limits.y = y;
    }
    public void RequestPotatoCreation()
    {
        buffer[0] = 1; // Instantiate Message
        mySocket.Send(buffer, SocketFlags.None);
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
