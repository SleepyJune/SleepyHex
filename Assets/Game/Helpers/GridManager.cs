using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

public class GridManager
{
    int column;
    int row;

    float slotHeight = 45;
    float slotWidth = 45;

    Vector3 initialPos;

    public GridManager(int column, int row)
    {
        this.column = column;
        this.row = row;
    }

    public void CalculateInitialPos()
    {
        /*initialPos = new Vector3(-slotWidth * row / 2f + slotWidth / 2, 0,
            column / 2f * slotHeight - slotHeight / 2);*/
    }

    public Vector3 CalculateWorldPos(Hex hexPos)
    {
        float offset = 0;

        if(hexPos.row % 2 != 0)
        {
            offset = slotHeight / 2;
        }

        float x = initialPos.x + hexPos.row * slotWidth * .75f;
        float z = initialPos.z - hexPos.col * slotHeight + offset;

        return new Vector3(x, z, 0);
    }
}