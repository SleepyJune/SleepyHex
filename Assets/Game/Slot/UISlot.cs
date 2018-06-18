using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using UnityEngine.UI;

public class UISlot : MonoBehaviour
{
    public Text text;

    public Animator anim;

    public Image background;

    [NonSerialized]
    public Slot slot;

    [NonSerialized]
    public int blankNumber;

    public Image buttonImage;
    
    bool isFilled;

    int iconState;

    Canvas canvas;
    RectTransform rect;

    [NonSerialized]
    public UIGameSlot gameSlot;

    void Start()
    {
        buttonImage.alphaHitTestMinimumThreshold = .05f;

        rect = GetComponent<RectTransform>();
        canvas = transform.root.GetComponent<Canvas>();

        gameSlot = GetComponent<UIGameSlot>();

        if (slot != null)
        {
            SetNumber(slot.number);
            ToggleText(false);
        }
    }

    public Vector2 GetScreenPosition()
    {        
        var pos = rect.anchoredPosition;
        return RectTransformUtility.PixelAdjustPoint(pos, canvas.transform, canvas);
    }

    public void SetBackgroundSaturation(float saturation)
    {
        float h;
        float s;
        float v;

        Color.RGBToHSV(background.color, out h, out s, out v);

        s = saturation / 255;

        Debug.Log(s);

        var color = Color.HSVToRGB(h, s, v);
        
        //color.a = .4f;

        background.color = color;
    }

    public void SetIconState(int state)
    {
        if(iconState != state)
        {
            anim.SetInteger("iconState", state);
            iconState = state;
        }
    }

    public void SetFilled(bool filled)
    {        
        if(isFilled != filled)
        {
            anim.SetBool("filled", filled);
            isFilled = filled;
        }
    }

    public void SetNumber(int number)
    {        
        slot.number = number;

        if (slot.isNumber && !slot.hideNumber)
        {
            text.text = number.ToString();
        }
        else
        {
            text.text = "";
        }

        anim.SetInteger("number", number);
    }

    public void SetBlankNumber(int number)
    {
        blankNumber = number;
        anim.SetInteger("number", number);
    }

    public void ToggleText(bool toggle = true)
    {
        if (toggle)
        {
            slot.hideNumber = !slot.hideNumber;
        }

        if (slot.hideNumber || !slot.isNumber)
        {
            text.text = "";
        }
        else
        {
            text.text = slot.number.ToString();
        }
    }
}
