using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeCars : MonoBehaviour
{
    public Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.M))
        {
            anim.SetBool("Intangible", true);
        }
        else if (Input.GetKeyUp(KeyCode.M))
        {
            anim.SetBool("Intangible", false);
        }
    }
}
