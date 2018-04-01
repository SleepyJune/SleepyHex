using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Path
{
    public List<PathSlot> waypoints;
    
    PathSlot lastPoint;
    PathSlot startPoint;

    public Path(Slot startPoint)
    {
        waypoints = new List<PathSlot>();

        var pathSlot = new PathSlot(startPoint);

        this.startPoint = pathSlot;
        this.lastPoint = pathSlot;

        if (startPoint.isNumber)
        {
            pathSlot.number = startPoint.number;
            pathSlot.sum = startPoint.number;
        }

        waypoints.Add(pathSlot);
    }

    public bool AddPoint(Slot slot)
    {
        var pathSlot = new PathSlot(slot);

        if (!waypoints.Contains(pathSlot))
        {
            if (!lastPoint.slot.isNeighbour(slot))
            {
                return false;
            }

            if (slot.isNumber)
            {
                if (lastPoint.isDescending && lastPoint.number < slot.number)
                {
                    return false;
                }
                else if (!lastPoint.isDescending && lastPoint.number > slot.number)
                {
                    return false;
                }
            }

            pathSlot.isDescending = lastPoint.isDescending;

            if (slot.number == (int)SpecialSlot.Reverse)
            {
                pathSlot.isDescending = !pathSlot.isDescending;
            }

            if (slot.isNumber)
            {
                pathSlot.number = slot.number;
                pathSlot.sum = lastPoint.sum + slot.number;
            }
            else
            {
                if (slot.number == (int)SpecialSlot.Blank)
                {
                    pathSlot.number = lastPoint.number;
                    pathSlot.sum = lastPoint.sum + lastPoint.number;
                }
                else
                {
                    pathSlot.number = lastPoint.number;
                    pathSlot.sum = lastPoint.sum;
                }                
            }

            lastPoint.next = pathSlot;
            pathSlot.previous = lastPoint;

            waypoints.Add(pathSlot);
            lastPoint = pathSlot;

            return true;
        }

        return false;
    }

    public bool RemovePoint(PathSlot pathSlot)
    {
        if (waypoints.Contains(pathSlot))
        {
            waypoints.Remove(pathSlot);
            lastPoint = waypoints.LastOrDefault();

            return true;
        }

        return false;
    }

    public PathSlot GetLastPoint()
    {
        return lastPoint;
    }

    public PathSlot GetPreviousPoint()
    {
        return lastPoint.previous;
    }

    public int GetSum()
    {
        return lastPoint.sum;
    }
}
