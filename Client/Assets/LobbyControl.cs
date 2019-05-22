using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LobbyControl : MonoBehaviour
{
    public RoomDetails RoomDetails;

    public GameObject Slider;
    public GameObject WaitingText;
    public GameObject ReadyButton;

    string[] text;

    public PlayerDetails MinePlayer;

    float AlphaDuration;
    float AlphaIncVal;
    Image img;
    TextMeshProUGUI textComponent;

    private void Start()
    {
        text = new string[2];
        text[0] = "Waiting For Player";
        text[1] = "Are U Ready?";

        textComponent = WaitingText.GetComponent<TextMeshProUGUI>();
        textComponent.text = text[0];

        img = GetComponent<Image>();
        img.canvasRenderer.SetAlpha(1);
        AlphaDuration = 0.2f;
        AlphaIncVal = -0.2f;
    }

    void Update()
    {
        if (RoomDetails.GetPlayerInRoom() == 2)
        {
            Slider.SetActive(false);

            if (!RoomDetails.Player1IsReady)
            {
                textComponent.text = text[1];
                ReadyButton.SetActive(true);
            }
        }

        if (RoomDetails.PlayersAreReady)
        {
            img.CrossFadeAlpha(AlphaIncVal, AlphaDuration, true);
            if (img.canvasRenderer.GetAlpha() <= 0.4f)
            {
                ReadyButton.SetActive(false);
                WaitingText.SetActive(false);

                this.gameObject.SetActive(false);
            }
        }
    }

    // (comando,idpersonaggioNellaStanza,idRoom,xpos,ypos,zpos,width,height collider)
    public void OnClick()
    {
        
        uint command_setup = 6;
        uint idMinePLayer = MinePlayer.MyIdInRoom;
        uint roomId = RoomDetails.RoomID;
        float posX = MinePlayer.Position.x;
        float posY = MinePlayer.Position.y;
        float posZ = MinePlayer.Position.z;
        float colliderWidth = MinePlayer.ColliderWidth;
        float colliderHeight = MinePlayer.ColliderHeight;

        RoomDetails.Player1IsReady = true;

        Packet packet = new Packet(command_setup, idMinePLayer, roomId, posX, posY, posZ, colliderWidth, colliderHeight);
        Client.Send(packet.GetData());

        textComponent.text = text[0];
        ReadyButton.SetActive(false);
    }
}
