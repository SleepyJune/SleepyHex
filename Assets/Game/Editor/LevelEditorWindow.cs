using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using UnityEditor;

public class LevelEditorWindow : EditorWindow
{
    public Level level;
    
    [MenuItem("Window/Level Editor Window")]
    static void Init()
    {
        EditorWindow.GetWindow(typeof(LevelEditorWindow)).Show();
    }

    void OnGUI()
    {

    }
}
