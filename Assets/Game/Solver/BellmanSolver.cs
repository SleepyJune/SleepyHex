using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

public class BellmanSolver
{
    List<Slot> verticies;

    Level level;

    public BellmanSolver(Level level)
    {
        this.level = level;

        verticies = level.map.Values.Where(n => n.isNumber).ToList();
    }

    public void GetEdges()
    {
        foreach(var start in verticies)
        {
            foreach(var end in verticies)
            {
                if(start != end)
                {

                }
            }
        }
    }

    public void MakePathEdge()
    {

    }

}
