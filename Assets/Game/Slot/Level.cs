using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

public class Level
{
    public string name = "New Level";
    public Dictionary<Vector3, Slot> map;

    public int columns;
    public int rows;

    public Level(int columns, int rows)
    {
        map = new Dictionary<Vector3, Slot>();

        this.columns = columns;
        this.rows = rows;
    }

    public void AddSlot(Slot slot)
    {
        if (!map.ContainsKey(slot.position))
        {
            map.Add(slot.position, slot);
        }
    }   
    
    public void ChangeSlotNumber(Slot newSlot)
    {
        Slot slot;
        if(map.TryGetValue(newSlot.position, out slot))
        {
            slot.number = newSlot.number;
        }
    }

    public void MakeEmptyLevel()
    {
        var gridColumns = this.columns;
        var gridRows = this.rows;

        for (int row = 0; row < gridRows; row++)
        {
            for (int column = 0; column < gridColumns; column++)
            {
                var hex = new Hex(column, row);
                var slot = new Slot(0, hex);

                AddSlot(slot);
            }
        }
    }

    public string SaveLevel()
    {
        List<Slot> slots = new List<Slot>();

        foreach (var slot in map.Values)
        {
            if (slot != null && slot.number >= 0)
            {
                slots.Add(slot);

            }
        }

        var str = JsonHelper.ToJson<Slot>(slots.ToArray());

        //str = str.Replace("\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0},", "");

        return str;
    }
}