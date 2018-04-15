using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Path
{
    public List<PathSlot> waypoints;

    public HashSet<Slot> waypointsHash;

    public PathSlot lastPoint;
    public PathSlot startPoint;
        
    public Path(Slot startPoint)
    {
        waypoints = new List<PathSlot>();
        waypointsHash = new HashSet<Slot>();

        var pathSlot = new PathSlot(startPoint);

        this.startPoint = pathSlot;
        this.lastPoint = pathSlot;

        if (startPoint.isNumber)
        {
            pathSlot.number = startPoint.number;
            pathSlot.sum = startPoint.number;
        }

        waypoints.Add(pathSlot);
        waypointsHash.Add(startPoint);
    }

    public Path(Path pathClone)
    {
        waypoints = new List<PathSlot>();
        waypoints.AddRange(pathClone.waypoints);

        waypointsHash = new HashSet<Slot>(pathClone.waypointsHash);       
        //waypoints.RemoveAt(waypoints.Count - 1);

        startPoint = pathClone.startPoint;
        lastPoint = pathClone.lastPoint;
        //lastPoint = new PathSlot(pathClone.GetLastPoint().slot);

        //waypoints.Add(lastPoint);
    }

    public bool AddPoint(Slot slot, bool mock = false)
    {
        var pathSlot = new PathSlot(slot);

        if (!waypointsHash.Contains(slot))
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

            pathSlot.previous = lastPoint;

            lastPoint.next = pathSlot;

            waypoints.Add(pathSlot);
            waypointsHash.Add(pathSlot.slot);

            lastPoint = pathSlot;

            return true;
        }

        return false;
    }
    
    public bool GoBack()
    {
        return RemovePoint(GetLastPoint());
    }

    public bool RemovePoint(PathSlot pathSlot)
    {
        if (waypointsHash.Contains(pathSlot.slot))
        {
            waypoints.Remove(pathSlot);
            waypointsHash.Remove(pathSlot.slot);
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

    public bool isSolution(Level level)
    {
        return waypoints.Count == level.slots.Length;
    }

    public int GetTotalPoints()
    {
        return lastPoint.sum;
    }
}
