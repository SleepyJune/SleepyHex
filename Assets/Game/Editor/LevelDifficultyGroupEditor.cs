using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;

using UnityEngine;

using UnityEditor;
using UnityEditorInternal;

using System.Linq;

[CustomEditor(typeof(LevelDifficultyGroup))]
public class LevelDifficultyGroupEditor : Editor
{
    private ReorderableList list;

    SerializedProperty levelsProperty;

    private void OnEnable()
    {

        levelsProperty = serializedObject.FindProperty("levels");

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
                new Rect(rect.x, rect.y, 120, EditorGUIUtility.singleLineHeight),
                element.FindProperty("levelName"), GUIContent.none);

            EditorGUI.PropertyField(
                new Rect(rect.x + 120, rect.y, rect.width - 120 - 60, EditorGUIUtility.singleLineHeight),
                element.FindProperty("levelID"), GUIContent.none);

            EditorGUI.PropertyField(
                new Rect(rect.x + rect.width - 60, rect.y, 60, EditorGUIUtility.singleLineHeight),
                element.FindProperty("difficulty"), GUIContent.none);
        };*/
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        //list.DoLayoutList();

        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.LabelField("ID" + "\tName", EditorStyles.boldLabel);

        GUILayout.FlexibleSpace();

        EditorGUILayout.LabelField("Difficulty", EditorStyles.boldLabel);

        EditorGUILayout.EndHorizontal();

        for (int i=0;i< levelsProperty.arraySize; i++)
        {
            var tempProp = levelsProperty.GetArrayElementAtIndex(i);

            SerializedObject element = new SerializedObject(tempProp.objectReferenceValue);

            //rect.y += 2;

            EditorGUILayout.BeginHorizontal();
            
            EditorGUILayout.LabelField(element.FindProperty("levelID").intValue + "\t" + element.FindProperty("levelName").stringValue);

            GUILayout.FlexibleSpace();

            EditorGUILayout.LabelField(""+element.FindProperty("difficulty").floatValue);

            EditorGUILayout.EndHorizontal();
                        
            //GUILayout.Button(element.FindProperty("levelName").stringValue);
        }


        serializedObject.ApplyModifiedProperties();
    }
}