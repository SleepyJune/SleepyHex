using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

public class PathManager : MonoBehaviour
{
    [NonSerialized]
    public UIGameSlot selectedSlot;

    public void OnGameSlotPressed(UIGameSlot slot)
    {
        if (selectedSlot)
        {
            selectedSlot.uiSlot.anim.SetBool("selected", false);
        }

        selectedSlot = slot;
        selectedSlot.uiSlot.anim.SetBool("selected", true);
    }
}
