using UnityEngine;

using System;
using System.Net.Sockets;
using System.Net;
using System.Collections.Generic;
using System.Collections;

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
    public GameObject[] potatoPrefab;
    public Gradient potatoGradient;
    public AudioClip audioClip;
    private List<Socket> clients = null; // Only Initialize It If It's The Server
    private int potatoesToSpawn = 0;
    private Vector3 limits;

    private bool isFocused = true;
    private bool toPototatoeFountainOrNotToPotatoeFountain = true;

    /// <summary>
    /// Socket Initialization Place Any Other Code Before the Socket Initialization
    /// </summary>
    private void Start()
    {
        limits.x = -4;
        limits.y =  4;
        limits.z =  4;

        AudioManager.Instance.PlaySound(EAudioPlayType.BGM, audioClip);

        if (toPototatoeFountainOrNotToPotatoeFountain)
        { StartCoroutine(POTATOFOUNTAIN()); } 

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
                GameObject obj = ((GameObject)Instantiate(potatoPrefab[ UnityEngine.Random.Range(0, potatoPrefab.Length-1) ], position, startRotation));
                obj.GetComponent<SpriteRenderer>().color = potatoGradient.Evaluate(UnityEngine.Random.Range(0f,1f));
                obj.GetComponent<PotatoDeactivation>().PlaySound(EAudioPlayType.SFXPotatoSpawn);
                float scale = UnityEngine.Random.Range(0.18f, 0.30f);
                obj.GetComponent<Transform>().localScale = new Vector3(scale, scale, scale);


                /// Remove this else to break the game and make an infinite spawner. leave the -- parth though please!
                if (potatoesToSpawn > 5)
                    potatoesToSpawn = 0;
                else potatoesToSpawn--;
            }
        }

        if (socketBehaviour == ESocketBehaviour.Client)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (mySocket == null)
                    Debug.LogError("done fucked it man! D:");
                RequestPotatoCreation();
            }

            //Touch[] touches = Input.touches;
            //if (touches.Length > 0)
            //{
            //    if ( && touches[0])
            //    RequestPotatoCreation();
            //}
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
        if (buffer == null)
            buffer = new byte[1];
        buffer[0] = 1; // Instantiate Message
        mySocket.Send(buffer, SocketFlags.None);
    }

    private IEnumerator POTATOFOUNTAIN()
    {
        while (true)
        {
            Vector3 position = new Vector3(UnityEngine.Random.Range(limits.x, limits.z), limits.y, 0);
            Quaternion startRotation = Quaternion.Euler(new Vector3(0, 0, UnityEngine.Random.Range(0, 360)));
            GameObject obj = ((GameObject)Instantiate(potatoPrefab[UnityEngine.Random.Range(0, potatoPrefab.Length - 1)], position, startRotation));
            obj.GetComponent<SpriteRenderer>().color = potatoGradient.Evaluate(UnityEngine.Random.Range(0f, 1f));
            obj.GetComponent<PotatoDeactivation>().PlaySound(EAudioPlayType.SFXPotatoSpawn);
            float scale = UnityEngine.Random.Range(0.18f, 0.30f);
            obj.GetComponent<Transform>().localScale = new Vector3(scale, scale, scale);

            yield return new WaitForSeconds(0.1f);
        }
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
