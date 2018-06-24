using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

[CreateAssetMenu(menuName = "Level/Level Database")]

public class LevelDatabase : ScriptableObject
{
    public LevelDifficultyGroup[] difficultyGroups = new LevelDifficultyGroup[0];
}
