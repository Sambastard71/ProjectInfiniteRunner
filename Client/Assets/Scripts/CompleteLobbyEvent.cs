using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

[System.Serializable]
public class OnFullLobby : UnityEvent {}

public class CompleteLobbyEvent : MonoBehaviour
{
    public OnFullLobby OnFullLobby;

    public void SetUpGame()
    {

    }

    UnityAction StartGame;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
