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

        amazonHelper.ListFiles(DataPath.webPath, LoadLevelNamesWeb);
    }

    public void LoadLevelNamesWeb(List<S3Object> files)
    {
        foreach(var file in files)
        {
            string levelName = System.IO.Path.GetFileNameWithoutExtension(file.Key);

            LevelTextAsset level;
            if(levelDatabase.TryGetValue(levelName, out level))
            {
                level.hasWebVersion = true;
            }
            else
            {
                var levelTextAsset = new LevelTextAsset(levelName);
                AddLevel(levelTextAsset);
            }
        }

        RefreshList();
    }

    void LoadLevelNames()
    {
        if (!Directory.Exists(DataPath.savePath))
        {
            Directory.CreateDirectory(DataPath.savePath);
        }

        var levels = Resources.LoadAll("Levels", typeof(TextAsset));
        int numfiles = 0;
        foreach (var obj in levels)
        {
            var level = obj as TextAsset;
            
            var levelTextAsset = new LevelTextAsset(level.name, level.text);
            //levelDatabase.Add(level.name, levelTextAsset);

            AddLevel(levelTextAsset);
            numfiles += 1;
        }

        RefreshList();
    }

    void AddButton(LevelTextAsset levelText)
    {
        var newButton = Instantiate(levelSelectionButton, levelList);
        newButton.GetComponentInChildren<Text>().text = levelText.name;
        newButton.GetComponent<Button>().onClick.AddListener(() => LoadLevel(levelText.name));
    }

    public void RefreshList()
    {
        foreach(Transform child in levelList)
        {
            Destroy(child.gameObject);
        }
        

        foreach (var level in levelDatabase.Values)
        {
            if(level != null)
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
        if(levelDatabase.TryGetValue(name, out levelText))
        {
            if (levelText.hasWebVersion && levelText.webText == null)
            {
                var filename = DataPath.webPath + levelText.name + ".json";
                amazonHelper.GetFile(filename, name, LoadLevelTextWeb);
            }
            else
            {
                levelLoader.Load(levelText);
                levelListParent.gameObject.SetActive(false);
            }
        }
    }

    public void LoadLevelTextWeb(string name, string data)
    {
        if(data == null)
        {
            return;
        }

        LevelTextAsset levelText;
        if (levelDatabase.TryGetValue(name, out levelText))
        {
            if (levelText.hasWebVersion)
            {
                levelText.webText = data;

                levelLoader.Load(levelText);
                levelListParent.gameObject.SetActive(false);
            }
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
}
