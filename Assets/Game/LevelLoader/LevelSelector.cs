using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;

using UnityEngine;
using UnityEngine.UI;

public class LevelSelector : MonoBehaviour
{
    public Transform levelList;
    public Transform levelListParent;

    public GameObject levelSelectionButton;

    public LevelLoader levelLoader;

    public static Dictionary<string, LevelTextAsset> levelDatabase = new Dictionary<string, LevelTextAsset>();

    void Start()
    {
        LoadLevelNames();
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

    public void LoadLevel(string name)
    {
        LevelTextAsset levelText;
        if(levelDatabase.TryGetValue(name, out levelText))
        {
            levelLoader.Load(levelText);
            levelListParent.gameObject.SetActive(false);
        }
    }

    public static void AddLevel(LevelTextAsset newLevel)
    {
        if (levelDatabase.ContainsKey(newLevel.name))
        {
            levelDatabase[newLevel.name] = newLevel;
        }
        else
        {
            levelDatabase.Add(newLevel.name, newLevel);
        }
    }

    public void ShowLevelList()
    {
        levelListParent.gameObject.SetActive(true);
    }
}
