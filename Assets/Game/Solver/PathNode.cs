using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

public class PathNode : IEquatable<PathNode>
{
    public Slot slot;

    public int number = 0;
    
    public bool isDescending = true;

    public List<PathNode> neighbours = new List<PathNode>();

    public PathNode parent;
    //public double gScore = 9999;
    //public double fScore = 9999;

    public int distance = 9999;

    public PathNode(Slot slot, int number, bool isDescending)
    {
        this.slot = slot;
        this.number = number;
        this.isDescending = isDescending;
    }

    public override bool Equals(object obj)
    {
        if (obj is PathNode)
        {
            return Equals((PathNode)this);
        }

        return false;
    }

    public bool Equals(PathNode node)
    {
        return
                this.slot == node.slot
             && this.number == node.number
             && this.isDescending == node.isDescending;
    }

    public override int GetHashCode()
    {
        return (this.isDescending?1:0) * 100 * 100 * 10 +
               this.number * 100 * 100 +
               this.slot.hexPosition.GetHashCode();
    }

    public static int GetHashCode(Slot slot, int number, bool isDescending)
    {
        return (isDescending ? 1 : 0) * 100 * 100 * 10 +
                number * 100 * 100 +
                slot.hexPosition.GetHashCode();
    }
}
