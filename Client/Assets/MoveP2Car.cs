using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveP2Car : MonoBehaviour
{
    public RoomDetails roomDetails;
    public PlayerDetails otherPlayer;

    float startMoveTime;
    float distOfMovement;
    float distCovered;
    float speed = 45;
    float fraction;

    Vector3 newPos;

    private void OnEnable()
    {
        Client.movesPlayer2 += MoveCarEvent;
    }

    private void Start()
    {
        newPos = roomDetails.Spawners.transform.GetChild((int)otherPlayer.MyIdInRoom - 1).position;
    }

    // Update is called once per frame
    void Update()
    {
        distCovered = (Time.time - startMoveTime) * speed;
        fraction = distCovered / distOfMovement;

        transform.position = Vector3.Lerp(transform.position, newPos, fraction);
    }

    private void MoveCarEvent(float Zpos)
    {
        startMoveTime = Time.time;

        Vector3 mypos = transform.position;

        newPos = new Vector3(transform.position.x, transform.position.y, Zpos);

        distOfMovement = Vector3.Distance(newPos, transform.position);

        

         
    }
}
