using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class LevelTextAsset
{
    public string name;
    public string text;
    public string webText;

    public bool hasWebVersion;

    public DateTime webDateModified;

    public LevelTextAsset(string name, string levelText)
    {
        this.name = name;
        this.text = levelText;
        this.webText = null;
    }

    public LevelTextAsset(string name, DateTime dateModified)
    {
        this.name = name;
        this.hasWebVersion = true;
        this.webText = null;
        this.webDateModified = dateModified;
    }
}
