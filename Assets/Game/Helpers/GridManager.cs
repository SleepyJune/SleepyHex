using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

public class GridManager
{
    List<UISlot> uiSlots;

    Dictionary<Vector3, UISlot> uiSlotDatabase;

    Level level;

    float xMin = 9999, xMax = 0, yMin = 9999, yMax = 0;

    float slotSize = 40;

    public GridManager()
    {
        uiSlotDatabase = new Dictionary<Vector3, UISlot>();
        uiSlots = new List<UISlot>();
    }

    public void DeleteAllSlots()
    {
        foreach(var slot in uiSlots)
        {
            GameObject.Destroy(slot.gameObject);
        }
    }

    public UISlot GetUISlot(Vector3 pos)
    {
        return uiSlotDatabase[pos];
    }

    public IEnumerable<UISlot> GetUISlots()
    {
        return uiSlotDatabase.Values;
    }

    void SetBoardDimension(UISlot slot)
    {
        float buffer = 45 / 2f + 10;
                
        xMin = Math.Min(xMin, slot.transform.localPosition.x - buffer);
        xMax = Math.Max(xMax, slot.transform.localPosition.x + buffer);
        yMin = Math.Min(yMin, slot.transform.localPosition.y - buffer);
        yMax = Math.Max(yMax, slot.transform.localPosition.y + buffer);
    }

    float GetBoardScaling()
    {        
        float minScaling = .6f;
        float maxScaling = 2f;

        /*float xMin = 9999, xMax = 0, yMin = 9999, yMax = 0;

        foreach (var uiSlot in uiSlots)
        {
            var slot = uiSlot.slot;

            xMin = Math.Min(xMin, slot.hexPosition.col - .5f);
            xMax = Math.Max(xMax, slot.hexPosition.col + .5f);
            yMin = Math.Min(yMin, slot.hexPosition.row - .5f);
            yMax = Math.Max(yMax, slot.hexPosition.row + .5f);
        }*/

        float maxColSize = xMax - xMin;
        float maxRowSize = yMax - yMin;

        float xScaling = 270 / (maxColSize);
        float yScaling = 300 / (maxRowSize);

        //Debug.Log("Yscaling: " + yScaling);

        float boardScaling = Math.Min(xScaling, yScaling);
        boardScaling = Math.Max(minScaling, boardScaling);
        boardScaling = Math.Min(maxScaling, boardScaling);

        return boardScaling;
    }

    public void AdjustBoard(Transform slotList)
    {
        var slotParent = slotList.parent;

        var boardScaling = GetBoardScaling();

        slotParent.localScale = new Vector3(boardScaling, boardScaling, boardScaling);

        float xCenter = xMin + (xMax - xMin) / 2;
        float yCenter = yMin + (yMax - yMin) / 2;

        float xStart = -(xCenter) * boardScaling;
        float yStart = -(yCenter) * boardScaling;


        //450, 900 - 150, 324

        slotParent.localPosition = new Vector3(xStart, yStart, 0);

        //Debug.Log()

    }

    public void MakeGrid(Level level, UISlot slotPrefab, Transform slotParent, LevelLoader levelLoader)
    {
        foreach (Transform child in slotParent)
        {
            GameObject.Destroy(child.gameObject);
        }

        this.level = level;

        foreach (var slot in level.map.Values)
        {
            if(levelLoader != null)
            {
                if (!levelLoader.isValidSlot(slot))
                {
                    continue;
                }
            }

            var newSlot = GameObject.Instantiate(slotPrefab, slotParent);
            var worldPos = slot.hexPosition.GetWorldPos();
            newSlot.transform.localPosition = worldPos;
            newSlot.slot = slot;

            SetBoardDimension(newSlot);

            uiSlots.Add(newSlot);

            uiSlotDatabase.Add(slot.position, newSlot);

            if (levelLoader != null)
            {
                levelLoader.InitUISlot(newSlot);
            }
        }

        AdjustBoard(slotParent);
    }
}