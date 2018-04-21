using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

using UnityEngine;
using UnityEngine.UI;

public class LevelManager : LevelLoader
{
    public Button solveButton;
    public Button rateButton;

    public LevelSolutionViewer levelSolutionViewer;

    public Transform dialogParent;

    public SolutionPopup solutionPopupPrefab;

    public Image backgroundImage;

    public static Level currentLevel = null;
    public static string levelNameToLoad = null;

    void Start()
    {
        if(Application.platform == RuntimePlatform.WindowsEditor)
        {
            amazonHelper = AmazonS3Helper.instance;

            if (rateButton)
            {
                rateButton.interactable = true;
            }
        }
        else
        {
            if (rateButton)
            {
                rateButton.interactable = false;
            }
        }

        if(levelNameToLoad != null)
        {
            levelSelector.LoadLevel(levelNameToLoad);
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
        
        ChangeRandomColor();
    }

    void ChangeRandomColor()
    {
        float h;
        float s;
        float v;

        Color.RGBToHSV(backgroundImage.color, out h, out s, out v);

        h = UnityEngine.Random.Range(0f, 1f);

        var color = Color.HSVToRGB(h, s, v);
        color.a = .4f;

        backgroundImage.color = color;
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

                Score.current = new Score(level, score);
                SceneChanger.ChangeScene("Score");

                return;

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