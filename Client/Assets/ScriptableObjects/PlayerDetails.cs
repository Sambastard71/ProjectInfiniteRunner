﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class PlayerDetails : ScriptableObject
{
    uint roomId;

    uint myIdInRoom;

    Vector3 position;
    Vector3 positionOfSpawners;

    float colliderWidth;
    float colliderHeight;

    public float Latency;
    public float Width;
    public float Height;


    public uint RoomId
    {
        get
        {
            return roomId;
        }
        set
        {
            roomId = value;
        }
    }

    public uint MyIdInRoom
    {
        get
        {
            return myIdInRoom;
        }
        set
        {
            myIdInRoom = value;
        }
    }

    public Vector3 Position
    {
        get
        {
            return position;
        }
        set
        {
            position = value;
        }
    }

    public Vector3 PositionOfSpawners
    {
        get
        {
            return positionOfSpawners;
        }
        set
        {
            positionOfSpawners = value;
        }
    }

    public float ColliderWidth
    {
        get
        {
            return colliderWidth;
        }
        set
        {
            colliderWidth = value;
        }
    }

    public float ColliderHeight
    {
        get
        {
            return colliderHeight;
        }
        set
        {
            colliderHeight = value;
        }
    }

    public void ResetElement()
    {
        roomId = 0;
        myIdInRoom = 0;
        position = Vector3.zero;
    }
}
