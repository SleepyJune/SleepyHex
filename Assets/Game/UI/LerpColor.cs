using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using UnityEngine.UI;

public class LerpColor : MonoBehaviour
{
    public Image image;

    public int startingHue;

    float hue = 0;
    bool change = false;
    
    void Awake()
    {
        //renderer = GetComponent<Renderer>();

        hue = (startingHue / 360.0f) * 100;
    }

    void Update()
    {
        //renderer.material.color = Color.Lerp(c[0], c[1], t);

        var color = Color.HSVToRGB((hue%100)/100f, .6f, 1);
        //color.a = .2f;
        
        image.color = color;// Color.Lerp(currentColor, color, Time.deltaTime);

        hue += 20 * Time.deltaTime;
    }
}
