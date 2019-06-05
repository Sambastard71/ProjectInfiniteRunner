using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Player2IsReady : MonoBehaviour
{
    public RoomDetails roomDetails;
    public TextMeshProUGUI text;

    string[] texts;

    // Start is called before the first frame update
    void Start()
    {
        texts = new string[2] {"Player2 isn't Ready!","Player2 is Ready!"};
    }

    // Update is called once per frame
    void Update()
    {
        if(roomDetails.Player2IsReady)
        {
            text.text = texts[1];
            text.color = new Color(0, 1, 0);
        }else
        {
            text.text = texts[0];
            text.color = new Color(1, 0, 0);

        }
    }
}
