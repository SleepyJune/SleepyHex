using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

public class GridManager
{
    int column;
    int row;

    float slotHeight = 44;
    float slotWidth = 50;

    Vector3 initialPos;

    List<UISlot> uiSlots;

    public GridManager(int column, int row)
    {
        this.column = column;
        this.row = row;

        uiSlots = new List<UISlot>();

        CalculateInitialPos();
    }

    public void CalculateInitialPos()
    {
        /*initialPos = new Vector3(-slotWidth * row / 2f + slotWidth / 2,
                                 slotHeight * column / 2f - slotHeight / 2f, 0);*/
    }

    public Vector3 CalculateWorldPos(Hex hexPos)
    {
        float offset = 0;

        if(hexPos.row % 2 != 0)
        {
            offset = slotHeight / 2;
        }

        float x = initialPos.x + hexPos.row * slotWidth * .75f;
        float z = initialPos.z - hexPos.col * slotHeight + offset;

        return new Vector3(x, z, 0);
    }
        
    public void DeleteAllSlots()
    {
        foreach(var slot in uiSlots)
        {
            GameObject.Destroy(slot.gameObject);
        }
    }

    public void MakeGrid(Level level, UISlot slotPrefab, Transform slotParent, LevelLoader levelLoader)
    {
        foreach(var slot in level.map.Values)
        {
            if(levelLoader != null)
            {
                if (!levelLoader.isValidSlot(slot))
                {
                    continue;
                }
            }

            var newSlot = GameObject.Instantiate(slotPrefab, slotParent);
            var worldPos = CalculateWorldPos(slot.hexPosition);
            newSlot.transform.localPosition = worldPos;
            newSlot.slot = slot;

            uiSlots.Add(newSlot);

            if (levelLoader != null)
            {
                levelLoader.InitUISlot(newSlot);
            }
        }
    }
}