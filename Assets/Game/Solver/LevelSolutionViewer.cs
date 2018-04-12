using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using UnityEngine.UI;

public class LevelSolutionViewer : MonoBehaviour
{
    public LevelEditor levelEditor;

    public LineRenderer linePrefab;
    public GameObject startIconPrefab;

    public Text solutionText;

    public GameObject solutionPanel;


    GameObject startIcon;
    LineRenderer line;

    public void ShowSolution(LevelSolution solution)
    {
        if (solution != null)
        {
            Debug.Log("Best Score: " + solution.bestScore);

            SetSolutionText(solution);

            if (line != null)
            {
                Destroy(line.gameObject);
            }

            line = Instantiate(linePrefab, levelEditor.slotListParent);


            foreach (var position in solution.bestPath)
            {
                Slot dummySlot = new Slot(position);

                var uiSlot = levelEditor.GetGridManager().GetUISlot(dummySlot);

                if (uiSlot != null)
                {
                    line.positionCount += 1;
                    line.SetPosition(line.positionCount - 1, uiSlot.transform.position);

                    if (startIcon == null)
                    {
                        startIcon = Instantiate(startIconPrefab, levelEditor.slotListParent);
                        startIcon.transform.position = uiSlot.transform.position;
                    }
                }
            }
        }
    }

    public void SetSolutionText(LevelSolution solution)
    {
        string finalText = "";

        finalText += "Best: " + solution.bestScore + "\n";
        finalText += "Worst: " + solution.worstScore + "\n";

        finalText += "\n";

        finalText += "Solutions: " + solution.numSolutions + "\n";
        finalText += "Best Solutions: " + solution.numBestSolutions + "\n";

        solutionText.text = finalText;

        solutionPanel.SetActive(true);
    }

    public void Close()
    {
        solutionPanel.SetActive(false);
    }
}
