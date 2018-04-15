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

    void Start()
    {
        buttonImage.alphaHitTestMinimumThreshold = .05f;

        if (slot != null)
        {
            SetNumber(slot.number);
            ToggleText(false);
        }
    }

    public void SetFilled(bool filled)
    {        
        if(isFilled != filled)
        {
            anim.SetBool("filled", filled);
            this.isFilled = filled;
        }
    }

    public void SetNumber(int number)
    {        
        slot.number = number;

        if (number > 0)// && number <= 9)
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

        if (slot.hideNumber || slot.number <= 0)
        {
            text.text = "";
        }
        else
        {
            text.text = slot.number.ToString();
        }
    }
}
