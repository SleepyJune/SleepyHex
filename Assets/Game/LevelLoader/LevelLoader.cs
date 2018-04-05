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

    void Clear()
    {
        if (gridManager != null)
        {
            gridManager.DeleteAllSlots();
            gridManager = null;
        }

        level = null;
    }

    public Level Load(LevelTextAsset levelText)
    {
        Clear();

        level = Level.LoadLevel(levelText);
        if (level != null)
        {
            Debug.Log("Loading " + level.levelName);

            LoadLevelFeatures(level);

            gridManager = new GridManager();
            gridManager.MakeGrid(level, slotPrefab, slotListParent, this);

            SetLevelName(level.levelName);

        }

        return level;
    }

    public GridManager GetGridManager()
    {
        return gridManager;
    }

    public void SetLevelName(string name)
    {
        levelNameField.text = name;
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