using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

public class LevelSolver
{
    Level level;
    DateTime startTime;

    int slotsVisited = 0;

    List<Path> edges;
                
    public LevelSolver(Level level)
    {
        this.level = level;        
    }

    public void GetEdges()
    {
        edges = new List<Path>();

        var slots = level.map.Values;

        foreach (var slot in slots)
        {
            if (slot.neighbours != null)
            {
                foreach (var neighbour in slot.neighbours)
                {
                    var newPath = new Path(slot);

                    if (newPath.AddPoint(neighbour))
                    {
                        edges.Add(newPath);
                    }
                }
            }
        }

        Debug.Log("Num Edges: " + edges.Count);
    }

    public void Solve()
    {
        startTime = DateTime.Now;

        GetEdges();

        foreach (var startPoint in level.map.Values)
        {
            if (startPoint.number >= 0)
            {
                CalculateShortestPaths(startPoint);
            }
        }

        Debug.Log("Visited: " + slotsVisited);
    }

    public void CalculateShortestPaths(Slot start)
    {
        if (DateTime.Now.Subtract(startTime).Seconds > 10)
        {
            return;
        }

        Dictionary<Slot, int> dist = new Dictionary<Slot, int>();
        Dictionary<Slot, Slot> prev = new Dictionary<Slot, Slot>();
        
        var slots = level.map.Values;

        foreach (var slot in slots) //initialize
        {
            dist.Add(slot, 999);
            prev.Add(slot, null);
        }

        dist[start] = -start.number;

        for(int i=0;i< slots.Count; i++)
        {
            foreach(var edge in edges)
            {
                var weight = -edge.GetLastPoint().slot.number;

                var alternativeDistance = dist[edge.startPoint.slot] + weight;

                if (alternativeDistance < dist[edge.lastPoint.slot])
                {
                    dist[edge.lastPoint.slot] = alternativeDistance;
                    prev[edge.lastPoint.slot] = edge.startPoint.slot;
                }

                slotsVisited++;
            }
        }

        var shortestSlot = dist.OrderBy(p => p.Value).FirstOrDefault();

        if(shortestSlot.Value < 0)
        {
            Debug.Log(shortestSlot.Value);
            //MakePath(start, shortestSlot.Key, prev.Values);
        }        
    }

    public void MakePath(Slot start, Slot end, IEnumerable<Slot> prev)
    {
        Path path = new Path(start);

        HashSet<Slot> visited = new HashSet<Slot>();

        Slot current = end;
        while(current != start)
        {
            path.AddPoint(current);
            //current = prev[current];
        }
    }
}
