using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;

public class LevelLoader
{
    public static Level LoadLevel(string path)
    {
        if (File.Exists(path))
        {
            string str = File.ReadAllText(path);

            var slots = JsonHelper.FromJson<Slot>(str);

            int columns = 0;
            int rows = 0;

            foreach (var slot in slots)
            {
                columns = Math.Max(columns, slot.hexPosition.col);
                rows = Math.Max(rows, slot.hexPosition.row);
            }

            var levelName = Path.GetFileNameWithoutExtension(path);

            var level = new Level(columns, rows);
            level.name = levelName;
            
            foreach (var slot in slots)
            {
                level.AddSlot(slot);
            }

            return level;    
        }

        return null;
    }
}

