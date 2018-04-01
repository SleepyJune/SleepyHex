using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

public class DataPath : MonoBehaviour
{
    public static string webPath = "SleepyHex/Resources/Levels/";

    public static string savePath
    {
        get
        {
            if (Application.platform == RuntimePlatform.Android
             || Application.platform == RuntimePlatform.WebGLPlayer)
            {
                return Application.persistentDataPath + "/Resources/Levels/";
            }
            else
            {
                return Application.dataPath + "/Resources/Levels/";
            }
        }
    }
}
