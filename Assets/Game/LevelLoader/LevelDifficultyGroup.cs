using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

[CreateAssetMenu(menuName = "Level/Level Difficulty Group")]
public class LevelDifficultyGroup : ScriptableObject
{
    public string groupName;

    public List<LevelTextAsset> levels = new List<LevelTextAsset>();
}
