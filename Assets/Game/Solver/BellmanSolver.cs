using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

public class BellmanSolver : ThreadedJob
{
    bool abort = false;

    Level level;

    Dictionary<int, PathEdge> pathEdges;

    int slotMaxNumber = 0;
    int slotMinNumber = 999;

    public Dictionary<int, PathNode> nodeTable = new Dictionary<int, PathNode>();

    public BellmanSolver(Level level)
    {
        this.level = level;

        pathEdges = new Dictionary<int, PathEdge>();

        GetNumberRange();
        InitializeNodes();
        InitializeNeighbours();
    }

    public void GetNumberRange()
    {
        foreach (var slot in level.map.Values.Where(n => n.number >= 0))
        {
            if (slot.isNumber)
            {
                slotMaxNumber = Math.Max(slotMaxNumber, slot.number);
                slotMinNumber = Math.Min(slotMinNumber, slot.number);
            }
        }
    }

    public void InitializeNodes()
    {
        foreach (var slot in level.map.Values.Where(n => n.number >= 0))
        {
            for (int i = 0; i < 2; i++)
            {
                var isDescending = i == 0 ? false : true;

                if (!slot.isBlank)
                {
                    var newNode = new PathNode(slot, slot.number, isDescending);
                    nodeTable.Add(newNode.GetHashCode(), newNode);
                }
                else
                {
                    for (int number = slotMinNumber; number <= slotMaxNumber; i++)
                    {
                        var newNode = new PathNode(slot, number, isDescending);
                        nodeTable.Add(newNode.GetHashCode(), newNode);
                    }
                }
            }
        }
    }

    public void InitializeNeighbours()
    {
        foreach (var current in nodeTable.Values)
        {
            foreach (var slot in current.slot.neighbours)
            {
                if (slot.isNumber)
                {
                    if (current.isDescending && current.number < slot.number)
                    {
                        continue;
                    }
                    else if (!current.isDescending && current.number > slot.number)
                    {
                        continue;
                    }
                }

                if (slot.number == (int)SpecialSlot.Reverse)
                {
                    var node = nodeTable[PathNode.GetHashCode(slot, (int)SpecialSlot.Reverse, !current.isDescending)];
                    if (node != null)
                    {
                        current.neighbours.Add(node);
                    }
                }

                if (slot.isNumber)
                {
                    var node = nodeTable[PathNode.GetHashCode(slot, slot.number, current.isDescending)];
                    if(node != null)
                    {
                        current.neighbours.Add(node);
                    }
                }
                else if (slot.number == (int)SpecialSlot.Blank)
                {
                    var node = nodeTable[PathNode.GetHashCode(slot, current.number, current.isDescending)];
                    if (node != null)
                    {
                        current.neighbours.Add(node);
                    }
                }
            }
        }
    }
    
}