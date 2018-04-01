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

        var maxWidth = 200;
        var maxHeight = 250;

        for (int row = (int)Math.Round(-gridRows/2f); row < gridRows; row++)
        {
            for (int column = (int)Math.Round(-gridColumns / 2f); column < gridColumns; column++)
            {
                var hex = new Hex(column, row);

                var worldPos = hex.GetWorldPos();

                if (worldPos.x < 0 || worldPos.y < 0 || worldPos.x > maxWidth || worldPos.y > maxHeight)
                {
                    continue;
                }

                var slot = new Slot(-1, hex);

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

        //Add neighbours
        foreach(var slot in map.Values)
        {
            foreach (var direction in VectorExtensions.directions)
            {
                var pos = slot.position + direction;
                
                Slot neighbour;
                if(map.TryGetValue(pos, out neighbour))
                {
                    slot.AddNeighbour(neighbour); //will be checked in add neighbour
                }                
            }            
        }
    }

    public string SaveLevel()
    {
        slots = map.Values.ToArray();
        return JsonUtility.ToJson(this);
    }

    public static Level LoadLevel(LevelTextAsset levelText)
    {
        if (levelText != null)
        {
            string str = levelText.text;

            var level = JsonUtility.FromJson<Level>(str);
            level.AddSlotsToMap();

            return level;
        }

        return null;
    }
}