using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class GamePanelController : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI countdown;
    RoomDetails roomDetails;

    // Start is called before the first frame update
    void Start()
    {
        countdown = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        roomDetails = transform.GetComponentInParent<UIGameManager>().RoomDetails;
    }

    // Update is called once per frame
    void Update()
    {
        if (roomDetails.CountdownToPlay != 0) countdown.text = roomDetails.CountdownToPlay.ToString();
        else countdown.text = "START!";
        
    }
}
