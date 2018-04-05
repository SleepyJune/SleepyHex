using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using UnityEngine;
using UnityEngine.UI;

public class LevelEditor : LevelLoader
{
    public Transform templateSlotParent;

    public LevelSelector levelSelector;

    public AmazonS3Helper amazonHelper;
   
    [NonSerialized]
    public UITemplateSlot selectedTemplate;

    [NonSerialized]
    public UIEditorSlot selectedEditorSlot;

    public LineRenderer linePrefab;
    LineRenderer line;

    void Start()
    {
        GenerateTemplateSlots();
        GenerateNewLevel();
    }

    public void GenerateNewLevel()
    {
        int gridColumns = 12;
        int gridRows = 12;

        level = new Level(gridColumns, gridRows);
        level.MakeEmptyLevel();

        gridManager = new GridManager();        
        gridManager.MakeGrid(level, slotPrefab, slotListParent, this);

        SetLevelName(level.levelName);
    }

    void GenerateTemplateSlots()
    {
        for (int i = -1; i < 11; i++)
        {
            var newSlot = Instantiate(slotPrefab, templateSlotParent);
            var template = newSlot.gameObject.AddComponent<UITemplateSlot>();

            template.uiSlot = newSlot;
            template.uiSlot.slot = new Slot(i);
            template.levelEditor = this;
        }
    }

    public void OnChangeLevelName(string name)
    {
        if (level != null)
        {
            level.levelName = name;
        }
    }

    public void OnTemplateSlotPressed(UITemplateSlot slot)
    {
        if (selectedTemplate)
        {
            selectedTemplate.uiSlot.anim.SetBool("selected", false);
        }

        selectedTemplate = slot;
        selectedTemplate.uiSlot.anim.SetBool("selected", true);
    }

    public void OnEditorSlotPressed(UIEditorSlot slot)
    {
        if (selectedEditorSlot)
        {
            selectedEditorSlot.uiSlot.anim.SetBool("selected", false);
        }

        selectedEditorSlot = slot;
        selectedEditorSlot.uiSlot.anim.SetBool("selected", true);

        if (selectedTemplate)
        {
            var number = selectedTemplate.uiSlot.slot.number;
            slot.uiSlot.SetNumber(number);
            level.ChangeSlotNumber(slot.uiSlot.slot);
        }
    }

    public override void InitUISlot(UISlot newSlot)
    {
        var editorSlot = newSlot.gameObject.AddComponent<UIEditorSlot>();
        editorSlot.uiSlot = newSlot;
        editorSlot.levelEditor = this;
    }

    public override void LoadLevelFeatures(Level level)
    {
        level.MakeEmptyLevel();
    }
    
    public void Solve()
    {
        var solver = new LevelSolver(level);

        var bestPath = solver.Solve();
        if (bestPath != null)
        {
            Debug.Log("Slots: " + bestPath.waypoints.Count);
            Debug.Log("Best Score: " + bestPath.GetSum());

            if(line != null)
            {
                Destroy(line.gameObject);
            }

            line = Instantiate(linePrefab, slotListParent);

            foreach(var pathSlot in bestPath.waypoints)
            {
                var uiSlot = gridManager.GetUISlot(pathSlot.slot);

                if(uiSlot != null)
                {
                    line.positionCount += 1;
                    line.SetPosition(line.positionCount - 1, uiSlot.transform.position);
                }                
            }
            

            /*foreach(var slot in bestPath.waypoints)
            {
                Debug.Log(slot.slot.number);
            }*/
        }
        else
        {
            Debug.Log("Unsolvable");
        }       
    }

    public void DeleteLevel()
    {
        var levelText = levelSelector.GetLevel(level.levelName);
        if(levelText != null)
        {
            if (Directory.Exists(DataPath.savePath))
            {
                var filePath = DataPath.savePath + level.levelName + ".json";
                File.Delete(filePath);
            }

            if (levelText.hasWebVersion)
            {
                var webPath = DataPath.webPath + levelText.name + ".json";
                amazonHelper.DeleteObject(level.levelName, webPath);
            }

            LevelSelector.DeleteLevel(level.levelName);
            levelSelector.RefreshList();
        }
    }

    public void Save()
    {
        LevelTextAsset levelText = level.SaveLevel();

        levelText.webText = levelText.text;
        levelText.hasWebVersion = true;
        levelText.webDateModified = DateTime.UtcNow;

        LevelSelector.AddLevel(levelText, true);
        levelSelector.RefreshList();

        var webPath = DataPath.webPath + levelText.name + ".json";

        amazonHelper.PostObject(webPath, levelText.text);

        //saveScreen.SetActive(false);
    }
}
