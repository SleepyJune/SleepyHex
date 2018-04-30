using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using UnityEngine.UI;

class LevelSolverController : MonoBehaviour
{    
    public enum SolveType
    {
        Ovewrite,
        Solve,
        FromData
    }

    public Slider slider;

    public GameObject progressPanel;
        
    public Text timeText;

    LevelSolver solver;

    LevelEditor levelEditor;
    Level level;

    EditorSolutionViewer solutionViewer;

    int solveType;

    void Update()
    {
        if (progressPanel.activeInHierarchy)
        {
            slider.value = solver.GetProgress();

            DateTime mydate = new DateTime((DateTime.Now.Subtract(solver.startTime)).Ticks);
            timeText.text = mydate.ToString(("mm:ss"));
        }
    }

    public void Solve(Level level, LevelEditor levelEditor, EditorSolutionViewer levelSolutionViewer, int solveType = 0)
    {
        this.levelEditor = levelEditor;
        this.level = level;
        this.solutionViewer = levelSolutionViewer;
        this.solveType = solveType;

        StartCoroutine("SolverCoroutine");
    }

    public void Abort()
    {
        solver.Abort();
    }

    void OnApplicationQuit()
    {
        if (solver != null)
        {
            solver.Abort();
        }
    }

    void OnDestroy()
    {
        if (solver != null)
        {
            solver.Abort();
        }
    }

    public IEnumerator SolverCoroutine()
    {
        LevelSolution solution = level.solution;

        if (solution != null && level.hasSolution && solveType == (int)SolveType.FromData)
        {
            progressPanel.SetActive(false);
        }
        else
        {
            solver = new LevelSolver(level);
            solver.Start();

            yield return StartCoroutine(solver.WaitFor());

            progressPanel.SetActive(false);

            solution = solver.GetSolution();

            if (solution != null)
            {
                solutionViewer.SetSolvedPaths(solver.GetSolvedPaths());
                //solutionViewer.SetAdditionalStats(solver.slotsVisited);

                level.solution = solution;
                level.isSolvedInEditor = true;

                if (solveType == (int)SolveType.Ovewrite)
                {
                    levelEditor.Save(false); //save solution
                }
            }
        }

        if (solution != null)
        {
            Debug.Log("Best Score: " + solution.bestScore);
            
            solutionViewer.ShowSolution(solution);
            //solutionViewer.SetPuzzleRating(level);
        }
        else
        {
            Debug.Log("Unsolvable");
        }
    }
}
