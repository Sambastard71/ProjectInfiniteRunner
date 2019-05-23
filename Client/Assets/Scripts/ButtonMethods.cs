using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonMethods : MonoBehaviour
{
    public FadeInPanel fadeInPanel;

    byte[] packet;

    public void QuickJoinButton()
    {
        packet = new byte[1];
        packet[0] = 1;
        Client.Send(packet);
        fadeInPanel.gameObject.SetActive(true);
        fadeInPanel.FadeIn = true;
    }
    
    public void QuitButton()
    {
        Application.Quit();
    }
}
