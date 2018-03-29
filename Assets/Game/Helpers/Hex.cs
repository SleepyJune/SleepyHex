using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

[System.Serializable]
public struct Hex : IEquatable<Hex>
{
    public int col;
    public int row;

    public Hex(int column, int row)
    {
        this.col = column;
        this.row = row;
    }

    public static bool operator ==(Hex value1, Hex value2)
    {
        return value1.col == value2.col
            && value1.row == value2.row;
    }

    public static bool operator !=(Hex value1, Hex value2)
    {
        return !(value1 == value2);
    }

    public override bool Equals(object obj)
    {
        return (obj is Hex) ? this == (Hex)obj : false;
    }

    public bool Equals(Hex other)
    {
        return this == other;
    }

    public override int GetHashCode()
    {
        return this.col * 100 + this.row;
    }
}

public static class HexExtensions
{
    public static Vector3 ConvertCube(this Hex hex)
    {
        float x = hex.col;
        float z = hex.row;
        float y = -x - z;
        
        return new Vector3(x, y, z);
    }

    public static Vector3 GetWorldPos(this Hex hexPos, float size = 25)
    {
        float x = size * (3 / 2f) * hexPos.col;
        float y = size * Mathf.Sqrt(3) * (hexPos.row + hexPos.col / 2f);

        return new Vector3(x, y, 0);
    }
}

