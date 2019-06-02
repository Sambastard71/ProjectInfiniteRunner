using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.Net;
using System;
using UnityEngine.SceneManagement;


delegate void command(byte[] packet, EndPoint sender);

public class Client : MonoBehaviour
{
    public string ServerAddress;
    public int ServerPort;

    public string MyAddres;
    public int MyPort;

    public const byte COMMAND_UPDATE= 2;
    public const byte COMMAND_WELCOME = 5;
    public const byte COMMAND_P_CONNECTED = 8;
    public const byte COMMAND_SETUP_OP = 9;
    public const byte COMMAND_COUNTDOWN = 10;
    public const byte COMMAND_SPAWN_OBSTACLE = 11;
    public const byte COMMAND_COLLIDE = 12;
    public const byte COMMAND_INTANGIBLE = 7;


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

    public List<GameObject> gameobjects = new List<GameObject>();

    

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
        commands = new command[20];

        commands[COMMAND_UPDATE] = Update;
        commands[COMMAND_WELCOME] = Welcome;
        commands[COMMAND_P_CONNECTED] = OtherPlayerConnected;
        commands[COMMAND_SETUP_OP] = SetUpOtherPlayer;
        commands[COMMAND_COUNTDOWN] = Countdown;
        commands[COMMAND_SPAWN_OBSTACLE] = SpawnObstacle;
        commands[COMMAND_COLLIDE] = Collided;
        



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

    void SetUpOtherPlayer(byte[] data, EndPoint sender)
    {
        if (data.Length != 33)
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

        float posX = BitConverter.ToSingle(data, 9);
        float posY = BitConverter.ToSingle(data, 13);
        float posZ = BitConverter.ToSingle(data, 17);

        OtherPlayer.Position = new Vector3(posX,posY,posZ);

        float posXSpawnerObstacles = BitConverter.ToSingle(data, 21);
        float posYSpawnerObstacles = BitConverter.ToSingle(data, 25);
        float posZSpawnerObstacles = BitConverter.ToSingle(data, 29);

        OtherPlayer.PositionOfSpawners = new Vector3(posXSpawnerObstacles, posYSpawnerObstacles,posZSpawnerObstacles);

        RoomDetails.Player2IsReady = true;

        Debug.Log("OtherPlayer Setupped!");

        
        RoomDetails.SpawnGameObject(OtherPlayer.MyIdInRoom,1, 2);


    }

    void Countdown(byte[] data, EndPoint sender)
    {
        if (data.Length != 9)
        {
            return;
        }

        if (RoomDetails.RoomID != BitConverter.ToUInt32(data, 1))
        {
            return;
        }

        float countdownValue = BitConverter.ToUInt32(data, 5);

        if (countdownValue == 3)
        {
            RoomDetails.CountdownToPlay = 3;
        }
        else if (countdownValue == 2)
        {
            RoomDetails.CountdownToPlay = 2;
        }
        else if (countdownValue == 1)
        {
            RoomDetails.CountdownToPlay = 1;
        }
        else if (countdownValue <= 0)
        {
            RoomDetails.CountdownToPlay = 0;
            RoomDetails.GameIsStarted = true;
            RoomDetails.SpawnPlayers = true;
        }


    }


    private void SpawnObstacle(byte[] data, EndPoint sender)
    {
        if (data.Length != 17)
        {
            return;
        }

        uint roomId = BitConverter.ToUInt32(data, 1);
        uint idinRoom = BitConverter.ToUInt32(data, 5);
        uint objectType = BitConverter.ToUInt32(data, 9);
        int laneToSpawn = BitConverter.ToInt32(data, 13);

        if (RoomDetails.RoomID != roomId)
        {
            return;
        }

        Debug.Log("Spawn data recieved");

        RoomDetails.SpawnGameObject(idinRoom,objectType,laneToSpawn);
    }

    private void Update(byte[] data, EndPoint sender)
    {
        if (data.Length != 21)
        {
            return;
        }

        uint idinRoom = BitConverter.ToUInt32(data, 1);
        uint roomId = BitConverter.ToUInt32(data, 5);
        float posX = BitConverter.ToSingle(data, 9);
        float posY = BitConverter.ToSingle(data, 13);
        float posZ = BitConverter.ToSingle(data, 17);

        if (RoomDetails.RoomID != roomId)
        {
            return;
        }

        

        RoomDetails.UpdateGameObject(idinRoom,posX,posY,posZ);



    }

    private void Collided(byte[] data, EndPoint sender)
    {
        if (data.Length != 9)
        {
            return;
        }

        uint idinRoom = BitConverter.ToUInt32(data, 1);
        uint roomId = BitConverter.ToUInt32(data, 5);
        

        if (RoomDetails.RoomID != roomId)
        {
            return;
        }

        DestroyImmediate(RoomDetails.GameObjects[idinRoom]);
        RoomDetails.GameObjects.Remove(idinRoom);

    }
}
