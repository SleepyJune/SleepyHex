using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;

using UnityEngine;

using UnityEditor;
using UnityEditorInternal;

using System.Linq;

[CustomEditor(typeof(LevelDatabase))]
public class LevelDatabaseGenerator : Editor
{
    SerializedProperty databaseProperty;

    LevelDatabase database;

    private ReorderableList list;

    Dictionary<string, Editor> groupEditors = new Dictionary<string, Editor>();
    List<bool> groupFoldout = new List<bool>();

    private void OnEnable()
    {
        databaseProperty = serializedObject.FindProperty("difficultyGroups");

        database = target as LevelDatabase;

        /*list = new ReorderableList(serializedObject,
                serializedObject.FindProperty("levels"),
                true, true, true, true);

        list.drawElementCallback =

        (Rect rect, int index, bool isActive, bool isFocused) =>
        {
            var tempProp = list.serializedProperty.GetArrayElementAtIndex(index);

            SerializedObject element = new SerializedObject(tempProp.objectReferenceValue);

            rect.y += 2;

            //Debug.Log(element.FindProperty("difficulty").floatValue);

            EditorGUI.PropertyField(
                new Rect(rect.x, rect.y, 60, EditorGUIUtility.singleLineHeight),
                element.FindProperty("levelName"), GUIContent.none);

            EditorGUI.PropertyField(
                new Rect(rect.x + 60, rect.y, rect.width - 60 - 30, EditorGUIUtility.singleLineHeight),
                element.FindProperty("levelID"), GUIContent.none);

            EditorGUI.PropertyField(
                new Rect(rect.x + rect.width - 30, rect.y, 30, EditorGUIUtility.singleLineHeight),
                element.FindProperty("difficulty"), GUIContent.none);
        };*/

        foreach(var name in Enum.GetNames(typeof(PuzzleDifficulty)))
        {            
            groupEditors.Add(name, null);
            groupFoldout.Add(false);
        }
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        if (GUILayout.Button("Generate"))
        {
            GenerateFromAsset(ref database);
        }

        if (GUILayout.Button("Write Level IDs"))
        {
            WriteLevelID();
        }

        if (GUILayout.Button("Check Broken Levels"))
        {
            CheckBrokenLevels();
        }

        //EditorGUILayout.PropertyField(databaseProperty, true);

        for(int i =0;i<database.difficultyGroups.Length;i++)
        {
            var group = database.difficultyGroups[i];

            groupFoldout[i] = EditorGUILayout.Foldout(groupFoldout[i], group.groupName);
            if (groupFoldout[i])
            {
                Editor groupEditor;
                if(groupEditors.TryGetValue(group.groupName, out groupEditor))
                {
                    if(groupEditor == null)
                    {
                        groupEditor = CreateEditor(group);
                        groupEditors[group.groupName] = groupEditor;
                    }

                    groupEditor.OnInspectorGUI();
                }
            }
        }

        //list.DoLayoutList();

        serializedObject.ApplyModifiedProperties();
    }

    void WriteLevelID()
    {
        foreach (var group in database.difficultyGroups)
        {
            var comparer = new NaturalComparer();

            int levelID = 1;

            /*if(group.groupName != "Insane")
            {
                continue;
            }*/

            foreach (var levelText in group.levels.OrderBy(level => level.difficulty)
                                                    .ThenBy(level => level.levelName, comparer))
            {
                var level = Level.LoadLevel(levelText);
                if (level != null)
                {
                    if(level.levelID != levelID)
                    {
                        level.levelID = levelID;
                        level.SaveLevel(false);
                    }

                    levelID = levelID + 1;
                }
            }
        }
    }

    public void CheckBrokenLevels()
    {
        foreach (var group in database.difficultyGroups)
        {
            foreach (var levelText in group.levels)
            {
                var level = JsonUtility.FromJson<Level>(levelText.text);

                if (level != null)
                {
                    for (int i = 1; i < 10; i++)
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
    }

    public static void DeleteAllSubAssets(UnityEngine.Object obj)
    {
        var path = AssetDatabase.GetAssetPath(obj);
        var assets = AssetDatabase.LoadAllAssetsAtPath(path);

        foreach(var asset in assets)
        {
            if(asset.name != obj.name)
            {
                //Debug.Log(asset.name);
                DestroyImmediate(asset, true);
            }
        }
    }

    public static void GenerateFromAsset(ref LevelDatabase database)
    {
        DeleteAllSubAssets(database);

        //clear all previous
        /*foreach (var group in database.difficultyGroups)
        {
            foreach (var level in group.levels)
            {
                DestroyImmediate(level, true);
            }
        }*/

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

        //Dictionary<int, List<LevelTextAsset>> newGroupsList = new Dictionary<int, List<LevelTextAsset>>();
        Dictionary<int, LevelDifficultyGroup> newGroups = new Dictionary<int, LevelDifficultyGroup>();

        //var levelTextAsset = CreateInstance(typeof(LevelTextAsset)) as LevelTextAsset;

        for (int i = 0; i < newList.Count; i++)
        {
            var level = newList[i];

            var groupKey = (int)Math.Floor(level.difficulty);
            if (!newGroups.ContainsKey(groupKey))
            {
                //newGroupsList.Add(groupKey, new List<LevelTextAsset>());

                var newGroup = CreateInstance(typeof(LevelDifficultyGroup)) as LevelDifficultyGroup;

                newGroup.name = ((PuzzleDifficulty)groupKey).ToString();
                newGroup.groupName = ((PuzzleDifficulty)groupKey).ToString();

                newGroups.Add(groupKey, newGroup);
                AssetDatabase.AddObjectToAsset(newGroup, database);
            }
                        
            if (i != 0)
            {
                level.previousLevel = newList[i - 1].levelName;
            }

            if (i + 1 < newList.Count)
            {
                level.nextLevel = newList[i + 1].levelName;
            }

            newGroups[groupKey].levels.Add(level);

            AssetDatabase.AddObjectToAsset(level, newGroups[groupKey]);
        }

        //collection = newList.ToArray();
        database.difficultyGroups = newGroups.Values.ToArray();

        EditorUtility.SetDirty(database);
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