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
    public Transform levelList;
    public Transform levelListParent;

    public GameObject levelSelectionButton;

    public LevelLoader levelLoader;

    public AmazonS3Helper amazonHelper;

    public static Dictionary<string, LevelTextAsset> levelDatabase = new Dictionary<string, LevelTextAsset>();

    void Start()
    {
        LoadLevelNames();

        var filename = DataPath.webPath + DataPath.fileListFolder + DataPath.fileListName;
        amazonHelper.GetFile(filename, DataPath.fileListName, LoadLevelListWeb);

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

        SaveLevelList(true);
    }

    public void LoadLevelListWeb(string name, string data)
    {
        if (data == null)
        {
            return;
        }

        LevelFileList fileList = JsonUtility.FromJson<LevelFileList>(data);

        foreach (var file in fileList.files)
        {
            LevelTextAsset level;
            DateTime dateModified = DateTime.Parse(file.dateModified);

            if (levelDatabase.TryGetValue(file.levelName, out level))
            {
                level.webVersion = file.version;
                level.dateModified = dateModified;
            }
            else
            {
                var levelTextAsset = new LevelTextAsset(file.levelName, -1, file.version, dateModified);
                AddLevel(levelTextAsset);
            }
        }

        Debug.Log("Downloaded file list");

        RefreshList();
    }

    void LoadLevelNames()
    {
        if (!Directory.Exists(DataPath.savePath))
        {
            Directory.CreateDirectory(DataPath.savePath);
        }

        var levels = Resources.LoadAll("Levels", typeof(TextAsset));

        foreach (var obj in levels)
        {
            var level = obj as TextAsset;

            var path = DataPath.savePath + level.name + ".json";
            if (!File.Exists(path)) //overwrite files if mobile platform
            {
                File.WriteAllText(DataPath.savePath + level.name + ".json", level.text);
            }
        }
                

        var fileListPath = DataPath.savePath + DataPath.fileListFolder + DataPath.fileListName;

        if (File.Exists(fileListPath))
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

                Debug.Log("Processing " + levelName);

                var levelTextAsset = new LevelTextAsset(levelName, 0, -1, file.LastWriteTimeUtc);
                //levelDatabase.Add(level.name, levelTextAsset);

                levelTextAsset.text = str;

                Level level = Level.LoadLevel(levelTextAsset);
                if (level != null)
                {
                    levelTextAsset.localVersion = level.version;

                    AddLevel(levelTextAsset);
                }

            }

            SaveLevelList(false);
        }

        RefreshList();
    }

    void AddButton(LevelTextAsset levelText)
    {
        var newButton = Instantiate(levelSelectionButton, levelList);
        newButton.GetComponentInChildren<Text>().text = levelText.name;
        newButton.GetComponent<Button>().onClick.AddListener(() => LoadLevel(levelText.name));

        if(levelText.webVersion > levelText.localVersion)
        {
            newButton.transform.Find("Panel").gameObject.SetActive(true);
        }
    }

    public void RefreshList()
    {
        foreach (Transform child in levelList)
        {
            Destroy(child.gameObject);
        }

        foreach (var level in levelDatabase.Values.OrderByDescending(level => level.dateModified))
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
            if (levelText.webVersion > levelText.localVersion)
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
                    SaveLevelList(false);
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
            SaveLevelList(false);
            
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

    public void SaveLevelList(bool saveWeb = true)
    {
        if (saveWeb)
        {
            SaveLevelListHelper(true);
        }

        SaveLevelListHelper(false);
    }

    public void SaveLevelListHelper(bool saveWeb)
    {
        List<LevelVersion> fileList = new List<LevelVersion>();

        foreach (var level in levelDatabase.Values.OrderByDescending(level => level.dateModified))
        {
            if ((saveWeb && level.webVersion >= 0)
                || (!saveWeb && level.localVersion >= 0))
            {
                LevelVersion version = new LevelVersion()
                {
                    levelName = level.name,
                    version = saveWeb ? level.webVersion : level.localVersion,
                    dateModified = level.dateModified.ToString(),
                };

                fileList.Add(version);
            }
        }

        var levelFileList = new LevelFileList(fileList.ToArray());
        var dataStr = JsonUtility.ToJson(levelFileList);

        if (saveWeb)
        {
            var webPath = DataPath.webPath + DataPath.fileListFolder + DataPath.fileListName;
            amazonHelper.PostObject(webPath, dataStr);
        }
        else
        {
            if (!Directory.Exists(DataPath.savePath + DataPath.fileListFolder))
            {
                Directory.CreateDirectory(DataPath.savePath + DataPath.fileListFolder);
            }

            var filePath = DataPath.savePath + DataPath.fileListFolder + DataPath.fileListName;
            File.WriteAllText(filePath, dataStr);
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
