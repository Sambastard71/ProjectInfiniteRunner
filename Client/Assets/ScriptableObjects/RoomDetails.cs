using System.Collections;
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
    float countdownToPlay;

    [SerializeField]
    Dictionary<uint, GameObject> gameObjects;

    [SerializeField]
    GameObject spawners;

    [SerializeField]
    Transform parentOfGameobjectsSpawned;

    [SerializeField]
    public GameObject[] prefabsObstacles;

    [SerializeField]
    public GameObject prefabPlayer;


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

    public GameObject PrefabPlayer
    {
        get { return prefabPlayer; }
        set { prefabPlayer = value; }
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
        countdownToPlay = 3;
        gameObjects = new Dictionary<uint, GameObject>();
        
    }



    public void SpawnGameObject(uint id, uint objectType, int laneToSpawn)
    {
        GameObject go;

        switch (objectType)
        {
            case 1:
                go = PrefabPlayer;
                if (laneToSpawn == 1)
                {
                    Vector3 SpawnPlayer1 = Spawners.transform.GetChild(0).transform.position;
                    go.transform.position = new Vector3(SpawnPlayer1.x, SpawnPlayer1.y,0);
                }
                else
                {
                    Vector3 SpawnPlayer2 = Spawners.transform.GetChild(1).transform.position;
                    go.transform.position = new Vector3(SpawnPlayer2.x, SpawnPlayer2.y, 0);
                }

                break;
            case 2:
                go = prefabsObstacles[0];
                break;
            case 3:
                go = prefabsObstacles[1];
                break;
            case 4:
                go = prefabsObstacles[2];
                break;
            default:
                System.Console.WriteLine("Wrong object type");
                return;

        }

        GameObject obstacle = Instantiate(go, ParentOfGameobjectsSpawned);

        Debug.Log("Spawn Gameobj");


        if (objectType != 1 && laneToSpawn == 1)
        {
            obstacle.transform.position = Spawners.transform.GetChild(2).transform.position;
        }
        else if (objectType != 1 && laneToSpawn == 2)
        {
            obstacle.transform.position = Spawners.transform.GetChild(3).transform.position;

        }
        else
        {
            System.Console.WriteLine("Wrong spawn datas!");
        }

        gameObjects.Add(id, obstacle);
    }

    public bool UpdateGameObject(uint id, float posX, float posY)
    {
        if (gameObjects.ContainsKey(id))
        {
            gameObjects[id].transform.position = new Vector3(posX, posY, Spawners.transform.GetChild(2).position.z);

            return true;
        }
        else
        {
            return false;
        }
    }
}

