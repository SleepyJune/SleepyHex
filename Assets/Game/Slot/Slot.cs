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
    PlusOne = 11,
    MinusOne = 12,
}

public enum SlotType
{
    Blank,
    Number,
    Reverse,
    PlusOne,
    MinusOne,
}

[System.Serializable]
public class Slot
{    
    public int number;

    public bool hideNumber;

    public Hex hexPosition;
    public Vector3 position;

    [NonSerialized]
    public HashSet<Slot> neighbours;
   
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

    public Slot(Vector3 position)
    {
        this.position = position;
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

    public bool isNumber
    {
        get
        {
            return number > 0 && number < 10;
        }
    }

    public bool isBlank
    {
        get
        {
            return number == 0;
        }
    }

    public bool isBlankFill
    {
        get
        {
            return number == 0 || number == 11 || number == 12;
        }
    }

    public bool isAddFill
    {
        get
        {
            return number == 11 || number == 12;
        }
    }

    public bool isReverse
    {
        get
        {
            return number == 10;
        }
    }

    public SlotType slotType
    {
        get
        {
            if (isNumber)
            {
                return SlotType.Number;
            }
            else if(isReverse)
            {
                return SlotType.Reverse;
            }
            else
            {
                return SlotType.Blank;
            }
        }
    }
}
