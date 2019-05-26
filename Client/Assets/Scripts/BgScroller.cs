using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BgScroller : MonoBehaviour
{
    public float speed = 0.1f;
    public RoomDetails roomDetails;
    float y;
    Vector2 offset;
    Renderer mat;

    // Start is called before the first frame update
    void Awake()
    {
        mat = GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (roomDetails.GameIsStarted)
        {
            y = Mathf.Repeat(Time.time * speed, 1);
            offset = new Vector2(y, 0);
            mat.sharedMaterial.SetTextureOffset("_MainTex", offset);
        }
    }
}
