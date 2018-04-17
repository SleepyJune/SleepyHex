using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

public class LevelSolver : ThreadedJob
{
    Level level;

    List<Path> solvedPaths;

    List<Slot> startingSlots;

    public DateTime startTime;

    int slotsVisited = 0;

    int numSlots;

    //Path bestPath;

    LevelSolution solution;

    int bestScore = 0;
    int worstScore = 0;

    float progressPercent = 0;

    bool abort = false;

    public LevelSolver(Level level)
    {
        this.level = level;

        this.solvedPaths = new List<Path>();
    }

    public override void Abort()
    {
        abort = true;
    }

    protected override void ThreadFunction()
    {
        Debug.Log("Solver Start");

        startTime = DateTime.Now;

        var slots = level.map.Values.Where(s => s.number >= 0);
        numSlots = slots.Count();

        startingSlots = slots.Where(s => s.isNumber).ToList();
        var numSlotsToProcess = startingSlots.Count();

        int slotsProcessed = 0;

        progressPercent = 0;

        foreach (var startPoint in startingSlots)
        {
            if (startPoint.isNumber)
            {
                var path = new Path(startPoint);
                ExploreNeighbour(path);

                slotsProcessed += 1;

                progressPercent = (float)slotsProcessed / numSlotsToProcess;
            }
        }

        if (abort)
        {
            Debug.Log("Aborted");
            return;
        }

        solvedPaths = solvedPaths.Where(p => p.waypoints.Count == numSlots)
                                 .OrderByDescending(p => p.GetTotalPoints()).ToList();

        TimeSpan solveTime = DateTime.Now.Subtract(startTime);

        Debug.Log("Solve time: " + solveTime.ToString());

        Debug.Log("Visited: " + slotsVisited);

        Debug.Log("Solutions: " + solvedPaths.Count);

        var bestPath = solvedPaths.FirstOrDefault();
        var worstPath = solvedPaths.LastOrDefault();

        int numSolutions = solvedPaths.Count;

        if (numSolutions > 0)
        {
            bestScore = bestPath.GetTotalPoints();
            worstScore = worstPath.GetTotalPoints();

            List<Vector3> waypoints = new List<Vector3>();
            foreach(var point in bestPath.waypoints)
            {
                waypoints.Add(point.slot.position);
            }

            int twoStarScore = (int)Math.Floor((bestScore + worstScore) / 2.0f);

            int numBestSolutions = solvedPaths.Where(p => p.GetTotalPoints() == bestScore).Count();

            solution = new LevelSolution()
            {
                bestScore = bestScore,
                twoStarScore = twoStarScore,
                worstScore = worstScore,
                numSolutions = numSolutions,
                numBestSolutions = numBestSolutions,
                bestPath = waypoints.ToArray(),
                version = level.version,
            };

            Debug.Log("Best Solutions: " + numBestSolutions);
        }

        //return bestPath;
    }

    public void ExploreNeighbour(Path path)
    {
        if (abort)// || DateTime.Now.Subtract(startTime).Seconds > 10)
        {
            return;
        }

        if(path.waypoints.Count == numSlots)
        {
            var points = path.GetTotalPoints();

            var newPath = new Path(path);
            solvedPaths.Add(newPath);
            return;
        }

        var lastPoint = path.lastPoint;

        /*if(CheckDeadStartingSlot(lastPoint.slot, path))
        {
            return;
        }*/

        if (lastPoint != null && lastPoint.slot.neighbours != null)
        {
            //HashSet<Slot> neighbourVisted = new HashSet<Slot>();

            foreach (var neighbour in lastPoint.slot.neighbours)
            {
                //neighbourVisted.Add(neighbour);

                if (path.AddPoint(neighbour))
                {
                    slotsVisited += 1;

                    if (CheckDeadNeighbour2(lastPoint.slot, neighbour, path))
                    {
                        path.GoBack();
                        continue;
                    }

                    /*if (CheckDeadNeighbour(lastPoint.slot, path, neighbourVisted))
                    {
                        continue;
                    }*/

                    //solvedPaths.Add(newPath);
                    ExploreNeighbour(path);

                    path.GoBack();
                }
            }
        }
    }

    public bool CheckDeadStartingSlot(Slot slot, Path newPath)
    {
        //contains all slots in path that is not the head

        foreach(var start in startingSlots)
        {
            if(start.neighbours != null
                && !newPath.waypointsHash.Contains(start))
            {
                if (!start.neighbours.Any(n => n == slot || !newPath.waypointsHash.Contains(n)))
                {
                    return true;
                }
            }
        }

        return false;
    }

    public bool CheckDeadNeighbour2(Slot slot, Slot head, Path newPath)
    {
        foreach (var otherNeighbour in slot.neighbours)
        {
            if (otherNeighbour.neighbours != null
                && !head.neighbours.Contains(otherNeighbour)
                && !newPath.waypointsHash.Contains(otherNeighbour))
            {
                if (!otherNeighbour.neighbours.Any(n => !newPath.waypointsHash.Contains(n)))
                {
                    return true;
                }
            }
        }

        return false;
    }

    public bool CheckDeadNeighbour(Slot slot, Path newPath, HashSet<Slot> neighbourVisted)
    {
        foreach (var otherNeighbour in slot.neighbours)
        {
            if (otherNeighbour.neighbours != null
                && !neighbourVisted.Contains(otherNeighbour)
                && !newPath.waypointsHash.Contains(otherNeighbour))
            {
                if (!otherNeighbour.neighbours.Any(n => !newPath.waypointsHash.Contains(n)))
                {
                    return true;
                }
            }
        }

        return false;
    }

    public float GetProgress()
    {
        return progressPercent;
    }

    public LevelSolution GetSolution()
    {
        return solution;
    }

    public List<Path> GetSolvedPaths()
    {
        var ret = solvedPaths.Where(p => p.GetTotalPoints() == bestScore).ToList();

        solvedPaths = null; //release from memory
        
        return ret;
    }
}