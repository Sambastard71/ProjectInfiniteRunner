using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inputs : MonoBehaviour
{
    public RoomDetails roomDetails;
    public float speed;

    public delegate void Player1Moves(float Zpos);
    public static event Player1Moves movesPlayer1;

    


    // Start is called before the first frame update
    void Start()
    {
          
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.M))
        {
            
            Packet packet = new Packet(Client.COMMAND_INTANGIBLE, roomDetails.minePlayer.MyIdInRoom, roomDetails.RoomID,false);
            Client.Send(packet.GetData());
        }
        else if(Input.GetKeyUp(KeyCode.M))
        {
            
            Packet packet = new Packet(Client.COMMAND_INTANGIBLE, roomDetails.minePlayer.MyIdInRoom, roomDetails.RoomID, true);
            Client.Send(packet.GetData());
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if(roomDetails.minePlayer.Position.z==90 || roomDetails.minePlayer.Position.z == 190)
            {
                return;
            }
            else
            {
                movesPlayer1(-45f);
            }
            
        }
        else if (Input.GetKeyUp(KeyCode.LeftArrow))
        {
            if (roomDetails.minePlayer.Position.z == 135 || roomDetails.minePlayer.Position.z == 235)
            {
                return;
            }
            else
            {
                movesPlayer1(+45f);
            }
        }
    }
}
