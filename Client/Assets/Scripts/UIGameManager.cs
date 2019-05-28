using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIGameManager : MonoBehaviour
{
    public RoomDetails RoomDetails;

    public GameObject PreStartPanel;
    public GameObject GamePanel;

    LobbyControl lobbyControl;

    
    // Start is called before the first frame update
    void Start()
    {
        lobbyControl = PreStartPanel.GetComponent<LobbyControl>();
        GamePanel.SetActive(false);
        PreStartPanel.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if(checkStartPanel())
        {
            GamePanel.SetActive(true);
            PreStartPanel.SetActive(false);
        }
        
    }

    private bool checkStartPanel()
    {
        bool StartCountdown = false;

        if(RoomDetails.PlayersAreReady)
        {
            lobbyControl.ReadyButton.SetActive(false);
            lobbyControl.WaitingText.SetActive(false);
            StartCountdown = true;
        }

        return StartCountdown;
    }
}
