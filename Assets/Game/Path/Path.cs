using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Path
{
    public List<Slot> waypoints;

    Slot lastPoint;
    Slot startPoint;

    public Path(Slot startPoint)
    {
        waypoints = new List<Slot>();

        this.startPoint = startPoint;
        this.lastPoint = startPoint;
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

                if(lastPoint.number < slot.number)
                {
                    return false;
                }
            }
            
            waypoints.Add(slot);
            return true;
        }

        return false;
    }
}
