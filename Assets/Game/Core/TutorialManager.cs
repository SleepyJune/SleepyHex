﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    public DialogWindow tutorialWindow;

    public Text tutorialText;
    public Animator anim;

    public TutorialLevel[] levels;

    void Start()
    {
        
    }

    public void ShowTutorial(Level current)
    {
        var tutorial = levels.FirstOrDefault(level => level.levelName == current.levelName);

        if (tutorial != null)
        {
            tutorialWindow.Show();

            tutorialText.text = tutorial.text;

            //Debug.Log("tut: " + tutorial.tutorialId);

            anim.SetInteger("tutorialID", tutorial.tutorialId);
            //anim.Play("tutorial" + tutorial.tutorialId);
        }
        else
        {
            tutorialWindow.Close();
        }
    }
}