using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using UnityEngine;

public class LevelSolver : ThreadedJob
{
    public delegate void SubLevelSolver();

    Level level;

    Queue<Path> solvedPaths;

    DateTime startTime;

    int slotsProcessed = 0;
    int slotsVisited = 0;
    int numSlotsToProcess = 0;
    int numSlots = 0;

    Path bestPath;

    float progressPercent = 0;

    bool abort
    {
        get; set;
    }

    public LevelSolver(Level level)
    {
        this.level = level;

        this.solvedPaths = new Queue<Path>();
    }

    public void Solve()
    {

    }

    public override void Abort()
    {
        abort = true;
        
        if (IsDone == false)
        {
            Debug.Log("Aborting");
        }
    }

    protected override void ThreadFunction()
    {
        Debug.Log("Solver Start");

        ThreadPool.SetMaxThreads(2, 2);

        startTime = DateTime.Now;

        var slots = level.map.Values.Where(s => s.number >= 0);
        numSlots = slots.Count();

        slots = slots.Where(s => s.isNumber);

        numSlotsToProcess = slots.Count();
        slotsProcessed = 0;

        progressPercent = 0;

        int resetEventCount = numSlotsToProcess;
        var resetEvent = new ManualResetEvent(false);

        if (numSlotsToProcess == 0)
        {
            return;
        }
        
        foreach (var startPoint in slots)
        {
            var path = new Path(startPoint);

            ThreadPool.QueueUserWorkItem(new WaitCallback(
                delegate (object state)
                {
                    try
                    {                   

                    ExploreNeighbour(path);
                    Interlocked.Increment(ref slotsProcessed);
                    progressPercent = (float)slotsProcessed / numSlotsToProcess;
                    
                    if (IsDone || Interlocked.Decrement(ref resetEventCount) == 0) resetEvent.Set();

                    }
                    catch (Exception e)
                    {
                        Debug.LogError(e);
                    }
                }), null);

            //ExploreNeighbour(path);
        }
                
        resetEvent.WaitOne();
        GetSolution();
    }

    /*public void MakeSubThread2()
    {
        //ThreadPool.SetMaxThreads(5, 5);
    }

    public void MakeSubThread()
    {
        var maxThreadCount = 8;
        while (threads.Count < maxThreadCount)
        {
            var newThread = new Thread(SubThreadStart);
            threads.Enqueue(newThread);

            newThread.Start();
        }
    }

    public void SubThreadStart()
    {
        Debug.Log("Subthread start: " + threads.Count);

        var subSolver = subSolvers.Dequeue();

        subSolver();
        SubThreadEnded();
    }

    public void SubThreadEnded()
    {
        Debug.Log("Subthread ended: " + threads.Count);

        slotsProcessed += 1;
        progressPercent = (float)slotsProcessed / numSlotsToProcess;

        if (abort || subSolvers.Count == 0)
        {
            GetSolution();
        }
        else
        {
            MakeSubThread();
        }
    }*/

    public void GetSolution()
    {
        var solvedList = solvedPaths.Where(p => p.waypoints.Count == numSlots)
                         .OrderByDescending(p => p.GetSum()).ToList();

        TimeSpan solveTime = DateTime.Now.Subtract(startTime);

        Debug.Log("Solve time: " + solveTime.ToString());
        Debug.Log("Visited: " + slotsVisited);
        Debug.Log("Max slots: " + numSlots);

        bestPath = solvedList.FirstOrDefault();

        //JobFinished();
    }

    public void ExploreNeighbour(Path path)
    {
        if (abort)// || DateTime.Now.Subtract(startTime).Seconds > 10)
        {
            return;
        }

        if(slotsVisited % 10000 == 0)
        {
            Debug.Log("Exploring: " + slotsVisited);
        }

        var lastPoint = path.GetLastPoint();

        if (lastPoint != null && lastPoint.slot.neighbours != null)
        {
            HashSet<Slot> neighbourVisted = new HashSet<Slot>();

            foreach (var neighbour in lastPoint.slot.neighbours)
            {
                neighbourVisted.Add(neighbour);

                if (path.AddPoint(neighbour))
                {
                    slotsVisited += 1;
                    var newPath = new Path(path);
                    path.GoBack();

                    if (CheckDeadNeighbour(lastPoint.slot, newPath, neighbourVisted))
                    {
                        continue;
                    }

                    lock (solvedPaths)
                    {
                        solvedPaths.Enqueue(newPath);
                    }
                    
                    ExploreNeighbour(newPath);
                }
            }
        }
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

    public Path GetBestPath()
    {
        return bestPath;
    }
}
