using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;

using UnityEngine;

using UnityEditor;

using System.Linq;

[CustomEditor(typeof(LevelDatabase))]
public class LevelDatabaseGenerator : Editor
{
    SerializedProperty databaseProperty;

    LevelDatabase database;

    private void OnEnable()
    {
        databaseProperty = serializedObject.FindProperty("levels");

        database = target as LevelDatabase;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        if (GUILayout.Button("Generate"))
        {
            GenerateFromAsset(ref database.levels);
        }

        if (GUILayout.Button("Check Broken Levels"))
        {
            CheckBrokenLevels();
        }

        EditorGUILayout.PropertyField(databaseProperty, true);

        serializedObject.ApplyModifiedProperties();
    }

    public void CheckBrokenLevels()
    {
        foreach(var levelText in database.levels)
        {
            var level = JsonUtility.FromJson<Level>(levelText.text);

            if (level != null)
            {
                for(int i = 1; i < 10; i++)
                {
                    if (level.slots.Count(x => x.number == i) == 1)
                    {
                        var slot = level.slots.First(x => x.number == i);
                        if (slot.hideNumber)
                        {
                            Debug.Log("Broken Level: " + level.levelName);
                            break;
                        }
                    }
                }
            }
        }
    }

    public void GenerateFromAsset(ref LevelTextAsset[] collection)
    {
        //clear all previous
        foreach(var level in collection)
        {
            DestroyImmediate(level, true);
        }

        List<LevelTextAsset> newList = new List<LevelTextAsset>();

        var fileListPath = DataPath.savePath + DataPath.fileListFolder + DataPath.fileListName;

        DirectoryInfo d = new DirectoryInfo(DataPath.savePath);
        foreach (var file in d.GetFiles("*.json"))
        {
            var path = file.FullName;
            string str = File.ReadAllText(path);

            var levelTextAsset = LoadLevelFromString(str);
            
            newList.Add(levelTextAsset);
        }

        newList = newList.OrderBy(level => level.difficulty).ThenBy(level => level.levelID).ToList();

        for(int i = 0; i < newList.Count; i++)
        {
            var level = newList[i];

            if (i != 0)
            {
                level.previousLevel = newList[i - 1].levelName;
            }

            if(i+1 < newList.Count)
            {
                level.nextLevel = newList[i + 1].levelName;
            }

            AssetDatabase.AddObjectToAsset(level, target);
        }

        collection = newList.ToArray();
        EditorUtility.SetDirty(target);
    }    

    public static LevelTextAsset LoadLevelFromString(string str)
    {
        var levelTextAsset = CreateInstance(typeof(LevelTextAsset)) as LevelTextAsset;
                
        levelTextAsset.text = str;

        Level level = Level.LoadLevel(levelTextAsset);
        if (level != null)
        {
            levelTextAsset.name = level.levelName;
            levelTextAsset.levelName = level.levelName;
            levelTextAsset.levelID = level.levelID;
            levelTextAsset.localVersion = level.version;
            levelTextAsset.hasSolution = level.hasSolution;
            levelTextAsset.difficulty = level.difficulty;

            levelTextAsset.dateCreated = level.dateCreated;
            levelTextAsset.dateModified = level.dateModified;

            if (level.hasSolution)
            {
                if (level.solution.bestScore == level.solution.worstScore
                    && level.solution.numBestSolutions != level.solution.numSolutions)
                {
                    levelTextAsset.hasSolution = false;
                }
            }

            return levelTextAsset;
        }

        return null;
    }
}