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

            
        }
    }

    public void ExploreNeighbour(Path path)
    {
        var lastPoint = path.GetLastPoint();
        
        foreach(var neighbour in lastPoint.slot.neighbours)
        {
            

            //if(path.AddPoint())
        }
    }
}
