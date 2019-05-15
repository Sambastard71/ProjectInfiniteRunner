using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonMethods : MonoBehaviour
{
    public Client Client;
    public FadeInPanel fadeInPanel;

    byte[] packet;

    public void QuickJoinButton()
    {
        packet = new byte[1];
        packet[0] = 0;
        Client.Send(packet, Client.ServerEndPoint);
        fadeInPanel.gameObject.SetActive(true);
        fadeInPanel.FadeIn = true;
    }
    
    public void QuitButton()
    {
        Application.Quit();
    }
}
