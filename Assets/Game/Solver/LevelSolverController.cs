using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using UnityEngine.UI;

public class LevelSolverController : MonoBehaviour
{    
    public Slider slider;

    public GameObject progressPanel;
        
    public Text timeText;

    LevelSolver solver;

    LevelEditor levelEditor;
    Level level;

    LevelSolutionViewer solutionViewer;

    void Update()
    {
        if (progressPanel.activeInHierarchy)
        {
            slider.value = solver.GetProgress();

            DateTime mydate = new DateTime((DateTime.Now.Subtract(solver.startTime)).Ticks);
            timeText.text = mydate.ToString(("mm:ss"));
        }
    }

    public void Solve(Level level, LevelEditor levelEditor, LevelSolutionViewer levelSolutionViewer)
    {
        this.levelEditor = levelEditor;
        this.level = level;
        this.solutionViewer = levelSolutionViewer;

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

        if (solution != null && solution.version == level.version && solution.bestScore > 0)
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
                level.solution = solution;
                levelEditor.Save(false); //save solution
            }
        }

        if (solution != null)
        {
            Debug.Log("Best Score: " + solution.bestScore);
            
            solutionViewer.ShowSolution(solution);
        }
        else
        {
            Debug.Log("Unsolvable");
        }
    }
}
