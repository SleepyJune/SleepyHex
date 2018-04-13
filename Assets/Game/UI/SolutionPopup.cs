using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using UnityEngine.UI;

public class SolutionPopup : MonoBehaviour
{
    public Text solutionText;

    public void SetType(SolutionType solutionType)
    {
        if(solutionType == SolutionType.BestSolution)
        {
            solutionText.text = "Best Solution!";
        }
        else if (solutionType == SolutionType.Solution)
        {
            solutionText.text = "A Solution";
        }
        else if (solutionType == SolutionType.WorstSolution)
        {
            solutionText.text = "Worst Solution";
        }
        else
        {
            solutionText.text = "Not a Solution";
        }
    }
}
