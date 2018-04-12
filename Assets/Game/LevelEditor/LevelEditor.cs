using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LevelEditor : LevelLoader
{
    public Transform templateSlotParent;

    public LevelSelector levelSelector;

    public AmazonS3Helper amazonHelper;
   
    [NonSerialized]
    public UITemplateSlot selectedTemplate;

    [NonSerialized]
    public UIEditorSlot selectedEditorSlot;

    public LevelSolverController levelSolverPrefab;

    public Button solveButton;

    public LevelSolutionViewer levelSolutionViewer;

    public DialogWindow overwritePanel;

    void Start()
    {
        GenerateTemplateSlots();
        GenerateNewLevel();

        if(Application.platform == RuntimePlatform.WindowsEditor)
        {
            solveButton.gameObject.SetActive(true);
        }
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
        for (int i = -2; i < 11; i++)
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

            level.modified = true;

            if (number >= -1)
            {
                slot.uiSlot.SetNumber(number);
                level.ChangeSlotNumber(slot.uiSlot.slot);
            }
            else
            {
                if(number == -2)
                {
                    slot.uiSlot.ToggleText();
                    level.ChangeHideText(slot.uiSlot.slot);
                }
            }
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

        LevelSolution solution = level.solution;

        if (Application.platform == RuntimePlatform.WindowsEditor)
        {
            solveButton.interactable = true;
        }
        else
        {
            if (solution != null && solution.version == level.version && solution.bestScore > 0)
            {
                solveButton.interactable = true;
            }
            else
            {
                solveButton.interactable = false;
            }
        }
    }

    public void Solve()
    {
        LevelSolution solution = level.solution;

        if (level.hasSolution && !level.modified)
        {
            levelSolutionViewer.ShowSolution(solution);
        }
        else
        {
            if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                if (level.modified)
                {
                    Save();
                }
                else
                {
                    var solver = Instantiate(levelSolverPrefab, slotListParent);
                    solver.Solve(level, this, levelSolutionViewer);
                }                
            }
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

            if (levelText.webVersion >= 0)
            {
                var webPath = DataPath.webPath + levelText.name + ".json";
                amazonHelper.DeleteObject(level.levelName, webPath);
            }

            LevelSelector.DeleteLevel(level.levelName);
            levelSelector.SaveLevelList();
            levelSelector.RefreshList();
        }
    }

    public void Save()
    {
        if (level.modified)
        {
            overwritePanel.Show();
        }
        else
        {
            Save(false);
        }        
    }

    public void Save(bool modified)
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
            version = level.version,
            dateModified = level.dateModified.ToString(),
            solved = level.hasSolution,
        };

        amazonHelper.UploadLevelVersion(version);

        if (modified)
        {
            levelSelector.SaveLevelList();
        }

        //saveScreen.SetActive(false);

        SoftLoad(levelText);
    }
}
