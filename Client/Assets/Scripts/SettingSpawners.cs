using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingSpawners : MonoBehaviour
{
    public RoomDetails roomDetails;
    public Transform ParentOfGameobjectsSpawned;
    public GameObject Spawners;



    void Start()
    {
        roomDetails.ParentOfGameobjectsSpawned = ParentOfGameobjectsSpawned;
        roomDetails.Spawners = Spawners;
        if (roomDetails.minePlayer.MyIdInRoom == 1)
        {
            roomDetails.minePlayer.Position = Spawners.transform.GetChild(0).position;
            roomDetails.otherPlayer.Position = Spawners.transform.GetChild(1).position;

            roomDetails.minePlayer.PositionOfSpawners = Spawners.transform.GetChild(2).position;
            roomDetails.otherPlayer.PositionOfSpawners = Spawners.transform.GetChild(3).position;

            
        }
        else
        {
            roomDetails.minePlayer.Position = Spawners.transform.GetChild(1).position;
            roomDetails.otherPlayer.Position = Spawners.transform.GetChild(0).position;

            roomDetails.minePlayer.PositionOfSpawners = Spawners.transform.GetChild(3).position;
            roomDetails.otherPlayer.PositionOfSpawners = Spawners.transform.GetChild(2).position;
        }

    }
}
