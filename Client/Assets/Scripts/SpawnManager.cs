using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public RoomDetails RoomDetails;
    public GameObject Spawners;

    public Client client;
    public GameObject prefabPlayer;

    public GameObject[] PrefabsObstacles;


    // Start is called before the first frame update
    void Start()
    {
        
        RoomDetails.Spawners = Spawners;
    }

    // Update is called once per frame
    void Update()
    {
        if (RoomDetails.GameIsStarted)
        {
            if(RoomDetails.SpawnPlayers)
            {
                GameObject Player1 = Instantiate(prefabPlayer);
                GameObject Player2 = Instantiate(prefabPlayer);

                Player1.transform.position = Spawners.transform.GetChild(0).position;
                Player2.transform.position = Spawners.transform.GetChild(1).position;

                RoomDetails.SpawnPlayers = false;

                client.gameobjects.Add(Player1);
                client.gameobjects.Add(Player2);

            }

            

        }
    }

    //public void SpawnObstacle()
    //{
    //    GameObject go;

    //    if(objectType == 1)
    //    {
    //        go = PrefabsObstacles[0];
    //    }
    //    else if (objectType == 2)
    //    {
    //        go = PrefabsObstacles[1];
    //    }
    //    else 
    //    {
    //        go = PrefabsObstacles[2];
    //    }

    //    GameObject obstacle = Instantiate(go);

    //    if (laneToSpawn==1)
    //    {
    //        obstacle.transform.position = Spawners.transform.GetChild(2).transform.position;
    //    }else
    //    {
    //        obstacle.transform.position = Spawners.transform.GetChild(3).transform.position;

    //    }

    //    client.gameobjects.Add(obstacle);
    //}

}


