using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeInPanel : MonoBehaviour
{
    public bool FadeIn;

    public GameObject Slider;
    public GameObject TextTMP;

    float AlphaDuration;
    float AlphaIncVal;
    Image img;

    public SceneManagement sceneManagement;
    public bool CanLoadScene;

    // Start is called before the first frame update
    void Start()
    {
        FadeIn = false;
        img = GetComponent<Image>();
        img.canvasRenderer.SetAlpha(0);
        AlphaDuration = 0.2f;
        AlphaIncVal = 1;
        Slider.active = false;
        TextTMP.active = false;
        gameObject.active = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (FadeIn)
        {
            img.CrossFadeAlpha(AlphaIncVal, AlphaDuration, true);
            FadeIn = false;
            sceneManagement.StartLoadScene();
        }

            if (img.canvasRenderer.GetAlpha() == 1)
            {
                Slider.active = true;
                TextTMP.active = true;
            }
    }
}
