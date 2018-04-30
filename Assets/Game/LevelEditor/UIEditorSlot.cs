using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using UnityEngine.EventSystems;

class UIEditorSlot : MonoBehaviour, IPointerDownHandler
{
    public UISlot uiSlot;
    
    public LevelEditor levelEditor;
    
    public void OnSlotPressed()
    {
        if (levelEditor)
        {
            levelEditor.OnEditorSlotPressed(this);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        OnSlotPressed();
    }
}
