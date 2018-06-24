using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using UnityEditor;

public class LevelEditorWindow : EditorWindow
{
    public Level currentLevel;

    public LevelDifficultyGroup currentGroup;

    bool showCurrentLevel = false;

    int selectionGridId = 0;
    string[] selectionGridStrings;

    Vector2 scrollPos;

    GUISkin customSkin;

    public string[] diffStrings = new string[] { "Beginner", "Easy", "Normal", "Hard", "Insane" };

    [MenuItem("Window/Level Editor Window")]
    static void Init()
    {
        EditorWindow.GetWindow(typeof(LevelEditorWindow)).Show();
                
        
    }

    void OnGUI()
    {
        if(customSkin == null)
        {
            //customSkin = EditorGUIUtility.Load("Assets/Prefabs/LevelLoader/SelectionLabel.guiskin") as GUISkin;
        }

        ShowLevelSelector();

    }

    void ShowGroup(LevelDifficultyGroup group)
    {
        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.LabelField("ID" + "\tDifficulty" + "\t\tName", EditorStyles.boldLabel);

        EditorGUILayout.EndHorizontal();

        //EditorGUILayout.LabelField("", GUI.skin.label);

        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, false, true, GUILayout.Width(Screen.width), GUILayout.Height(Screen.height-225));

        var lastSelection = selectionGridId;
        selectionGridId = GUILayout.SelectionGrid(selectionGridId, selectionGridStrings, 1, GUI.skin.label);

        if(lastSelection != selectionGridId)
        {
            //Debug.Log("Selected: " + group.levels[selectionGridId].levelName);

            showCurrentLevel = true;

            var selectedLevel = group.levels[selectionGridId];

            if (LevelEditor.instance)
            {
                currentLevel = LevelEditor.instance.Load(selectedLevel);
            }
            else if (GameManager.instance)
            {
                currentLevel = GameManager.instance.levelManager.Load(selectedLevel);
            }
            else
            {
                currentLevel = Level.LoadLevel(selectedLevel);
            }

            selectionGridId = -1;
        }

        EditorGUILayout.EndScrollView();

        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

        if (currentLevel != null)
        {
            ShowCurrentLevel();
        }
    }
    
    void ShowLevelSelector()
    {
        var database = (LevelDatabase)AssetDatabase.LoadAssetAtPath("Assets/Prefabs/LevelLoader/LevelDatabase.asset", typeof(LevelDatabase));

        if (database != null)
        {
            EditorGUILayout.BeginHorizontal();
            
            foreach (var group in database.difficultyGroups)
            {
                if(currentGroup == null)
                {
                    SetCurrentGroup(group);
                }

                if (GUILayout.Button(group.name))
                {
                    SetCurrentGroup(group);
                }
            }

            EditorGUILayout.EndHorizontal();
            
            ShowGroup(currentGroup);
        }
    }

    void SetCurrentGroup(LevelDifficultyGroup group)
    {
        currentGroup = group;

        List<string> labels = new List<string>();

        for (int i = 0; i < group.levels.Count; i++)
        {
            var level = group.levels[i];

            labels.Add(level.levelID + "\t" + level.difficulty + "\t\t" + level.levelName);
        }

        selectionGridStrings = labels.ToArray();
    }

    void ShowCurrentLevel()
    {
        var level = currentLevel; //LevelEditor.instance.GetCurrentLevel();

        if (level != null)
        {
            /*if (GUILayout.Button("Back", GUILayout.Width(100)))
            {
                showCurrentLevel = false;
            }*/

            //GUILayout.Label("Level Editor Window", EditorStyles.boldLabel);

            //GUILayout.Space(20);

            level.levelName = EditorGUILayout.TextField("Name: ", level.levelName);
            level.levelID = EditorGUILayout.IntField("ID: ", level.levelID);

            GUILayout.Space(20);

            level.difficulty = EditorGUILayout.FloatField("Difficulty: ", level.difficulty);

            PuzzleDifficulty bigDiff = (PuzzleDifficulty)Math.Floor(level.difficulty);
            level.difficulty = level.difficulty % 1 + (int)(PuzzleDifficulty)EditorGUILayout.EnumPopup("Group: ", bigDiff);

            var smallDiff = (float)Math.Round(EditorGUILayout.Slider("Decimal: ", level.difficulty % 1, 0, .9f), 1);

            if (Math.Abs(level.difficulty % 1 - smallDiff) >= .1)
            {
                level.difficulty = (int)level.difficulty + smallDiff;
            }

            //GUILayout.Space(20);

            if (GUILayout.Button("Save", GUILayout.Width(100)))
            {
                level.SaveLevel(false);

                AssetDatabase.Refresh();
            }
        }
    }

    void Update()
    {
        if (LevelEditor.instance)
        {
            var level = LevelEditor.instance.GetCurrentLevel();

            if (level != null && currentLevel != level)
            {
                currentLevel = level;

                Repaint();
            }
        }
    }

}
