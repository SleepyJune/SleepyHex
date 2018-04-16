using System;
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

    public static Dictionary<string, LevelTextAsset> levelDatabase = new Dictionary<string, LevelTextAsset>();

    public SortType sortType = SortType.DateModified;

    void Start()
    {
        LoadLevelNames();

        amazonHelper.ListLevelVersions(LoadLevelListWeb);

        //var filename = DataPath.webPath + DataPath.fileListFolder + DataPath.fileListName;
        //amazonHelper.GetFile(filename, DataPath.fileListName, LoadLevelListWeb);

        //amazonHelper.ListFiles(DataPath.webPath, LoadLevelNamesWeb);
        //SaveLevelList(false);
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
            }
            else
            {
                var levelTextAsset = new LevelTextAsset(file.levelName, -1, file.version, dateModified);

                levelTextAsset.webVersionFile = file;

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

                    if (level.hasSolution)
                    {
                        if(level.solution.bestScore == level.solution.worstScore
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

        IEnumerable<LevelTextAsset> levels;

        if (sortType == SortType.Difficulty)
        {
            levels = levelDatabase.Values.OrderByDescending(level => level.dateModified);
        }
        else if (sortType == SortType.Name)
        {
            levels = levelDatabase.Values
                        .OrderByDescending(level => level.name.Any(char.IsDigit) ?
                        Int32.Parse(System.Text.RegularExpressions.Regex.Match(level.name, @"\d+").Value) : 0
                        );

            var comparer = new NaturalComparer();
            levels = levelDatabase.Values.OrderByDescending(level => level.name, comparer);
        }
        else
        {
            levels = levelDatabase.Values.OrderByDescending(level => level.dateModified);
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
