using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class RoomDetails : ScriptableObject
{
    [SerializeField]
    uint roomID;
    int playerInRoom;

    bool player1IsReady;
    bool player2IsReady;

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
    }
}
