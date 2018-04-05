using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class SlotEdge
{
    public Slot slot1;
    public Slot slot2;

    public int weight;

    public SlotEdge(Slot slot1, Slot slot2)
    {
        if (slot2.isNumber)
        {
            weight = slot2.number;
        }
    }
}
