using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

public class LevelSolver : ThreadedJob
{
    Level level;

    List<Path> solvedPaths;

    DateTime startTime;

    int slotsVisited = 0;

    Path bestPath;

    float progressPercent = 0;

    bool abort = false;

    public LevelSolver(Level level)
    {
        this.level = level;

        this.solvedPaths = new List<Path>();
    }

    public void Solve()
    {

    }

    protected override void OnFinished()
    {

    }

    public override void Abort()
    {
        abort = true;

        if(IsDone == false)
        {
            Debug.Log("Aborting");
        }
    }

    protected override void ThreadFunction()
    {
        Debug.Log("Solver Start");

        startTime = DateTime.Now;

        var slots = level.map.Values.Where(s => s.number >= 0);

        var numSlots = slots.Count();
        int slotsProcessed = 0;

        progressPercent = 0;

        foreach (var startPoint in slots)
        {
            if (startPoint.isNumber)
            {
                var path = new Path(startPoint);
                ExploreNeighbour(path);

                slotsProcessed += 1;

                progressPercent = (float)slotsProcessed / numSlots;
            }
        }

        solvedPaths = solvedPaths.OrderByDescending(p => p.waypoints.Count == numSlots)
                                 .ThenByDescending(p => p.GetSum()).ToList();

        if (DateTime.Now.Subtract(startTime).Seconds > 10)
        {
            Debug.Log("Solving Timed out");
        }
        
        Debug.Log("Visited: " + slotsVisited);

        Debug.Log("Max slots: " + numSlots);

        bestPath = solvedPaths.FirstOrDefault();

        //return bestPath;
    }

    public void ExploreNeighbour(Path path)
    {
        if (abort)// || DateTime.Now.Subtract(startTime).Seconds > 10)
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

    public float GetProgress()
    {
        return progressPercent;
    }

    public Path GetBestPath()
    {
        return bestPath;
    }
}
