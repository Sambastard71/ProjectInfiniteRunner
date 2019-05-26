using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingSpawners : MonoBehaviour
{
    public RoomDetails roomDetails;
    public Transform ParentOfGameobjectsSpawned;
    public PlayerDetails MinePlayer;
    public float colliderWidth;
    public float colliderHeight;


    void Start()
    {
        if(MinePlayer.MyIdInRoom==1)
        {
            MinePlayer.Position = transform.GetChild(0).position;
            MinePlayer.PositionOfSpawners = transform.GetChild(2).position;
        }
        else
        {
            MinePlayer.Position = transform.GetChild(1).position;
            MinePlayer.PositionOfSpawners = transform.GetChild(3).position;
        }

        MinePlayer.ColliderWidth = colliderWidth;
        MinePlayer.ColliderHeight = colliderHeight;

        roomDetails.Spawners = this.gameObject;
        roomDetails.ParentOfGameobjectsSpawned = ParentOfGameobjectsSpawned;
    }
}
