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
    int pathsDenied = 0;

    int numSlots;
    int numReverses;

    //Path bestPath;

    LevelSolution solution;

    int bestScore = 0;
    int worstScore = 0;

    int slotsProcessed = 0;
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
        numReverses = slots.Where(s => s.number == 10).Count();

        startingSlots = slots.Where(s => s.isNumber).OrderBy(s=> s.number).ToList();
        var numSlotsToProcess = startingSlots.Count();
                
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

        Debug.Log("Denied: " + pathsDenied);

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

        if (lastPoint != null && lastPoint.slot.neighbours != null)
        {
            foreach (var neighbour in lastPoint.slot.neighbours)
            {
                if (!path.waypointsHash.Contains(neighbour))
                {
                    slotsVisited += 1;

                    if (path.AddPoint(neighbour))
                    {
                        if (CheckDeadNeighbour2(lastPoint.slot, neighbour, path))
                        {
                            path.GoBack();
                            continue;
                        }                  

                        if (!neighbour.isBlank && CheckDeadNumSlot(path))
                        {
                            path.GoBack();
                            continue;
                        }

                        ExploreNeighbour(path);

                        path.GoBack();
                    }
                    else
                    {
                        pathsDenied += 1;
                    }
                }
            }
        }
    }

    public bool CheckDeadNumSlot(Path newPath)
    {
        var slot = newPath.lastPoint;
        int currentNumber = newPath.lastPoint.number;

        if (newPath.lastPoint.isDescending)
        {
            for (int i = startingSlots.Count-1; i >= slotsProcessed; i--) //counting down
            {
                var numSlot = startingSlots[i];

                if (numSlot.number <= currentNumber)
                {
                    break;
                }

                if (!newPath.waypointsHash.Contains(numSlot))
                {
                    if (numReverses <= slot.reverseUsed)
                    {
                        return true;
                    }
                }
            }
        }
        else
        {
            for (int i = slotsProcessed; i < startingSlots.Count; i++) //counting up
            {
                var numSlot = startingSlots[i];

                if (numSlot.number >= currentNumber)
                {
                    break;
                }

                if (!newPath.waypointsHash.Contains(numSlot))
                {
                    if (numReverses <= slot.reverseUsed)
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    public bool CheckDeadNeighbour3(Slot slot, Slot head, Path path)
    {
        foreach (var otherNeighbour in slot.neighbours)
        {
            if (!head.neighbours.Contains(otherNeighbour)
                && !path.waypointsHash.Contains(otherNeighbour))
            {
                HashSet<Slot> checkedSlots = new HashSet<Slot>();

                if (CheckDeadNeighbour3_Helper(otherNeighbour, checkedSlots, path))
                {
                    return true;
                }
            }
        }

        return false;
    }

    public bool CheckDeadNeighbour3_Helper(Slot otherNeighbour, HashSet<Slot> checkedSlots, Path path)
    {
        Slot first = null;
        bool hasOneNeighbour = false;

        foreach(var neighbour in otherNeighbour.neighbours)
        {
            if (!checkedSlots.Contains(neighbour) && !path.waypointsHash.Contains(neighbour))
            {
                first = neighbour;
                hasOneNeighbour = true;
            }

            if (hasOneNeighbour) //has two now
            {
                return false;
            }
        }

        if (hasOneNeighbour) //has only one, check that one
        {
            checkedSlots.Add(otherNeighbour);
            return CheckDeadNeighbour3_Helper(first, checkedSlots, path);
        }
        else //has no neighbours
        {
            return true;
        }
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