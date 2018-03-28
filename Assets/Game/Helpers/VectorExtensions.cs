using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

static class VectorExtensions
{
    public static float Distance(this Vector3 a, Vector3 b)
    {
        //return (Math.Abs(value1.x - value2.x) + Math.Abs(value1.y - value2.y) + Math.Abs(value1.z - value2.z))/2;
        return Math.Max(Math.Max(Math.Abs(a.x - b.x), Math.Abs(a.y - b.y)), Math.Abs(a.z - b.z));
    }

    public static Vector3 Normalized(this Vector3 value)
    {
        float factor = value.Distance(Vector3.zero);
        factor = 1f / factor;
        Vector3 result;
        result.x = value.x * factor;
        result.y = value.y * factor;
        result.z = value.z * factor;

        return result;
    }

    public static Vector3 Direction(this Vector3 current, Vector3 previous)
    {
        return (current - previous).Normalized();
    }

    public static Hex ConvertHex(this Vector3 vec)
    {
        int col = (int)Math.Round(vec.x) + ((int)Math.Round(vec.z) - ((int)Math.Round(vec.z) & 1)) / 2;
        int row = (int)Math.Round(vec.z);

        return new Hex(col, row);
    }

    public static bool isInBound(this Vector3 vec)
    {
        return vec.ConvertHex().isInBound();
    }

    public static string toStr(this Vector3 vec)
    {
        return vec.ConvertHex().toStr();
    }

    public static string toCubeStr(this Vector3 vec)
    {
        return "(" + vec.x + ", " + vec.y + ", " + vec.z + ")";
    }
}
