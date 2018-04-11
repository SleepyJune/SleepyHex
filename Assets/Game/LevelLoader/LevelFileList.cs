using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

[Serializable]
public class LevelFileList
{
    public LevelVersion[] files;

    public LevelFileList(LevelVersion[] files)
    {
        this.files = files;
    }
}