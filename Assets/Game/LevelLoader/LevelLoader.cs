using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using UnityEngine.UI;

public class LevelLoader : MonoBehaviour
{
    public UISlot slotPrefab;
    public Transform slotListParent;

    protected Level level;
    protected GridManager gridManager;

    public InputField levelNameField;
    public InputField levelIDField;

    public LevelSelector levelSelector;

    protected AmazonS3Helper amazonHelper;

    void Clear()
    {
        if (gridManager != null)
        {
            gridManager.DeleteAllSlots();
            gridManager = null;
        }

        level = null;
    }

    public void LoadLevel(string name)
    {
        LevelTextAsset levelText;
        if (LevelSelector.levelDatabase.TryGetValue(name, out levelText))
        {
            Load(levelText);
        }
    }

    public Level Load(LevelTextAsset levelText)
    {
        Clear();

        level = Level.LoadLevel(levelText);
        if (level != null)
        {
            Debug.Log("Loading " + level.levelName);

            PlayerPrefs.SetString("currentLevel", level.levelName);
            LevelManager.levelNameToLoad = level.levelName;
            LevelManager.currentLevel = level;

            LoadLevelFeatures(level);

            gridManager = new GridManager();
            gridManager.MakeGrid(level, slotPrefab, slotListParent, this);

            SetLevelName(level.levelName);
            SetLevelID(level.levelID);
        }

        return level;
    }

    public void Save(bool modified, bool softLoad)
    {
        LevelTextAsset levelText = level.SaveLevel(modified);

        var metadata = level.GetMetadata();

        LevelSelector.AddLevel(levelText, true);
        levelSelector.RefreshList();

        var webPath = DataPath.webPath + levelText.name + ".json";
        amazonHelper.PostObject(webPath, levelText.text, metadata);

        LevelVersion version = new LevelVersion()
        {
            levelName = level.levelName,
            levelID = level.levelID,
            version = level.version,
            dateModified = level.dateModified,
            dateCreated = level.dateCreated,
            solved = level.hasSolution,
            difficulty = level.difficulty,
        };

        //Debug.Log(levelText.dateModified.GetUnixEpoch());

        amazonHelper.UploadLevelVersion(version);

        //saveScreen.SetActive(false);

        if (softLoad)
        {
            SoftLoad(levelText);
        }
    }

    public Level SoftLoad(LevelTextAsset levelText)
    {
        level = Level.LoadLevel(levelText);

        return level;
    }

    public GridManager GetGridManager()
    {
        return gridManager;
    }

    public void SetLevelName(string name)
    {
        if (levelNameField)
        {
            levelNameField.text = name;
        }
    }

    public void SetLevelID(int id)
    {
        if (levelIDField)
        {
            levelIDField.text = id.ToString();
        }
    }

    public Level GetCurrentLevel()
    {
        return level;
    }

    public virtual void LoadLevelFeatures(Level level)
    {

    }

    public virtual bool isValidSlot(Slot slot)
    {
        return true;
    }

    public virtual void InitUISlot(UISlot slot)
    {

    }
}