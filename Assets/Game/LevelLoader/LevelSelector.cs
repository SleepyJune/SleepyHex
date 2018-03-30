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

        /*var levels = Resources.LoadAll("Levels", typeof(TextAsset));
        foreach (var obj in levels)
        {
            var level = obj as TextAsset;
            //var path = DataPath.savePath + level.name + ".json";

            if (!File.Exists(path) || Application.isMobilePlatform) //overwrite files if mobile platform
            {
                File.WriteAllText(DataPath.savePath + level.name + ".json", level.text);
            }
        }*/

        DirectoryInfo d = new DirectoryInfo(DataPath.savePath);

        int numfiles = 0;
        foreach (var file in d.GetFiles("*.json"))
        {
            var newButton = Instantiate(levelSelectionButton, levelList);

            //newButton.transform.SetParent(levelSelectionButtonHolder.transform, false);
            newButton.GetComponentInChildren<Text>().text = System.IO.Path.GetFileNameWithoutExtension(file.Name);

            string fullPath = file.FullName;

            newButton.GetComponent<Button>().onClick.AddListener(() => LoadLevel(fullPath));

            numfiles += 1;
        }
    }

    public void LoadLevel(string path)
    {
        levelLoader.Load(path);
        levelListParent.gameObject.SetActive(false);
    }

    public void ShowLevelList()
    {
        levelListParent.gameObject.SetActive(true);
    }
}
