using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Path
{
    public List<Slot> waypoints;

    public int sum = 0;

    Slot lastPoint;
    Slot startPoint;

    int lastNumber = 0;

    bool isDescending = true;


    public Path(Slot startPoint)
    {
        waypoints = new List<Slot>();

        this.startPoint = startPoint;
        this.lastPoint = startPoint;

        sum = startPoint.isNumber ? startPoint.number : 0;

        lastNumber = lastPoint.number;

        waypoints.Add(startPoint);
    }

    public bool AddPoint(Slot slot)
    {
        if (!waypoints.Contains(slot))
        {
            if(lastPoint != null)
            {
                if (!lastPoint.isNeighbour(slot))
                {
                    return false;
                }

                if (slot.number > 0 && slot.number < 10)
                {
                    if (isDescending && lastNumber < slot.number)
                    {
                        return false;
                    }
                    else if(!isDescending && lastNumber > slot.number)
                    {
                        return false;
                    }
                }
            }
            
            if(slot.number == (int)SpecialSlot.Reverse)
            {
                isDescending = !isDescending;
            }

            waypoints.Add(slot);
            lastPoint = slot;

            if (slot.number > 0 && slot.number < 10)
            {
                lastNumber = slot.number;
                sum += slot.number;
            }
            return true;
        }

        return false;
    }
}
