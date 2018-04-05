using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

public class LevelSolver2
{
    Level level;

    List<Path> solvedPaths;

    DateTime startTime;

    int slotsVisited = 0;

    public LevelSolver2(Level level)
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

        solvedPaths = solvedPaths.OrderByDescending(p => p.GetSum()).ToList();

        Debug.Log("Visited: " + slotsVisited);

        return solvedPaths.FirstOrDefault();
    }

    public void ExploreNeighbour(Path path)
    {
        if (DateTime.Now.Subtract(startTime).Seconds > 10)
        {
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
