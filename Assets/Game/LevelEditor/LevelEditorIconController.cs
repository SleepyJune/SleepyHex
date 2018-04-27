using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using UnityEngine.UI;

public class LevelEditorIconController : MonoBehaviour
{
    public DialogWindow window;

    public Button hasSolution;
    public Button hasRating;
    public Button hasName;
    

    void Start()
    {
        //window = GetComponent<DialogWindow>();
    }

    public void SetIcons(Level level)
    {
        hasSolution.interactable = level.hasSolution;
        hasRating.interactable = level.difficulty != 0;
        hasName.interactable = level.levelName != "New Level";
    }
}
