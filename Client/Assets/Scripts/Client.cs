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
    public delegate void EventP2Intangible(bool boolean);
    public static event EventP2Intangible Player2Intangible;

    public delegate void Player2Moves(float Zpos);
    public static event Player2Moves movesPlayer2;

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
    public const byte COMMAND_INTANGIBLEOP = 13;
    public const byte COMMAND_CHANGEPLAYERPOSITION = 15;
    public const byte COMMAND_CHANGEOTHERPLAYERPOSITION = 16;
    public const byte COMMAND_CHECKPING = 17;
    public const byte COMMAND_CHECKRESPONSE = 18;



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
    public RoomDetails roomDetails;

    public List<GameObject> gameobjects = new List<GameObject>();

    bool isSpawnedPlayer2 = false;
    bool countdownStart = false;

    private void OnEnable()
    {
        sceneManagement = GetComponent<SceneManagement>();
        MinePlayer.ResetElement();
        OtherPlayer.ResetElement();
        roomDetails.ResetElement();

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
        commands[COMMAND_INTANGIBLEOP] = IntangibleOP;
        commands[COMMAND_CHANGEOTHERPLAYERPOSITION] = ChangePosOP;
        commands[COMMAND_CHECKPING] = CheckLatency;
        commands[COMMAND_CHECKRESPONSE] = CheckResponse;


        sceneManagement.LoadGameScene();
        SceneManager.activeSceneChanged += LoadGameSceneElements;
    }

   

    private void Update()
    {
        EndPoint sender = new IPEndPoint(0, 0);
        byte[] data = Recv(256, ref sender);

        if (data!= null)
        {
            
            commands[data[0]](data,sender);
        }


        foreach (KeyValuePair<uint, GameObject> gos in roomDetails.GameObjects)
        {
            uint key = gos.Key;

            if (key == 1 || key == 2)
                continue;
        
            GameObject go = gos.Value;

            go.transform.position = Vector3.Lerp(go.transform.position, roomDetails.gameObjectsNewPositions[key],(1+MinePlayer.Latency)*Time.deltaTime);

        }

        if (!isSpawnedPlayer2)
        {
            if (SceneManager.GetActiveScene().buildIndex == 1 && roomDetails.Player2IsReady)
            {
                roomDetails.SpawnGameObject(OtherPlayer.MyIdInRoom, 1, OtherPlayer.Position);
                isSpawnedPlayer2 = true;
                animatorGame.SetTrigger("Ready2");
            }
        }

        if(roomDetails.GameIsStarted)
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

        roomDetails.RoomID = MinePlayer.RoomId;

        roomDetails.AddPlayerRoom(true);

        

        animatorMenu.SetTrigger("FadeIn");

        
        Invoke("LoadGameScene", 2);
        
    }
    
    // (command, player2Id, roomId)
    void OtherPlayerConnected(byte[] data, EndPoint sender)
    {
        roomDetails.OtherPlayerIsConnected = true;

        if (data.Length > 9)
        {
            return;
        }
        
        if (roomDetails.RoomID != BitConverter.ToUInt32(data, 5))
        {
            return;
        }

        OtherPlayer.MyIdInRoom = BitConverter.ToUInt32(data, 1);
        OtherPlayer.RoomId = BitConverter.ToUInt32(data, 5);

        roomDetails.AddPlayerRoom(true);
        
        

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

        if (roomDetails.RoomID != BitConverter.ToUInt32(data, 5))
        {
            return;
        }

        OtherPlayer.MyIdInRoom = BitConverter.ToUInt32(data, 1);
        OtherPlayer.RoomId = BitConverter.ToUInt32(data, 5);


        roomDetails.AddPlayerRoom(true);

        float posX = BitConverter.ToSingle(data, 9);
        float posY = BitConverter.ToSingle(data, 13);
        float posZ = BitConverter.ToSingle(data, 17);

        OtherPlayer.Position = new Vector3(posX,posY,posZ);

        float posXSpawnerObstacles = BitConverter.ToSingle(data, 21);
        float posYSpawnerObstacles = BitConverter.ToSingle(data, 25);
        float posZSpawnerObstacles = BitConverter.ToSingle(data, 29);

        OtherPlayer.PositionOfSpawners = new Vector3(posXSpawnerObstacles, posYSpawnerObstacles,posZSpawnerObstacles);

        roomDetails.Player2IsReady = true;

        

        
      


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

        if (roomDetails.RoomID != BitConverter.ToUInt32(data, 1))
        {
            return;
        }

        float countdownValue = BitConverter.ToUInt32(data, 5);

        if (countdownValue == 3)
        {
            roomDetails.CountdownToPlay = 3;
        }
        else if (countdownValue == 2)
        {
            roomDetails.CountdownToPlay = 2;
        }
        else if (countdownValue == 1)
        {
            roomDetails.CountdownToPlay = 1;
        }
        else if (countdownValue <= 0)
        {
            roomDetails.CountdownToPlay = 0;
            roomDetails.GameIsStarted = true;
            roomDetails.SpawnPlayers = true;
        }


    }


    private void SpawnObstacle(byte[] data, EndPoint sender)
    {
        if (data.Length != 25)
        {
            return;
        }

        uint roomId = BitConverter.ToUInt32(data, 1);
        uint idinRoom = BitConverter.ToUInt32(data, 5);
        uint objectType = BitConverter.ToUInt32(data, 9);
        float posX = BitConverter.ToSingle(data, 13);
        float posY = BitConverter.ToSingle(data, 17);
        float posZ = BitConverter.ToSingle(data, 21);

        Vector3 pos = new Vector3(posX, posY, posZ);

        if (roomDetails.RoomID != roomId)
        {
            return;
        }

       

        roomDetails.SpawnGameObject(idinRoom,objectType,pos);
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

        if (roomDetails.RoomID != roomId)
        {
            return;
        }

        

        roomDetails.UpdateGameObject(idinRoom,posX,posY,posZ);



    }

    private void Collided(byte[] data, EndPoint sender)
    {
        if (data.Length != 9)
        {
            return;
        }

        uint idinRoom = BitConverter.ToUInt32(data, 1);
        uint roomId = BitConverter.ToUInt32(data, 5);
        

        if (roomDetails.RoomID != roomId)
        {
            return;
        }

        DestroyImmediate(roomDetails.GameObjects[idinRoom]);
        roomDetails.GameObjects.Remove(idinRoom);

    }

    private void DestroyObstacle(byte[] data, EndPoint sender)
    {
        if (data.Length != 9)
        {
            return;
        }

        uint idinRoom = BitConverter.ToUInt32(data, 5);
        uint roomId = BitConverter.ToUInt32(data, 1);


        if (roomDetails.RoomID != roomId)
        {
            return;
        }

        DestroyImmediate(roomDetails.GameObjects[idinRoom]);
        roomDetails.GameObjects.Remove(idinRoom);

    }

    private void IntangibleOP(byte[] data, EndPoint sender)
    {
        if (data.Length != 6)
        {
            return;
        }

        uint roomId = BitConverter.ToUInt32(data, 1);
        bool isIntangible = BitConverter.ToBoolean(data, 5);


        if (roomDetails.RoomID != roomId)
        {
            return;
        }

        Player2Intangible(isIntangible);

    }

    private void ChangePosOP(byte[] data, EndPoint sender)
    {
        if (data.Length != 13)
        {
            return;
        }
        
        uint rooomId = BitConverter.ToUInt32(data, 1);
        uint Id = BitConverter.ToUInt32(data, 5);
        float zPos = BitConverter.ToSingle(data, 9);

        if(Id != OtherPlayer.MyIdInRoom)
        {
            return;
        }

        movesPlayer2(zPos);
        

    }


    private void CheckLatency(byte[] data, EndPoint sender)
    {
        if (data.Length != 5)
        {
            return;
        }

        float time = BitConverter.ToSingle(data, 1);

        Packet packet = new Packet(Client.COMMAND_CHECKPING, MinePlayer.RoomId, MinePlayer.MyIdInRoom, time);
        Client.Send(packet.GetData());
    }

    private void CheckResponse(byte[] data, EndPoint sender)
    {
        if (data.Length != 13)
        {
            return;
        }

        uint roomID = BitConverter.ToUInt32(data, 1);
        uint Id = BitConverter.ToUInt32(data, 5);
        float ping = BitConverter.ToSingle(data, 9);

        MinePlayer.Latency = ping;
        Debug.Log(ping);
        
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

            if(roomDetails.OtherPlayerIsConnected==true)
            {
                animatorGame.SetTrigger("P2connected");
            }
        }
    }
}
