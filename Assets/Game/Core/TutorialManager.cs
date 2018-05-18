using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public DialogWindow tutorialWindow;

    void Start()
    {
        StartTutorial();
    }

    void StartTutorial()
    {
        if (PlayerPrefs.GetInt("PlayedTutorial", 0) == 0)
        {
            tutorialWindow.Show();
            PlayerPrefs.SetInt("PlayedTutorial", 1);
        }
    }
}