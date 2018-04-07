using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class PathEdge
{
    Slot start;
    Slot end;

    Path path;

    public PathEdge(Slot start, Slot end, Path path)
    {
        this.start = start;
        this.end = end;

        this.path = path;
    }
}
