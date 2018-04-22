using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using Amazon.S3.Model;

public class LevelSelector2 : MonoBehaviour
{
    public enum SortType
    {
        DateModified,
        Name,
        Difficulty,
    }

    public Transform levelList;
    public Transform levelListParent;

    public GameObject levelSelectionButton;

    public LevelLoader levelLoader;

    public Button[] difficultyButtons;

    public SortType sortType = SortType.Difficulty;

    [NonSerialized]
    public int difficultyFilter = -1;

    void Start()
    {
        Initialize();
    }

    public void Initialize()
    {
        difficultyFilter = PlayerPrefs.GetInt("difficultyFilter", -1);
        LoadLevels();

        //WriteLevelID();
    }

    void LoadLevels()
    {
        if (!LevelSelector.isLoaded)
        {
            LoadLevelNames();
            RefreshList();

            LevelSelector.isLoaded = true;
        }
        else
        {
            RefreshList();
        }
    }
        
    public void SetDifficultyFilter(int difficulty)
    {
        difficultyFilter = difficultyFilter == difficulty ? -1 : difficulty;
        PlayerPrefs.SetInt("difficultyFilter", difficultyFilter);

        RefreshList();
    }
        
    void LoadLevelNames()
    {
        if (!Directory.Exists(DataPath.savePath))
        {
            Directory.CreateDirectory(DataPath.savePath);
        }

        if (Application.platform != RuntimePlatform.WindowsEditor) //overwrite files if mobile platform
        {
            var levels = Resources.LoadAll("Levels", typeof(TextAsset));

            foreach (var obj in levels)
            {
                var level = obj as TextAsset;

                var path = DataPath.savePath + level.name + ".json";

                File.WriteAllText(DataPath.savePath + level.name + ".json", level.text);
            }
        }

        var fileListPath = DataPath.savePath + DataPath.fileListFolder + DataPath.fileListName;

        DirectoryInfo d = new DirectoryInfo(DataPath.savePath);
        foreach (var file in d.GetFiles("*.json"))
        {
            var path = file.FullName;

            string str = File.ReadAllText(path);

            var levelName = System.IO.Path.GetFileNameWithoutExtension(path);

            //Debug.Log("Processing " + levelName);

            var levelTextAsset = new LevelTextAsset(levelName, 0, -1, file.LastWriteTimeUtc, file.CreationTimeUtc);
            //levelDatabase.Add(level.name, levelTextAsset);

            levelTextAsset.text = str;

            Level level = Level.LoadLevel(levelTextAsset);
            if (level != null)
            {
                levelTextAsset.localVersion = level.version;
                levelTextAsset.hasSolution = level.hasSolution;
                levelTextAsset.difficulty = level.difficulty;
                levelTextAsset.levelID = level.levelID;

                levelTextAsset.dateCreated = DateTime.Parse(level.dateCreated);
                levelTextAsset.dateModified = DateTime.Parse(level.dateModified);

                if (level.hasSolution)
                {
                    if (level.solution.bestScore == level.solution.worstScore
                        && level.solution.numBestSolutions != level.solution.numSolutions)
                    {
                        levelTextAsset.hasSolution = false;
                    }
                }

                LevelSelector.AddLevel(levelTextAsset);
            }
        }
    }
        
    public void SetSortType(int sortType)
    {
        this.sortType = (SortType)sortType;
        RefreshList();
    }

    public void RefreshList()
    {        
        foreach (Transform child in levelList)
        {
            Destroy(child.gameObject);
        }

        IEnumerable<LevelTextAsset> filteredLevels =
            difficultyFilter > 0 ? LevelSelector.levelDatabase.Values.Where(level => level.difficulty == difficultyFilter) 
                                        : new List<LevelTextAsset>();

        if (sortType == SortType.Difficulty)
        {
            var comparer = new NaturalComparer();
            LevelSelector.levelListDatabase = filteredLevels
                        .OrderBy(level => level.difficulty)
                        .ThenBy(level => level.name, comparer)
                        .ToList();
        }
        else if (sortType == SortType.Name)
        {
            //var comparer = new NaturalComparer();
            //levelListDatabase = filteredLevels.OrderByDescending(level => level.name, comparer).ToList();

            LevelSelector.levelListDatabase = filteredLevels.OrderBy(level => level.difficulty).ThenBy(level => level.levelID).ToList();
        }
        else
        {
            LevelSelector.levelListDatabase = filteredLevels.OrderBy(level => level.dateCreated).ToList();
        }

        Debug.Log("Num levels: " + LevelSelector.levelListDatabase.Count());

        foreach (var level in LevelSelector.levelListDatabase)
        {
            if (level != null)
            {
                var newButton = Instantiate(levelSelectionButton, levelList);
                newButton.GetComponent<LevelSelectButton>().SetButton(level);
            }
        }
    }    
}
