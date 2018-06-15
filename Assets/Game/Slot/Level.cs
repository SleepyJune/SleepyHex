using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;

using UnityEngine;

public enum PuzzleDifficulty
{    
    Unrated,
    Easy,
    Normal,
    Hard,
    Insane,
    Beginner,
}

[Serializable]
public class Level
{
    public string levelName = "New Level";

    public int levelID;

    public int version;
    
    public float difficulty;

    public string dateCreated;
    public string dateModified;

    public Slot[] slots;

    public LevelSolution solution;

    [NonSerialized]
    public Dictionary<Vector3, Slot> map;

    [NonSerialized]
    public bool modified;

    [NonSerialized]
    public bool isInitialized;

    [NonSerialized]
    public bool isSolvedInEditor;

    public Level(int columns, int rows)
    {
        map = new Dictionary<Vector3, Slot>();

        dateCreated = DateTime.UtcNow.ToString();
    }

    public void AddSlot(Slot slot)
    {
        if (!map.ContainsKey(slot.position))
        {
            map.Add(slot.position, slot);            
        }
    }

    public void ChangeSlotNumber(Slot newSlot)
    {
        Slot slot;
        if (map.TryGetValue(newSlot.position, out slot))
        {
            slot.number = newSlot.number;
        }
    }

    public void ChangeHideText(Slot newSlot)
    {
        Slot slot;
        if (map.TryGetValue(newSlot.position, out slot))
        {
            slot.hideNumber = newSlot.hideNumber;
        }
    }

    public void MakeEmptyLevel()
    {
        var gridColumns = 12;
        var gridRows = 12;

        var maxWidth = 350;
        var maxHeight = 450;

        for (int row = (int)Math.Round(-gridRows/2f); row < gridRows; row++)
        {
            for (int column = (int)Math.Round(-gridColumns / 2f); column < gridColumns; column++)
            {
                var hex = new Hex(column, row);

                var worldPos = hex.GetWorldPos();

                if (worldPos.x < 0 || worldPos.y < 0 || worldPos.x > maxWidth || worldPos.y > maxHeight)
                {
                    continue;
                }

                var slot = new Slot(-1, hex);

                AddSlot(slot);
            }
        }

        slots = map.Values.ToArray();
    }

    public void Initialize(bool reinitialize = false)
    {
        if (isInitialized && !reinitialize)
        {
            return;
        }

        if(map == null)
        {
            map = new Dictionary<Vector3, Slot>();
        }

        foreach(var slot in slots)
        {
            AddSlot(slot);       
        }

        //Add neighbours
        foreach(var slot in map.Values)
        {
            slot.neighbours = new HashSet<Slot>();

            foreach (var direction in VectorExtensions.directions)
            {
                var pos = slot.position + direction;
                
                Slot neighbour;
                if(map.TryGetValue(pos, out neighbour))
                {
                    slot.AddNeighbour(neighbour); //will be checked in add neighbour
                }                
            }            
        }

        isInitialized = true;
    }

    public bool hasSolution
    {
        get
        {
            return solution != null && solution.version == version && solution.bestScore > 0;
        }
    }

    public bool isNewLevel
    {
        get
        {
            return !LevelSelector.levelDatabase.ContainsKey(levelName);
        }
    }

    public bool canSave
    {
        get
        {
            return hasSolution && difficulty != 0 && levelName != "New Level";
        }
    }

    public IDictionary<string, string> GetMetadata()
    {
        IDictionary<string, string> data = new Dictionary<string, string>()
        {
            { "name", levelName },
            { "version", version.ToString() },
        };

        return data;
    }

    public PuzzleDifficulty GetPuzzleDifficulty()
    {
        return (PuzzleDifficulty) Math.Floor(difficulty);
    }

    public string GetDifficultyString()
    {
        return ((PuzzleDifficulty)Math.Floor(difficulty)).ToString();
    }

    public void SetLastPlayedLevel()
    {
        if (levelName != null && levelName != "")
        {
            string key = "LastPlayedLevel";
            PlayerPrefs.SetString(key, levelName);
        }
    }

    public static string GetLastPlayedLevel()
    {
        string key = "LastPlayedLevel";

        return PlayerPrefs.GetString(key, null);    
    }

    public static string GetHighestPlayedLevel(int difficulty)
    {
        string key = "HighestPlayedLevel_" + ((PuzzleDifficulty) difficulty).ToString();

        return PlayerPrefs.GetString(key, null);
    }

    public LevelTextAsset SaveLevel(bool modified = true)
    {
        slots = map.Values.Where(s => s.number >= 0).ToArray();

        string levelStr = JsonUtility.ToJson(this);

        if (!Directory.Exists(DataPath.savePath))
        {
            Directory.CreateDirectory(DataPath.savePath);
        }

        var filePath = DataPath.savePath + levelName + ".json";

        File.WriteAllText(filePath, levelStr);
                
        var levelText = new LevelTextAsset(levelName, version, version, dateModified, dateCreated);
        levelText.text = levelStr;
        levelText.hasSolution = hasSolution;
        levelText.difficulty = difficulty;
        levelText.levelID = levelID;
        
        Debug.Log("Saved to: " + filePath);

        return levelText;
    }

    public static Level LoadLevel(LevelTextAsset levelText)
    {
        if (levelText != null)
        {
            string str = levelText.text;

            var level = JsonUtility.FromJson<Level>(str);

            if (level != null)
            {
                level.Initialize();
                return level;
            }
            else
            {
                return null;
            }            
        }

        return null;
    }
}