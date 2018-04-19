using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class PathEdge
{
    PathSlot start;
    PathSlot end;

    Path path;

    public PathEdge(PathSlot start, PathSlot end, Path path)
    {
        this.start = start;
        this.end = end;

        this.path = path;
    }
}