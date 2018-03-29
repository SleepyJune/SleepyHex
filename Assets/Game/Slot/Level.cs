using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;

using UnityEngine;

[Serializable]
public class Level
{
    public string levelName = "New Level";

    public Slot[] slots;

    public int columns;
    public int rows;

    [NonSerialized]
    public Dictionary<Vector3, Slot> map;

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
        if (map.TryGetValue(newSlot.position, out slot))
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

        slots = map.Values.ToArray();
    }

    public void AddSlotsToMap()
    {
        if(map == null)
        {
            map = new Dictionary<Vector3, Slot>();
        }

        foreach(var slot in slots)
        {
            AddSlot(slot);
        }
    }

    public string SaveLevel()
    {
        slots = map.Values.ToArray();
        return JsonUtility.ToJson(this);
    }

    public static Level LoadLevel(string path)
    {
        if (File.Exists(path))
        {
            string str = File.ReadAllText(path);

            var level = JsonUtility.FromJson<Level>(str);
            level.AddSlotsToMap();

            return level;
        }

        return null;
    }
}