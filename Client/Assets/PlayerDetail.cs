using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDetail : MonoBehaviour
{
    uint roomId;
    uint myIdInRoom;

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
}
