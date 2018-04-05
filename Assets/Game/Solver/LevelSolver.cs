using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

public class LevelSolver
{
    Level level;

    List<Path> solvedPaths;

    public LevelSolver(Level level)
    {
        this.level = level;

        this.solvedPaths = new List<Path>();
    }

    public void Solve()
    {
        foreach(var startPoint in level.map.Values)
        {
            var path = new Path(startPoint);

            ExploreNeighbour(path);
        }
    }

    public void ExploreNeighbour(Path path)
    {
        var lastPoint = path.GetLastPoint();
        
        foreach(var neighbour in lastPoint.slot.neighbours)
        {
            if (path.AddPoint(neighbour))
            {
                var newPath = new Path(path);
                solvedPaths.Add(newPath);

                path.GoBack();

                ExploreNeighbour(newPath);
            }
        }
    }
}
