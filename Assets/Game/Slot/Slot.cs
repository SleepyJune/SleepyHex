using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

public enum SpecialSlot
{
    Blank = 0,
    Reverse = 10,
}

public class Slot
{    
    public int number;

    public Hex hexPosition;
    public Vector3 position;

    public Slot(int number)
    {
        this.number = number;
    }

    public Slot(int number, Vector3 position)
    {
        this.number = number;
        this.position = position;
    }

    public static bool operator ==(Slot value1, Slot value2)
    {
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
