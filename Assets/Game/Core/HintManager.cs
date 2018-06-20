using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

public class HintManager : MonoBehaviour
{
    [NonSerialized]
    public int totalHintsUsed = 0;

    [NonSerialized]
    public int hintsUsed = 0;

    public LevelLoader levelLoader;

    public LineRenderer linePrefab;
    public GameObject startIconPrefab;

    GameObject startIcon;
    LineRenderer line;

    void Start()
    {

    }

    public void ResetHintsUsed()
    {
        hintsUsed = 0;
    }

    public void ShowSolution()
    {
        var current = LevelManager.currentLevel;

        if (current != null)
        {
            if (current.solution != null)
            {              
                var bestPath = current.solution.bestPath;
                var numHex = bestPath.Length;

                totalHintsUsed += 1;

                hintsUsed += 1;

                if(hintsUsed == 1)
                {
                    var numHexShown = (int)Math.Min(numHex, Math.Ceiling(numHex / 3.0));

                    DrawPathLine(bestPath.Take(numHexShown));
                }
                else if(hintsUsed == 2)
                {
                    var numHexShown = (int)Math.Min(numHex, Math.Ceiling(2 * numHex / 3.0));

                    DrawPathLine(bestPath.Take(numHexShown));
                }
                else
                {
                    DrawPathLine(bestPath);
                }

            }
        }
    }

    public void DrawPathLine(IEnumerable<Vector3> path)
    {
        GameManager.instance.pathManager.ClearPath();

        if (startIcon != null)
        {
            Destroy(startIcon);

            startIcon = null;
        }

        if (line != null)
        {
            Destroy(line.gameObject);
        }

        line = Instantiate(linePrefab, levelLoader.slotListParent);

        foreach (var position in path)
        {
            var uiSlot = levelLoader.GetGridManager().GetUISlot(position);

            if (uiSlot != null)
            {
                line.positionCount += 1;
                line.SetPosition(line.positionCount - 1, uiSlot.transform.position);

                if (startIcon == null)
                {
                    startIcon = Instantiate(startIconPrefab, levelLoader.slotListParent);
                    startIcon.transform.position = uiSlot.transform.position;
                }
            }
        }
    }

}
