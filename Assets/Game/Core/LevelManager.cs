using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

public class LevelManager : LevelLoader
{
    public override bool isValidSlot(Slot slot)
    {
        return slot.number >= 0;
    }

    public override void InitUISlot(UISlot newSlot)
    {
        var gameSlot = newSlot.gameObject.AddComponent<UIGameSlot>();
        gameSlot.uiSlot = newSlot;
        gameSlot.pathManager = GameManager.instance.pathManager;
    }

    /*public override void Load(string path)
    {
        base.Load(path);

        if(level != null)
        {

        }
    }*/
}