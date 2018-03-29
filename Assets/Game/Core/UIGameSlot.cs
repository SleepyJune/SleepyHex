using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using UnityEngine.EventSystems;

public class UIGameSlot : MonoBehaviour, IPointerDownHandler
{
    public UISlot uiSlot;

    public PathManager pathManager;

    public void OnSlotPressed()
    {
        if (pathManager)
        {
            pathManager.OnGameSlotPressed(this);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        OnSlotPressed();
    }
}
