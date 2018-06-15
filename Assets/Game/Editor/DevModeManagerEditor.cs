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


        //serializedObject.ApplyModifiedProperties();
    }
}

