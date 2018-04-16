using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

public class EditorSolutionViewer : LevelSolutionViewer
{
    List<Path> paths;

    int currentIndex = 0;

    public void SetSolvedPaths(List<Path> solvedPaths)
    {
        this.paths = solvedPaths;
    }

    public void Next()
    {
        if(paths != null)
        {
            if(currentIndex+1 < paths.Count)
            {
                currentIndex++;
                ShowPath();
            }
        }
    }

    public void Prev()
    {
        if (paths != null)
        {
            if (currentIndex - 1 >= 0)
            {
                currentIndex--;
                ShowPath();
            }
        }
    }

    public void ShowPath()
    {
        if (paths != null && currentIndex >= 0 && currentIndex < paths.Count)
        {
            var path = paths[currentIndex];
            
            List<Vector3> waypoints = new List<Vector3>();
            foreach (var point in path.waypoints)
            {
                waypoints.Add(point.slot.position);
            }

            DrawPathLine(waypoints);
        }
    }
}
