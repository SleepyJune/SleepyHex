using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

public class PuzzleRating
{
    public static float RatePuzzle(Level level, int slotsVisited)
    {
        if(level != null && level.hasSolution)
        {
            level.AddSlotsToMap();

            var validSlots = level.slots.Where(s => s.number >= 0);

            var uniqueNums = validSlots.Where(s => s.isNumber).Select(s => s.number).Distinct().Count();
            var numReverse = validSlots.Where(s => s.number == 10).Count();
            var numSlots = validSlots.Count();
            var avgNeighbours = Math.Round(validSlots.Average(s => s.neighbours.Count),1);

            int sections = SimulateSolution(level);

            Debug.Log("Neighbours: " + avgNeighbours);
            Debug.Log("Numbers: " + uniqueNums);
            Debug.Log("Slots: " + numSlots);
            Debug.Log("Sections: " + sections);

            var rating = Math.Log10(slotsVisited * (float)sections);

            Debug.Log("Rating: " + rating);

            return (float)Math.Round(rating,2);
        }

        return -1;
    }

    public static int SimulateSolution(Level level)
    {
        var waypoints = level.solution.bestPath;
        
        Path path = null;

        int sections = 1;

        Vector3 direction = Vector3.zero;
        
        foreach(var point in waypoints)
        {
            var slot = level.map[point];

            if(path == null)
            {
                path = new Path(slot);
                continue;
            }

            var last = path.GetLastPoint();                                              
                        
            path.AddPoint(slot);

            var current = path.GetLastPoint();

            if (last.number != current.number)
            {
                sections += 1;
            }
            else if (slot.slotType != last.slot.slotType)
            {
                sections += 1;
            }

            //direction

            /*if(direction != Vector3.zero)
            {
                var pos = last.slot.position + direction;

                if (level.map.ContainsKey(pos))
                {
                    var possibleRoute = level.map[pos];
                    if (!path.waypointsHash.Contains(possibleRoute) && possibleRoute != current.slot)
                    {
                        Debug.Log("turn");
                    }
                }
            }

            direction = (current.slot.position - last.slot.position);*/

        }
            return sections;       
    }
}
