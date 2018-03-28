using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

public class Level
{
    public string name = "New Level";
    public Dictionary<Vector3, Slot> map;

    public Level()
    {
        map = new Dictionary<Vector3, Slot>();
    }
}