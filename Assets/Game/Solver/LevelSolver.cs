using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

public class LevelSolver
{
    Level level;

    List<Path> solvedPaths;

    DateTime startTime;

    int slotsVisited = 0;

    Path bestPath;

    public LevelSolver(Level level)
    {
        this.level = level;

        this.solvedPaths = new List<Path>();
    }

    public Path Solve()
    {
        startTime = DateTime.Now;

        foreach(var startPoint in level.map.Values)
        {
            if(startPoint.number >= 0)
            {
                var path = new Path(startPoint);
                ExploreNeighbour(path);
            }
        }

        var numSlots = level.map.Values.Sum(s => s.number >= 0 ? 1 : 0);

        solvedPaths = solvedPaths.OrderByDescending(p => p.waypoints.Count == numSlots)
                                 .ThenByDescending(p => p.GetSum()).ToList();

        Debug.Log("Visited: " + slotsVisited);

        Debug.Log("Max slots: " + numSlots);

        bestPath = solvedPaths.FirstOrDefault();

        return bestPath;
    }

    public void ExploreNeighbour(Path path)
    {
        if (DateTime.Now.Subtract(startTime).Seconds > 10)
        {
            Debug.Log("Solving Timed out");
            return;
        }

        var lastPoint = path.GetLastPoint();

        if (lastPoint != null && lastPoint.slot.neighbours != null)
        {            
            foreach (var neighbour in lastPoint.slot.neighbours)
            {
                if (path.AddPoint(neighbour))
                {
                    slotsVisited += 1;

                    var newPath = new Path(path);
                    solvedPaths.Add(newPath);

                    path.GoBack();

                    ExploreNeighbour(newPath);
                }
            }
        }
    }
}
