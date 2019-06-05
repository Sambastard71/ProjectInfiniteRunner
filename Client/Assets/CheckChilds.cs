using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckChilds : MonoBehaviour
{
    public Inputs input;

    bool set = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!set)
        {
            if (transform.childCount >= 2)
            {
                input.animator = GameObject.FindGameObjectWithTag("Player1").GetComponent<Animator>();
                set = true;
            }
        }
    }
}
