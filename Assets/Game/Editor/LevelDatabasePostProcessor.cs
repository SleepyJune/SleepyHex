using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;

using UnityEngine;
using UnityEditor;

using System.Linq;

class LevelDatabasePostProcessor : AssetPostprocessor
{
    static LevelDatabase database;

    static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        if(database == null)
        {
            database = (LevelDatabase)AssetDatabase.LoadAssetAtPath("Assets/Prefabs/LevelLoader/LevelDatabase.asset", typeof(LevelDatabase));
        }

        bool databaseModified = false;

        foreach (string str in importedAssets)
        {
            if (str.StartsWith("Assets/Resources/Levels/"))
            {
                databaseModified = true;
                break;
            }

            //Debug.Log("Reimported Asset: " + str);            
        }

        foreach (string str in deletedAssets)
        {
            if (str.StartsWith("Assets/Resources/Levels/"))
            {
                databaseModified = true;
                break;
            }

            //Debug.Log("Deleted Asset: " + str);
        }

        if (databaseModified)
        {
            LevelDatabaseGenerator.GenerateFromAsset(ref database);
        }

        /*for (int i = 0; i < movedAssets.Length; i++)
        {
            Debug.Log("Moved Asset: " + movedAssets[i] + " from: " + movedFromAssetPaths[i]);
        }*/
    }
}