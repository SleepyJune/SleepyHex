using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

[Serializable]
public class LevelTextAsset : ScriptableObject
{
    public string levelName;
    public string text;

    public int levelID;

    public int localVersion;
    public int webVersion;

    public LevelVersion webVersionFile;

    public bool hasSolution;

    public string dateModified;
    public string dateCreated;

    public float difficulty;

    public string previousLevel;
    public string nextLevel;

    public LevelTextAsset(string name, int localVersion, int webVersion, string dateModified, string dateCreated)
    {
        this.levelName = name;
        this.text = null;

        this.localVersion = localVersion;
        this.webVersion = webVersion;

        this.dateModified = dateModified;
        this.dateCreated = dateCreated;
    }
}
