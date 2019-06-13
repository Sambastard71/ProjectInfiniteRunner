using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCar : MonoBehaviour
{
    public RoomDetails roomDetails;
    public PlayerDetails minePlayer;
    

    Vector3 newPos;

    float startMoveTime;
    float distOfMovement;
    float distCovered;
    float speed = 45;
    float fraction;

    private void OnEnable()
    {
        Inputs.movesPlayer1 += MoveCarEvent;
       
    }

    private void Start()
    {
        
        newPos = roomDetails.Spawners.transform.GetChild((int)minePlayer.MyIdInRoom-1).position;
    }

    // Update is called once per frame
    void Update()
    {
        distCovered = (Time.time - startMoveTime) * speed;
        fraction = distCovered / distOfMovement;

        transform.position = Vector3.Lerp(transform.position, newPos, fraction);
    }

    void MoveCarEvent(float Zpos)
    {
        startMoveTime = Time.time;

        Vector3 mypos = minePlayer.Position;

        newPos = mypos + new Vector3(0,0,Zpos);
        
        distOfMovement = Vector3.Distance(newPos, minePlayer.Position);

        minePlayer.Position = newPos;


        Packet packet = new Packet(Client.COMMAND_CHANGEPLAYERPOSITION,minePlayer.RoomId,minePlayer.MyIdInRoom, minePlayer.Position.z);
        Client.Send(packet.GetData());
    }
}
