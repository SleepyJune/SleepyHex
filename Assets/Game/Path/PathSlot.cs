using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class PathSlot
{
    public Slot slot;

    public PathSlot previous;
    public PathSlot next;

    public int number = 0;
    public int sum = 0;

    public int reverseUsed = 0;

    public bool isDescending = true;

    public PathSlot(Slot slot)
    {
        this.slot = slot;
    }

    public static bool operator ==(PathSlot value1, PathSlot value2)
    {
        if (object.ReferenceEquals(value1, null))
        {
            return object.ReferenceEquals(value2, null);
        }

        if (object.ReferenceEquals(value2, null))
        {
            return object.ReferenceEquals(value1, null);
        }

        return value1.slot == value2.slot;
    }

    public static bool operator !=(PathSlot value1, PathSlot value2)
    {
        return !(value1 == value2);
    }

    public override bool Equals(object obj)
    {
        return (obj is PathSlot) ? this == (PathSlot)obj : false;
    }

    public bool Equals(PathSlot other)
    {
        return this == other;
    }

    public override int GetHashCode()
    {
        return slot.GetHashCode();
    }
}
