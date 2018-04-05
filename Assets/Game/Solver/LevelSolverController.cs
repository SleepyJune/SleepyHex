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

    LineRenderer line;

    LevelSolver solver;

    LevelEditor levelEditor;
    Level level;

    void Update()
    {
        if (progressPanel.activeInHierarchy)
        {
            slider.value = solver.GetProgress();
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
        solver = new LevelSolver(level);

        solver.Start();

        yield return StartCoroutine(solver.WaitFor());

        progressPanel.SetActive(false);

        var bestPath = solver.GetBestPath();
        if (bestPath != null)
        {
            Debug.Log("Slots: " + bestPath.waypoints.Count);
            Debug.Log("Best Score: " + bestPath.GetSum());

            if (line != null)
            {
                Destroy(line.gameObject);
            }

            line = Instantiate(linePrefab, transform);

            foreach (var pathSlot in bestPath.waypoints)
            {
                var uiSlot = levelEditor.GetGridManager().GetUISlot(pathSlot.slot);

                if (uiSlot != null)
                {
                    line.positionCount += 1;
                    line.SetPosition(line.positionCount - 1, uiSlot.transform.position);
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
