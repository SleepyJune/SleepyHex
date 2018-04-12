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

    [NonSerialized]
    public Slot slot;

    public Image buttonImage;

    void Start()
    {
        buttonImage.alphaHitTestMinimumThreshold = .5f;

        if (slot != null)
        {
            SetNumber(slot.number);
            ToggleText(false);
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
