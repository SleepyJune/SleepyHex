using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

public enum SolutionType
{
    NoSolution,
    WorstSolution,
    Solution,
    BestSolution,
}

[Serializable]
public class LevelSolution
{
    public int bestScore;
    public int twoStarScore;
    public int worstScore;

    public int numSolutions;
    public int numBestSolutions;

    public Vector3[] bestPath;

    public int version;
}
