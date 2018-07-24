using System.Collections;
using System.Collections.Generic;
using System;

using UnityEngine;

using UnityEditor;

using System.Linq;

[CustomEditor(typeof(DevModeManager))]
[CanEditMultipleObjects]
public class DevModeManagerEditor : Editor
{
    
    public override void OnInspectorGUI()
    {
        //serializedObject.Update();

        DrawDefaultInspector();

        if (GUILayout.Button("Delete Settings"))
        {
            PlayerPrefs.DeleteAll();
        }

        if (GUILayout.Button("Add Hints"))
        {
            int hints = PlayerPrefs.GetInt("Hints", 0) + 1;
            PlayerPrefs.SetInt("Hints", hints);

            if(GameManager.instance != null)
            {
                GameManager.instance.hintManager.UpdateHintText(hints);
            }
        }

        //serializedObject.ApplyModifiedProperties();
    }
}

