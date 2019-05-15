using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AutoslideSlider : MonoBehaviour
{
    public float Increment;

    Slider slider;
    // Start is called before the first frame update
    void Start()
    {
        slider = GetComponent<Slider>();
    }

    // Update is called once per frame
    void Update()
    {
        slider.value += Increment;
        if(slider.value == slider.maxValue || slider.value == slider.minValue)
        {
            Increment *= -1;
        }
    }
}
