using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

public class SettingsManager : DialogWindow
{
    public static string lastScene;

    public static void LoadScene(string currentScene)
    {
        lastScene = currentScene;
        SceneChanger.ChangeScene("Settings");
    }

    public void Back()
    {
        SceneChanger.ChangeScene(lastScene);
    }
}