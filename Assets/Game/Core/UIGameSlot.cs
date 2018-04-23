using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using UnityEngine.EventSystems;

public class UIGameSlot : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    public UISlot uiSlot;

    public PathManager pathManager;

    public bool isSelected = false;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (pathManager)
        {
            pathManager.OnGameSlotPressed(this);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isSelected = true;

        if (pathManager)
        {
            pathManager.OnGameSlotEnter(this);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isSelected = false;
    }
}
