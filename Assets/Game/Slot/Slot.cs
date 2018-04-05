using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

public enum SpecialSlot
{
    Empty = -1,
    Blank = 0,
    Reverse = 10,
}

[System.Serializable]
public class Slot
{    
    public int number;

    public Hex hexPosition;
    public Vector3 position;

    [NonSerialized]
    public HashSet<Slot> neighbours;
    
    public bool isNumber
    {
        get
        {
            return number > 0 && number < 10;
        }
    }

    public Slot(int number)
    {
        this.number = number;
        this.hexPosition = new Hex(0, 0);
        this.position = Vector3.zero;
    }

    public Slot(int number, Hex position)
    {
        this.number = number;
        this.hexPosition = position;
        this.position = position.ConvertCube();
    }

    public void AddNeighbour(Slot neighbour)
    {
        if (neighbour.number >= 0) //!neighbours.Contains(neighbour) && this != neighbour)
        {
            neighbours.Add(neighbour);
        }
    }

    public bool isNeighbour(Slot slot)
    {
        if (neighbours.Contains(slot))
        {
            return true;
        }

        return false;
    }

    public static bool operator ==(Slot value1, Slot value2)
    {
        if (object.ReferenceEquals(value1, null))
        {
            return object.ReferenceEquals(value2, null);
        }

        if (object.ReferenceEquals(value2, null))
        {
            return object.ReferenceEquals(value1, null);
        }

        return value1.position == value2.position;
    }

    public static bool operator !=(Slot value1, Slot value2)
    {
        return !(value1 == value2);
    }

    public override bool Equals(object obj)
    {
        return (obj is Slot) ? this == (Slot)obj : false;
    }

    public bool Equals(Slot other)
    {
        return this == other;
    }

    public override int GetHashCode()
    {
        return position.GetHashCode();
    }
}
