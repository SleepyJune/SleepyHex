using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

public class TouchInput
{
    public int id;
    public Vector2 startPosition;
    public Vector2 previousPosition;
    public Vector2 position;

    public float lastUpdate;
    
    public TouchInput(Touch touch)
    {
        id = touch.fingerId;
        startPosition = touch.position;
        previousPosition = startPosition;
        position = startPosition;                
    }
}
