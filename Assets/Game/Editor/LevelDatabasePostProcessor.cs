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

        /*foreach (string str in importedAssets)
        {
            Debug.Log("Reimported Asset: " + str);            
        }

        foreach (string str in deletedAssets)
        {
            Debug.Log("Deleted Asset: " + str);
        }*/

        LevelDatabaseGenerator.GenerateFromAsset(ref database);

        /*for (int i = 0; i < movedAssets.Length; i++)
        {
            Debug.Log("Moved Asset: " + movedAssets[i] + " from: " + movedFromAssetPaths[i]);
        }*/
    }
}