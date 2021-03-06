﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class RoomDetails : ScriptableObject
{
    [SerializeField]
    uint roomID;

    [SerializeField]
    int playerInRoom;

    [SerializeField]
    bool player1IsReady;

    [SerializeField]
    bool player2IsReady;

    [SerializeField]
    bool gameIsStarted;

    [SerializeField]
    bool spawnPlayers;

    [SerializeField]
    bool otherPlayerIsConnected;

    [SerializeField]
    float countdownToPlay;

    [SerializeField]
    Dictionary<uint, GameObject> gameObjects;

    [SerializeField]
    public Dictionary<uint, Vector3> gameObjectsNewPositions;

    [SerializeField]
    GameObject spawners;

    [SerializeField]
    Transform parentOfGameobjectsSpawned;

    [SerializeField]
    public GameObject[] prefabsObstacles;

    public GameObject prefabPlayer1;
    public GameObject prefabPlayer2;

    public PlayerDetails minePlayer;
    public PlayerDetails otherPlayer;

    private Vector3 MPpositionOfSpawner;
    private Vector3 OPpositionOfSpawner;


    public GameObject Spawners
    {
        get { return spawners; }
        set { spawners = value; }
    }

    public Transform ParentOfGameobjectsSpawned
    {
        get { return parentOfGameobjectsSpawned; }
        set { parentOfGameobjectsSpawned = value; }
    }

    public GameObject[] PrefabsObstacles
    {
        get { return prefabsObstacles; }
        set { prefabsObstacles = value; }
    }

    public GameObject PrefabPlayer1
    {
        get { return prefabPlayer1; }
        set { prefabPlayer1 = value; }
    }

    public GameObject PrefabPlayer2
    {
        get { return prefabPlayer2; }
        set { prefabPlayer2 = value; }
    }

    public Dictionary<uint, GameObject> GameObjects
    {
        get { return gameObjects; }
        set { gameObjects = value; }
    }

    public bool GameIsStarted
    {
        get { return gameIsStarted; }
        set { gameIsStarted = value; }
    }

    public bool OtherPlayerIsConnected
    {
        get { return otherPlayerIsConnected; }
        set { otherPlayerIsConnected = value; }
    }

    public bool SpawnPlayers
    {
        get { return (gameIsStarted && spawnPlayers); }
        set { spawnPlayers = value; }
    }

    public float CountdownToPlay
    {
        get { return countdownToPlay; }
        set { countdownToPlay = value; }
    }

    public int GetPlayerInRoom()
    {
        return playerInRoom;
    }

    public void AddPlayerRoom(bool trueToAddPlayer)
    {
        if (trueToAddPlayer == true)
        {
            playerInRoom++;
            if (playerInRoom >= 2)
            {
                playerInRoom = 2;
            }
        }
        else
        {
            playerInRoom--;
            if (playerInRoom <= 0)
            {
                playerInRoom = 0;
            }
        }
    }

    public bool Player1IsReady
    {
        get
        {
            return player1IsReady;
        }
        set
        {
            player1IsReady = value;
        }
    }

    public bool Player2IsReady
    {
        get
        {
            return player2IsReady;
        }
        set
        {
            player2IsReady = value;
        }
    }

    public bool PlayersAreReady
    {
        get
        {
            return (player1IsReady && player2IsReady);
        }
    }

    public uint RoomID
    {
        get
        {
            return roomID;
        }
        set
        {
            roomID = value;
        }
    }
    public void ResetElement()
    {
        playerInRoom = 0;
        roomID = 0;
        player1IsReady = false;
        player2IsReady = false;
        gameIsStarted = false;
        spawnPlayers = false;
        otherPlayerIsConnected = false;
        countdownToPlay = 3;
        gameObjects = new Dictionary<uint, GameObject>();
        gameObjectsNewPositions = new Dictionary<uint, Vector3>();


        
    }



    public void SpawnGameObject(uint id, uint objectType, Vector3 pos)
    {
        MPpositionOfSpawner = minePlayer.PositionOfSpawners;
        OPpositionOfSpawner = otherPlayer.PositionOfSpawners;


        GameObject go;

        if (objectType == 1)
        {
            if (id == minePlayer.MyIdInRoom)
            {
                go = prefabPlayer1;
                

            }
            else
            {
                go = prefabPlayer2;
                

            }
        }
        else if (objectType == 2)
        {
            go = prefabsObstacles[0];
        }
        else
        {
            go = prefabsObstacles[1];
        }

        

        GameObject obstacle = Instantiate(go, ParentOfGameobjectsSpawned);

        


        
        obstacle.transform.localPosition = pos;

        

        gameObjects.Add(id, obstacle);
        gameObjectsNewPositions.Add(id, obstacle.transform.position);
    }

    public bool UpdateGameObject(uint id, float posX, float posY, float posZ)
    {
        
        if (gameObjects.ContainsKey(id))
        {
            

            Vector3 newpos = new Vector3(posX, posY, posZ);

            gameObjectsNewPositions[id] = newpos;

            Vector3 oldpos = gameObjects[id].transform.position;



            return true;
        }
        else
        {
            return false;
        }
    }

    
}

