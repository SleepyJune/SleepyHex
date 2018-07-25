using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using UnityEngine.UI;

public class HintManager : MonoBehaviour
{
    [NonSerialized]
    public int totalHintsUsed = 0;

    [NonSerialized]
    public int hintsUsed = 0;

    public Text numHintCount;

    public LevelLoader levelLoader;

    public LineRenderer linePrefab;
    public GameObject startIconPrefab;

    GameObject startIcon;
    LineRenderer line;

    void Awake()
    {
        SetDefaultHints();
    }

    void Start()
    {
        
    }

    void OnEnable()
    {
        UpdateHintText(GetPrefabHints());
    }

    void SetDefaultHints()
    {
        if (!PlayerPrefs.HasKey("Hints"))
        {
            PlayerPrefs.SetInt("Hints", 10);
        }
    }

    public void UpdateHintText(int hints)
    {
        numHintCount.text = hints.ToString();
    }

    public int GetPrefabHints()
    {        
        return PlayerPrefs.GetInt("Hints", 0);
    }

    public int AddHint(int count = 1)
    {
        int hints = PlayerPrefs.GetInt("Hints", 0) + count;
        PlayerPrefs.SetInt("Hints", hints);
        return hints;        
    }

    public int ReduceHint()
    {
        int hints = PlayerPrefs.GetInt("Hints", 0) - 1;
        PlayerPrefs.SetInt("Hints", hints);

        UpdateHintText(hints);
        return hints;
    }

    public void ResetHintsUsed()
    {
        hintsUsed = 0;
    }

    public void ShowSolution()
    {
        var hints = GetPrefabHints();

        if(hints <= 0) //no more hints left
        {
            GameManager.instance.characterController.TriggerSpeechBubble(SpeechBubbleIndex.NoMoreHints);
            return;
        }

        var current = LevelManager.currentLevel;

        if (current != null)
        {
            if(current.GetPuzzleDifficulty() == PuzzleDifficulty.Insane)
            {
                GameManager.instance.characterController.TriggerSpeechBubble(SpeechBubbleIndex.NoInsaneHints);
                return;
            }

            if (current.solution != null)
            {              
                var bestPath = current.solution.bestPath;
                var numHex = bestPath.Length;

                totalHintsUsed += 1;

                hintsUsed += 1;

                if(hintsUsed <= 3)
                {
                    ReduceHint();
                }
                                                
                if (hintsUsed == 1)
                {
                    var numHexShown = (int)Math.Min(numHex, Math.Ceiling(numHex / 3.0));

                    DrawPathLine(bestPath.Take(numHexShown));

                    GameManager.instance.characterController.TriggerHints(1);
                }
                else if(hintsUsed == 2)
                {
                    var numHexShown = (int)Math.Min(numHex, Math.Ceiling(2 * numHex / 3.0));

                    DrawPathLine(bestPath.Take(numHexShown));

                    GameManager.instance.characterController.TriggerHints(2);
                }
                else
                {
                    DrawPathLine(bestPath);

                    GameManager.instance.characterController.TriggerHints(3);
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
