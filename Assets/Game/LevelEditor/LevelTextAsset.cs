using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class LevelTextAsset
{
    public string name;
    public string text;

    public int localVersion;
    public int webVersion;

    public bool hasSolution;

    public DateTime dateModified;

    public LevelTextAsset(string name, int localVersion, int webVersion, DateTime dateModified)
    {
        this.name = name;
        this.text = null;

        this.localVersion = localVersion;
        this.webVersion = webVersion;

        this.dateModified = dateModified;
    }
}
