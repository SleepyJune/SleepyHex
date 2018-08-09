using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    public DialogWindow tutorialWindow;

    GameObject tutorialOverlay;
                
    public Text tutorialText;
    public Animator anim;

    public AnimatorStateManager animManager;

    public TutorialLevel[] levels;
        
    void Start()
    {
       
    }

    public void ShowTutorial(Level current)
    {       
        var tutorial = levels.FirstOrDefault(level => level.levelName == current.levelName);
                
        if (tutorial != null)
        {
            
            if(tutorialOverlay != null)
            {
                Destroy(tutorialOverlay);
            }

            if(tutorial.overlay != null)
            {
                tutorialOverlay = Instantiate(tutorial.overlay, tutorialWindow.transform);
            }

            tutorialWindow.Show();

            //Invoke("DelayedShowWindow", .05f);

            //tutorialText.text = tutorial.text;            

        }
        else
        {
            tutorialWindow.Close();
        }
    }
}