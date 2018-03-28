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

    public Slot slot;
    
    void Start()
    {
        var number = slot.number;

        SetNumber(number);
    }

    public void SetNumber(int number)
    {
        if (number > 0 && number <= 9)
        {
            text.text = number.ToString();
        }
        else
        {
            text.text = "";
        }

        anim.SetInteger("number", number);
    }
}
