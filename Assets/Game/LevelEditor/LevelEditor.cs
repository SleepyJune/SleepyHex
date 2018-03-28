﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using UnityEngine;
using UnityEngine.UI;

public class LevelEditor : MonoBehaviour
{
    public Transform templateSlotParent;
    public Transform editorSlotParent;

    public UISlot slotPrefab;

    public InputField levelNameField;
    
    [NonSerialized]
    public UITemplateSlot selectedTemplate;

    [NonSerialized]
    public UIEditorSlot selectedEditorSlot;

    Level level;
    GridManager gridManager;

    string savePath;

    void Start()
    {
        InitializeSavePath();

        GenerateTemplateSlots();
        GenerateNewLevel();
    }

    void InitializeSavePath()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            savePath = Application.persistentDataPath + "/Saves/";
        }
        else
        {
            savePath = Application.dataPath + "/Saves/";
        }
    }

    void GenerateNewLevel()
    {
        int gridColumns = 6;
        int gridRows = 6;

        level = new Level(gridColumns, gridRows);
        level.MakeEmptyLevel();

        gridManager = new GridManager(gridColumns, gridRows);        
        gridManager.MakeGrid(level, slotPrefab, editorSlotParent, this);
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
            level.name = name;
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

    void Clear()
    {
        if (gridManager != null)
        {
            gridManager.DeleteAllSlots();
            gridManager = null;
        }

        level = null;
    }

    public void Load(string path)
    {
        Clear();

        level = LevelLoader.LoadLevel(path);
        if (level != null)
        {
            Debug.Log("Loading " + level.name);

            gridManager = new GridManager(level.columns, level.rows);
            gridManager.MakeGrid(level, slotPrefab, editorSlotParent, this);

            levelNameField.text = level.name;
        }
    }

    public void Save()
    {
        string str = level.SaveLevel();

        if (!Directory.Exists(savePath))
        {
            Directory.CreateDirectory(savePath);
        }

        var filePath = savePath + level.name + ".json";

        File.WriteAllText(filePath, str);

        Debug.Log("Saved to: " + filePath);

        //saveScreen.SetActive(false);
    }
}