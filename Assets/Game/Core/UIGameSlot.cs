using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using UnityEngine.EventSystems;

public class UIGameSlot : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler
{
    public UISlot uiSlot;

    public PathManager pathManager;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (pathManager)
        {
            pathManager.OnGameSlotPressed(this);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (pathManager)
        {
            pathManager.OnGameSlotEnter(this);
        }
    }
}
