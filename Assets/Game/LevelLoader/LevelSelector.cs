﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;

using UnityEngine;
using UnityEngine.UI;

using Amazon.S3.Model;

public class LevelSelector : MonoBehaviour
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

    public AmazonS3Helper amazonHelper;

    public Button[] difficultyButtons;

    public static Dictionary<string, LevelTextAsset> levelDatabase = new Dictionary<string, LevelTextAsset>();

    public SortType sortType = SortType.DateModified;

    [NonSerialized]
    public int difficultyFilter = -1;

    void Start()
    {
        LoadLevelNames();

        if (Application.platform == RuntimePlatform.WindowsEditor)
        {
            amazonHelper.ListLevelVersions(LoadLevelListWeb);
        }

        difficultyFilter = PlayerPrefs.GetInt("difficultyFilter", -1);
    }

    public void SetDifficultyFilter(int difficulty)
    {
        difficultyFilter = difficultyFilter == difficulty ? -1 : difficulty;
        PlayerPrefs.SetInt("difficultyFilter", difficultyFilter);
    }

    public void LoadLevelNamesWeb(List<AmazonS3Object> files)
    {
        foreach (var file in files)
        {
            string levelName = System.IO.Path.GetFileNameWithoutExtension(file.Key);

            LevelTextAsset level;
            if (levelDatabase.TryGetValue(levelName, out level))
            {
                level.webVersion = 0;
            }
            else
            {
                var levelTextAsset = new LevelTextAsset(levelName, -1, 0, file.LastModified);
                AddLevel(levelTextAsset);
            }
        }

        RefreshList();

        //SaveLevelList(true);
    }

    public void LoadLevelListWeb(List<LevelVersion> data)
    {
        if (data == null)
        {
            return;
        }

        foreach (var file in data)
        {
            LevelTextAsset level;
            DateTime dateModified = DateTime.Parse(file.dateModified);

            if (levelDatabase.TryGetValue(file.levelName, out level))
            {
                level.webVersion = file.version;
                level.dateModified = dateModified;
                level.webVersionFile = file;
                level.difficulty = file.difficulty;
            }
            else
            {
                var levelTextAsset = new LevelTextAsset(file.levelName, -1, file.version, dateModified);

                levelTextAsset.webVersionFile = file;
                levelTextAsset.difficulty = file.difficulty;

                AddLevel(levelTextAsset);
            }
        }

        Debug.Log("Downloaded file list: " + data.Count);

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

        if (false)//File.Exists(fileListPath))
        {
            string data = File.ReadAllText(fileListPath);

            LevelFileList fileList = JsonUtility.FromJson<LevelFileList>(data);

            foreach (var file in fileList.files)
            {
                DateTime dateModified = DateTime.Parse(file.dateModified);

                var levelTextAsset = new LevelTextAsset(file.levelName, file.version, -1, dateModified);
                AddLevel(levelTextAsset);
            }
        }
        else
        {
            DirectoryInfo d = new DirectoryInfo(DataPath.savePath);
            foreach (var file in d.GetFiles("*.json"))
            {
                var path = file.FullName;

                string str = File.ReadAllText(path);

                var levelName = System.IO.Path.GetFileNameWithoutExtension(path);

                //Debug.Log("Processing " + levelName);

                var levelTextAsset = new LevelTextAsset(levelName, 0, -1, file.LastWriteTimeUtc);
                //levelDatabase.Add(level.name, levelTextAsset);

                levelTextAsset.text = str;

                Level level = Level.LoadLevel(levelTextAsset);
                if (level != null)
                {
                    levelTextAsset.localVersion = level.version;
                    levelTextAsset.hasSolution = level.hasSolution;
                    levelTextAsset.difficulty = level.difficulty;

                    if (level.hasSolution)
                    {
                        if (level.solution.bestScore == level.solution.worstScore
                            && level.solution.numBestSolutions != level.solution.numSolutions)
                        {
                            levelTextAsset.hasSolution = false;
                        }
                    }

                    AddLevel(levelTextAsset);

                    //upload versions
                    /*LevelVersion version = new LevelVersion()
                    {
                        //category = "Default",
                        levelName = levelName,
                        version = level.version,
                        solved = level.hasSolution,
                        dateModified = levelTextAsset.dateModified.ToString(),
                        //timestamp = levelTextAsset.dateModified.GetUnixEpoch(),
                        
                    };

                    amazonHelper.UploadLevelVersion(version);*/
                }

            }
        }

        RefreshList();
    }

    void AddButton(LevelTextAsset levelText)
    {
        var newButton = Instantiate(levelSelectionButton, levelList);
        newButton.GetComponentInChildren<Text>().text = levelText.name;
        newButton.GetComponent<Button>().onClick.AddListener(() => LoadLevel(levelText.name));

        if (levelText.webVersion > levelText.localVersion ||
            (levelText.webVersionFile != null && !levelText.hasSolution && levelText.webVersionFile.solved))
        {
            newButton.transform.Find("Panel").gameObject.SetActive(true);
        }

        if (!levelText.hasSolution)
        {
            newButton.transform.Find("Unsolved").gameObject.SetActive(true);
        }

        if (levelText.difficulty > 0)
        {
            PuzzleDifficulty difficulty = (PuzzleDifficulty)levelText.difficulty;

            var ratingTransform = newButton.transform.Find("Rating");
            ratingTransform.gameObject.SetActive(true);
            ratingTransform.Find("Text").GetComponent<Text>().text = difficulty.ToString();
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
                
        for (int i = 0; i < difficultyButtons.Length; i++)
        {
            if(difficultyFilter == -1)
            {
                difficultyButtons[i].gameObject.SetActive(true);
            }
            else
            {
                if(difficultyFilter-1 != i)
                {
                    difficultyButtons[i].gameObject.SetActive(false);
                }
                else
                {
                    difficultyButtons[i].gameObject.SetActive(true);
                }
            }
        }

        IEnumerable<LevelTextAsset> levels;

        IEnumerable<LevelTextAsset> filteredLevels =
            difficultyFilter > 0 ? levelDatabase.Values.Where(level => level.difficulty == difficultyFilter) : levelDatabase.Values;

        if (sortType == SortType.Difficulty)
        {
            levels = filteredLevels.OrderByDescending(level => level.difficulty);
        }
        else if (sortType == SortType.Name)
        {
            var comparer = new NaturalComparer();
            levels = filteredLevels.OrderByDescending(level => level.name, comparer);
        }
        else
        {
            levels = filteredLevels.OrderByDescending(level => level.dateModified);
        }

        foreach (var level in levels)
        {
            if (level != null)
            {
                AddButton(level);
            }
        }
    }

    public LevelTextAsset GetLevel(string name)
    {
        LevelTextAsset levelText;
        if (levelDatabase.TryGetValue(name, out levelText))
        {
            return levelText;
        }

        return null;
    }

    public void LoadLevel(string name)
    {
        LevelTextAsset levelText;
        if (levelDatabase.TryGetValue(name, out levelText))
        {
            if (levelText.webVersion > levelText.localVersion ||
            (levelText.webVersionFile != null && !levelText.hasSolution && levelText.webVersionFile.solved))
            {
                var filename = DataPath.webPath + levelText.name + ".json";
                amazonHelper.GetFile(filename, name, LoadLevelTextWeb);
            }
            else
            {
                var filePath = DataPath.savePath + levelText.name + ".json";
                if (File.Exists(filePath))
                {
                    string data = File.ReadAllText(filePath);
                    levelText.text = data;

                    levelLoader.Load(levelText);
                }
                else
                {
                    levelText.localVersion = -1;
                    return;
                }
                levelListParent.gameObject.SetActive(false);
            }
        }
    }

    public void LoadLevelTextWeb(string name, string data)
    {
        if (data == null)
        {
            return;
        }

        LevelTextAsset levelText;
        if (levelDatabase.TryGetValue(name, out levelText))
        {
            levelText.text = data;

            Level level = levelLoader.Load(levelText);
            level.SaveLevel(false);

            levelText.localVersion = levelText.webVersion;
            levelText.dateModified = DateTime.Parse(level.dateModified);

            levelListParent.gameObject.SetActive(false);
            RefreshList();
        }
    }

    public static void AddLevel(LevelTextAsset newLevel, bool overwrite = false)
    {
        if (levelDatabase.ContainsKey(newLevel.name))
        {
            if (overwrite)
            {
                levelDatabase[newLevel.name] = newLevel;
            }
        }
        else
        {
            levelDatabase.Add(newLevel.name, newLevel);
        }
    }

    public static void DeleteLevel(string name)
    {
        if (levelDatabase.ContainsKey(name))
        {
            levelDatabase.Remove(name);
        }
    }

    public void ShowLevelList()
    {
        levelListParent.gameObject.SetActive(true);
    }

    public void HideLevelList()
    {
        levelListParent.gameObject.SetActive(false);
    }
}
