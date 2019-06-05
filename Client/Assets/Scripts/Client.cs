using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.Net;
using System;
using UnityEngine.SceneManagement;
using System.Net.NetworkInformation;


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
    public const byte COMMAND_DESTROY= 14;
    public const byte COMMAND_INTANGIBLE_OP = 13;

    public Animator animatorMenu;
    public Animator animatorGame;

    public Animator animatorOP;

    System.Net.NetworkInformation.Ping pingSender = new System.Net.NetworkInformation.Ping();
    PingOptions options = new PingOptions(64, true);
    PingReply reply;

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

    bool isSpawnedPlayer2 = false;
    bool countdownStart = false;

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
        commands[COMMAND_DESTROY] = DestroyObstacle;
        commands[COMMAND_INTANGIBLE_OP] = IntangibleOP;




        sceneManagement.LoadGameScene();
        SceneManager.activeSceneChanged += LoadGameSceneElements;
    }

   

    private void Update()
    {
        EndPoint sender = new IPEndPoint(0, 0);
        byte[] data = Recv(256, ref sender);

        if (data != null)
        {
            Debug.Log(data[0]);
            commands[data[0]](data, sender);
        }
        
        //CheckLatency(MinePlayer);

        foreach (KeyValuePair<uint, GameObject> gos in RoomDetails.GameObjects)
        {
            uint key = gos.Key;
            GameObject go = gos.Value;

            go.transform.position = Vector3.Lerp(go.transform.position, RoomDetails.gameObjectsNewPositions[key],(1+MinePlayer.Latency)*Time.deltaTime);
        }

        if (!isSpawnedPlayer2)
        {
            if (SceneManager.GetActiveScene().buildIndex == 1 && RoomDetails.Player2IsReady)
            {
                RoomDetails.SpawnGameObject(OtherPlayer.MyIdInRoom, 1, (int)OtherPlayer.MyIdInRoom);
                isSpawnedPlayer2 = true;
                animatorGame.SetTrigger("Ready2");
            }
        }

        if(RoomDetails.GameIsStarted)
        {
            animatorGame.SetBool("Countdown", false);
        }

       
        


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

        animatorMenu.SetTrigger("FadeIn");

        
        Invoke("LoadGameScene", 2);
        
    }
    
    // (command, player2Id, roomId)
    void OtherPlayerConnected(byte[] data, EndPoint sender)
    {
        RoomDetails.OtherPlayerIsConnected = true;

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

        if(SceneManager.GetActiveScene().buildIndex==1)
        {
            animatorGame.SetTrigger("P2connected");
        }

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

        

        
      


    }

    void Countdown(byte[] data, EndPoint sender)
    {
        if (!countdownStart)
        {
            animatorGame.SetBool("Countdown", true);
            countdownStart = false;
        }

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

    private void DestroyObstacle(byte[] data, EndPoint sender)
    {
        if (data.Length != 9)
        {
            return;
        }

        uint idinRoom = BitConverter.ToUInt32(data, 5);
        uint roomId = BitConverter.ToUInt32(data, 1);


        if (RoomDetails.RoomID != roomId)
        {
            return;
        }

        DestroyImmediate(RoomDetails.GameObjects[idinRoom]);
        RoomDetails.GameObjects.Remove(idinRoom);

    }

    private void IntangibleOP(byte[] data, EndPoint sender)
    {
        if (data.Length != 9)
        {
            return;
        }

        uint roomId = BitConverter.ToUInt32(data, 1);
        bool OtherPlaierIsIntangible = BitConverter.ToBoolean(data, 5);


        if (RoomDetails.RoomID != roomId)
        {
            return;
        }

        

    }

    private void CheckLatency(PlayerDetails player)
    {
        

        reply = pingSender.Send(serverEndPoint.Address);
        

        player.Latency = reply.RoundtripTime/1000;
    }

    private void LoadGameScene()
    {
        
        sceneManagement.StartLoadScene();
    }

    private void LoadGameSceneElements(Scene current, Scene next)
    {
        if(next.isLoaded && next.buildIndex==1)
        {
            animatorGame = GameObject.FindGameObjectWithTag("CanvasGame").GetComponent<Animator>();

            if(RoomDetails.OtherPlayerIsConnected==true)
            {
                animatorGame.SetTrigger("P2connected");
            }
        }
    }
}
