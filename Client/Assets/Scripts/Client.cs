using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.Net;
using System;


delegate void command(byte[] packet, EndPoint sender);

public class Client : MonoBehaviour
{
    public string ServerAddress;
    public int ServerPort;

    public string MyAddres;
    public int MyPort;

    const int COMMAND_WELCOME = 2;
    command[] commands;

    Socket socket;
    IPEndPoint myEndPoint;
    IPEndPoint serverEndPoint;
    public IPEndPoint ServerEndPoint
    {
        get
        {
            return serverEndPoint;
        }
    }

    SceneManagement sceneManagement;
    PlayerDetail playerDetail;

    private void OnEnable()
    {
        sceneManagement = GetComponent<SceneManagement>();
        playerDetail = GetComponent<PlayerDetail>();
        
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        socket.Blocking = false;
        myEndPoint = new IPEndPoint(IPAddress.Parse(MyAddres), MyPort);
        socket.Bind(myEndPoint);

        serverEndPoint = new IPEndPoint(IPAddress.Parse(ServerAddress), ServerPort);

        DontDestroyOnLoad(this.gameObject);
        commands = new command[COMMAND_WELCOME + 1];
        commands[COMMAND_WELCOME] = Welcome;
    }

    private void Update()
    {
        EndPoint sender = new IPEndPoint(0, 0);
        byte[] data = Recv(256, ref sender);

        if (data == null)
        {
            return;
        }

        commands[data[0]](data, sender);

    }


    public bool Send(byte[] data, EndPoint endpoint)
    {
        bool success = false;
        try
        {
            int rlen = socket.SendTo(data, endpoint);
            if (rlen == data.Length)
                success = true;
        }
        catch
        {
            success = false;
        }
        return success;
    }

    public byte[] Recv(int bufferSize, ref EndPoint sender)
    {
        int rlen = -1;
        byte[] data = new byte[bufferSize];
        try
        {
            rlen = socket.ReceiveFrom(data, ref sender);
            if (rlen <= 0)
                return null;
        }
        catch
        {
            return null;
        }
        byte[] trueData = new byte[rlen];
        Buffer.BlockCopy(data, 0, trueData, 0, rlen);
        return trueData;
    }

    void Welcome(byte[] data, EndPoint sender)
    {
        if (data.Length > 9)
        {
            return;
        }

        sceneManagement.LoadGameScene();

        playerDetail.MyIdInRoom = BitConverter.ToUInt32(data, 1);
        playerDetail.RoomId = BitConverter.ToUInt32(data, 5);

        Debug.Log(playerDetail.MyIdInRoom);
        Debug.Log(playerDetail.RoomId);


    }


}
