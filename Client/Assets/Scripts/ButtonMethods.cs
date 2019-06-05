using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonMethods : MonoBehaviour
{
    public GameObject LoadingPanel;
    public Animator animator;

    byte[] packet;

    public void QuickJoinButton()
    {
        packet = new byte[1];
        packet[0] = 1;
        Client.Send(packet);
        LoadingPanel.gameObject.SetActive(true);
        animator.SetTrigger("Start");
    }
    
    public void QuitButton()
    {
        Application.Quit();
    }
}
