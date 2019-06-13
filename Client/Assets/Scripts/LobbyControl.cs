using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LobbyControl : MonoBehaviour
{
    public RoomDetails RoomDetails;
    public PlayerDetails MinePlayer;
    public Animator animatorGame;
    public Inputs inputs;

   

    // (comando,idpersonaggioNellaStanza,idRoom,xpos,ypos,zpos,width,height collider)
    public void OnClick()
    {
        
        byte command_setup = 6;
        uint idMinePLayer = MinePlayer.MyIdInRoom;
        uint roomId = RoomDetails.RoomID;
        float posX = MinePlayer.Position.x;
        float posY = MinePlayer.Position.y;
        float posZ = MinePlayer.Position.z;
        float colliderWidth = MinePlayer.ColliderWidth;
        float colliderHeight = MinePlayer.ColliderHeight;
        float SpawnObstacleX = MinePlayer.PositionOfSpawners.x;
        float SpawnObstacleY = MinePlayer.PositionOfSpawners.y;
        float SpawnObstacleZ = MinePlayer.PositionOfSpawners.z;

        RoomDetails.Player1IsReady = true;
        

        Packet packet = new Packet(command_setup, idMinePLayer, roomId, posX, posY ,posZ, colliderWidth, colliderHeight,SpawnObstacleX,SpawnObstacleY,SpawnObstacleZ);
        
        Client.Send(packet.GetData());

        RoomDetails.SpawnGameObject(MinePlayer.MyIdInRoom, 1, MinePlayer.Position);
        
        animatorGame.SetTrigger("Ready");

       
    }
}
