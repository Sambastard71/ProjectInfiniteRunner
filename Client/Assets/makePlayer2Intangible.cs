using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class makePlayer2Intangible : MonoBehaviour
{
    public Animator anim;

    // Start is called before the first frame update
    private void OnEnable()
    {
        Client.Player2Intangible += IntangibleAnimationStart;
    }

    void IntangibleAnimationStart(bool boolean)
    {
        anim.SetBool("Intangible", !boolean);
    }
}
