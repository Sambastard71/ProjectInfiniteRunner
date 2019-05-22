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
    const int COMMAND_P_CONNECTED = 3;
    command[] commands;

    static Socket socket;
    IPEndPoint myEndPoint;
    static IPEndPoint serverEndPoint;
    public static IPEndPoint ServerEndPoint
    {
        get
        {
            return serverEndPoint;
        }
    }

    SceneManagement sceneManagement;
    public PlayerDetails MinePlayer;
    public PlayerDetails OtherPlayer;
    public RoomDetails RoomDetails;

    private void OnEnable()
    {
        sceneManagement = GetComponent<SceneManagement>();
        MinePlayer.ResetElement();
        OtherPlayer.ResetElement();
        RoomDetails.ResetElement();

        socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        socket.Blocking = false;
        myEndPoint = new IPEndPoint(IPAddress.Parse(MyAddres), MyPort);
        socket.Bind(myEndPoint);

        serverEndPoint = new IPEndPoint(IPAddress.Parse(ServerAddress), ServerPort);

        DontDestroyOnLoad(this.gameObject);
        commands = new command[COMMAND_P_CONNECTED + 1];
        commands[COMMAND_WELCOME] = Welcome;
        commands[COMMAND_P_CONNECTED] = OtherPlayerConnected;

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


    public static bool Send(byte[] data)
    {
        bool success = false;
        try
        {
            int rlen = socket.SendTo(data, ServerEndPoint);
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

        MinePlayer.MyIdInRoom = BitConverter.ToUInt32(data, 1);
        MinePlayer.RoomId = BitConverter.ToUInt32(data, 5);

        RoomDetails.RoomID = MinePlayer.RoomId;

        RoomDetails.AddPlayerRoom(true);

        Debug.Log(MinePlayer.MyIdInRoom);
        Debug.Log(MinePlayer.RoomId);


    }
    
    // (command, player2Id, roomId)
    void OtherPlayerConnected(byte[] data, EndPoint sender)
    {
        if (data.Length > 9)
        {
            return;
        }
        
        if (RoomDetails.RoomID != BitConverter.ToUInt32(data, 5))
        {
            return;
        }

        OtherPlayer.MyIdInRoom = BitConverter.ToUInt32(data, 1);
        OtherPlayer.RoomId = BitConverter.ToUInt32(data, 5);

        RoomDetails.AddPlayerRoom(true);
        
        Debug.Log(OtherPlayer.MyIdInRoom);
        Debug.Log(OtherPlayer.RoomId);

    }


}
