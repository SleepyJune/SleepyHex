using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using UnityEngine.UI;

public class LevelSolverController : MonoBehaviour
{
    public LineRenderer linePrefab;
    public Slider slider;

    public GameObject progressPanel;

    public GameObject startIconPrefab;

    public Text timeText;

    GameObject startIcon;

    LineRenderer line;

    LevelSolver solver;

    LevelEditor levelEditor;
    Level level;

    void Update()
    {
        if (progressPanel.activeInHierarchy)
        {
            slider.value = solver.GetProgress();

            DateTime mydate = new DateTime((DateTime.Now.Subtract(solver.startTime)).Ticks);
            timeText.text = mydate.ToString(("mm:ss"));
        }
    }

    public void Solve(Level level, LevelEditor levelEditor)
    {
        this.levelEditor = levelEditor;
        this.level = level;

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

        if (solution != null && solution.dateModified == level.dateModified)
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
                levelEditor.Save(); //save solution
            }
        }

        if (solution != null)
        {
            Debug.Log("Best Score: " + solution.bestScore);            

            if (line != null)
            {
                Destroy(line.gameObject);
            }

            line = Instantiate(linePrefab, transform);
            

            foreach (var position in solution.bestPath)
            {
                Slot dummySlot = new Slot(position);

                var uiSlot = levelEditor.GetGridManager().GetUISlot(dummySlot);

                if (uiSlot != null)
                {
                    line.positionCount += 1;
                    line.SetPosition(line.positionCount - 1, uiSlot.transform.position);

                    if(startIcon == null)
                    {
                        startIcon = Instantiate(startIconPrefab, transform);
                        startIcon.transform.position = uiSlot.transform.position;
                    }
                }
            }


            /*foreach(var slot in bestPath.waypoints)
            {
                Debug.Log(slot.slot.number);
            }*/
        }
        else
        {
            Debug.Log("Unsolvable");
        }
    }
}
