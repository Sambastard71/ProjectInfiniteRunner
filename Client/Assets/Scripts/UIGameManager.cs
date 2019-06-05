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
    Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        lobbyControl = PreStartPanel.GetComponent<LobbyControl>();

    }
    // Update is called once per frame
    void Update()
    {
        
        
    }

    
}
