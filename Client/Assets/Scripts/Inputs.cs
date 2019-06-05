using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inputs : MonoBehaviour
{
    public RoomDetails roomDetails;
    public Animator animator;

   

    // Start is called before the first frame update
    void Start()
    {
           
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.M))
        {
            animator.SetBool("Intangible", true);
            Packet packet = new Packet(Client.COMMAND_INTANGIBLE, roomDetails.minePlayer.MyIdInRoom, roomDetails.RoomID,false);
            Client.Send(packet.GetData());
        }
        else if(Input.GetKeyUp(KeyCode.M))
        {
            animator.SetBool("Intangible", false);
            Packet packet = new Packet(Client.COMMAND_INTANGIBLE, roomDetails.minePlayer.MyIdInRoom, roomDetails.RoomID, true);
            Client.Send(packet.GetData());
        }
    }
}
