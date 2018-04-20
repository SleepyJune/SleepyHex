using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using UnityEngine.UI;

public class LevelManager : LevelLoader
{
    public Button solveButton;
    public Button checkButton;
    public Button rateButton;

    public LevelSolutionViewer levelSolutionViewer;

    public Transform dialogParent;

    public SolutionPopup solutionPopupPrefab;

    void Start()
    {
        if(Application.platform == RuntimePlatform.WindowsEditor)
        {
            rateButton.interactable = true;
        }
        else
        {
            rateButton.interactable = false;
        }
    }

    public override bool isValidSlot(Slot slot)
    {
        return slot.number >= 0;
    }

    public override void InitUISlot(UISlot newSlot)
    {
        var gameSlot = newSlot.gameObject.AddComponent<UIGameSlot>();
        gameSlot.uiSlot = newSlot;
        gameSlot.pathManager = GameManager.instance.pathManager;
    }

    public override void LoadLevelFeatures(Level level)
    {
        solveButton.interactable = level.hasSolution;
        checkButton.interactable = level.hasSolution;
    }

    public void Solve()
    {
        if (level.hasSolution)
        {
            levelSolutionViewer.ShowSolution(level.solution);
        }
    }

    public void Check()
    {
        if(level.hasSolution)
        {
            var path = GameManager.instance.pathManager.GetPath();
            if(path != null)
            {
                var solution = level.solution;
                var score = path.GetTotalPoints();

                var popup = Instantiate(solutionPopupPrefab, dialogParent);

                if (path.isSolution(level))
                {
                    if (score == solution.bestScore)
                    {
                        popup.SetType(SolutionType.BestSolution);
                    }
                    else if (score == solution.worstScore)
                    {
                        popup.SetType(SolutionType.WorstSolution);
                    }
                    else
                    {
                        popup.SetType(SolutionType.Solution);
                    }
                }
                else
                {
                    popup.SetType(SolutionType.NoSolution);
                }
            }
        }
    }
}