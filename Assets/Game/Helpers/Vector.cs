﻿using System;
using System.Collections;
using System.Collections.Generic;

public struct Vector : IEquatable<Vector>
{
    public double x;
    public double y;
    public double z;

    public static List<Vector> directions = new List<Vector>{
            new Vector( 1, -1,  0), new Vector( 1,  0, -1), new Vector( 0,  1, -1),
            new Vector(-1,  1,  0), new Vector(-1,  0,  1), new Vector( 0, -1,  1)
        };

    public static Dictionary<Vector, int> directionTable = new Dictionary<Vector, int>{
            {new Vector( 1, -1,  0), 0}, {new Vector( 1,  0, -1), 1}, {new Vector( 0,  1, -1), 2},
            {new Vector(-1,  1,  0), 3}, {new Vector(-1,  0,  1), 4}, {new Vector( 0, -1,  1), 5}
        };

    public Vector(double x, double y, double z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public static Vector Zero
    {
        get { return new Vector(0f, 0f, 0f); }
    }

    public static Vector Undefined
    {
        get { return new Vector(-1337f, -1337f, -1337f); }
    }

    public static bool operator ==(Vector value1, Vector value2)
    {
        return value1.ConvertHex() == value2.ConvertHex();
    }

    public static bool operator !=(Vector value1, Vector value2)
    {
        return !(value1 == value2);
    }

    public static Vector operator +(Vector value1, Vector value2)
    {
        value1.x += value2.x;
        value1.y += value2.y;
        value1.z += value2.z;
        return value1;
    }

    public static Vector operator -(Vector value)
    {
        value = new Vector(-value.x, -value.y, -value.z);
        return value;
    }

    public static Vector operator -(Vector value1, Vector value2)
    {
        value1.x -= value2.x;
        value1.y -= value2.y;
        value1.z -= value2.z;
        return value1;
    }

    public static Vector operator *(Vector value1, Vector value2)
    {
        value1.x *= value2.x;
        value1.y *= value2.y;
        value1.z *= value2.z;
        return value1;
    }

    public static Vector operator *(Vector value, double scaleFactor)
    {
        value.x *= scaleFactor;
        value.y *= scaleFactor;
        value.z *= scaleFactor;
        return value;
    }

    public static Vector operator *(double scaleFactor, Vector value)
    {
        value.x *= scaleFactor;
        value.y *= scaleFactor;
        value.z *= scaleFactor;
        return value;
    }

    public override bool Equals(object obj)
    {
        return (obj is Vector) ? this == (Vector)obj : false;
    }

    public bool Equals(Vector other)
    {
        return this == other;
    }

    public override int GetHashCode()
    {
        return this.ConvertHex().GetHashCode();
    }
}

static class VectorExtensions
{
    public static double Distance(this Vector a, Vector b)
    {
        //return (Math.Abs(value1.x - value2.x) + Math.Abs(value1.y - value2.y) + Math.Abs(value1.z - value2.z))/2;
        return Math.Max(Math.Max(Math.Abs(a.x - b.x), Math.Abs(a.y - b.y)), Math.Abs(a.z - b.z));
    }

    public static Vector Normalized(this Vector value)
    {
        double factor = value.Distance(Vector.Zero);
        factor = 1f / factor;
        Vector result;
        result.x = value.x * factor;
        result.y = value.y * factor;
        result.z = value.z * factor;

        return result;
    }

    public static Vector Direction(this Vector current, Vector previous)
    {
        return (current - previous).Normalized();
    }

    public static Hex ConvertHex(this Vector vec)
    {
        int col = (int)Math.Round(vec.x) + ((int)Math.Round(vec.z) - ((int)Math.Round(vec.z) & 1)) / 2;
        int row = (int)Math.Round(vec.z);

        return new Hex(col, row);
    }

    public static bool isInBound(this Vector vec)
    {
        return vec.ConvertHex().isInBound();
    }

    public static string toStr(this Vector vec)
    {
        return vec.ConvertHex().toStr();
    }

    public static string toCubeStr(this Vector vec)
    {
        return "(" + vec.x + ", " + vec.y + ", " + vec.z + ")";
    }

    public static int GetRotation(this Vector end, Vector start)
    {
        for (int i = 0; i < Vector.directions.Count; i++)
        {
            var pos = start + Vector.directions[i];
            if (pos == end)
            {
                return i;
            }
        }

        //Console.Error.WriteLine(start.toStr() + " -> " + end.toStr());

        return -1;
    }
}
