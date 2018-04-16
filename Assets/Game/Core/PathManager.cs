using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using UnityEngine.UI;

public class PathManager : MonoBehaviour
{
    [NonSerialized]
    public UIGameSlot selectedSlot;

    public Transform slotListTop;

    public LineRenderer linePrefab;

    public GameObject startPrefab;
    public GameObject endPrefab;

    GameObject startIcon;
    GameObject endIcon;

    Path path;
    LineRenderer line;

    public Text sumText;

    bool isMouseDown = false;

    LevelManager levelManager;

    void Start()
    {
        levelManager = GameManager.instance.levelManager;

        TouchInputManager.instance.touchStart += OnTouchStart;
        TouchInputManager.instance.touchEnd += OnTouchEnd;
    }

    private void OnTouchStart(Touch touch)
    {
        isMouseDown = true;
    }

    private void OnTouchEnd(Touch touch)
    {
        isMouseDown = false;
    }

    public void ClearPath()
    {
        if (line != null)
        {
            Destroy(line.gameObject);
        }

        if (startIcon != null)
        {
            Destroy(startIcon);
        }

        if (endIcon != null)
        {
            Destroy(endIcon);
        }

        path = null;

        ResetAllBlanks();
        UpdateSumText();
    }

    public void OnGameSlotPressed(UIGameSlot gameSlot)
    {
        var slot = gameSlot.uiSlot.slot;

        if(path != null)
        {
            var lastPoint = path.GetLastPoint();
            if (lastPoint.slot == slot)
            {
                return;
            }
        }

        if (selectedSlot)
        {
            selectedSlot.uiSlot.anim.SetBool("selected", false);
        }

        selectedSlot = gameSlot;
        selectedSlot.uiSlot.anim.SetBool("selected", true);

        ClearPath();

        if (!slot.isNumber)
        {
            return;
        }

        path = new Path(slot);

        UpdateSumText();

        line = Instantiate(linePrefab, slotListTop);
        line.positionCount += 1;
        line.SetPosition(line.positionCount - 1, gameSlot.transform.position);

        startIcon = Instantiate(startPrefab, slotListTop);
        startIcon.transform.position = gameSlot.transform.position;
    }

    public void OnGameSlotEnter(UIGameSlot gameSlot)
    {
        if (!isMouseDown)
        {
            return;
        }

        if (selectedSlot)
        {
            selectedSlot.uiSlot.anim.SetBool("selected", false);
        }

        selectedSlot = gameSlot;
        selectedSlot.uiSlot.anim.SetBool("selected", true);

        if (path != null)
        {
            var slot = gameSlot.uiSlot.slot;
            var previous = path.GetPreviousPoint();

            if (slot != null && previous != null && previous.slot == slot) //retracting
            {
                var lastPoint = path.GetLastPoint();
                RemovePoint(lastPoint, lastPoint.previous);

                path.RemovePoint(lastPoint);
                line.positionCount -= 1;

                UpdateSumText();
            }
            else
            {
                if (path.AddPoint(slot))
                {
                    var lastPoint = path.GetLastPoint();
                    AddPoint(lastPoint.previous, lastPoint);

                    line.positionCount += 1;
                    line.SetPosition(line.positionCount - 1, gameSlot.transform.position);

                    UpdateSumText();

                    //Debug.Log(path.GetLastPoint().number);

                    if (slot.number == (int)SpecialSlot.Reverse)
                    {
                        Debug.Log("Reverse");
                    }
                }
            }
        }
    }

    public Path GetPath()
    {
        return path;
    }

    public void UpdateSumText()
    {
        if(path != null)
        {
            sumText.text = path.GetTotalPoints().ToString();
        }
        else
        {
            sumText.text = "0";
        }

        UpdateFill();
    }

    public void UpdateFill()
    {
        var gridManager = levelManager.GetGridManager();

        if (gridManager != null)
        {            
            foreach (var slot in gridManager.GetUISlots())
            {
                var filled = false;

                if (path != null)
                {
                    filled = path.waypointsHash.Contains(slot.slot);
                }

                slot.SetFilled(filled);
            }
        }
    }

    void ResetAllBlanks()
    {
        var gridManager = levelManager.GetGridManager();

        if (gridManager != null)
        {
            foreach (var slot in gridManager.GetUISlots())
            {
                if (slot.slot.number == (int)SpecialSlot.Blank)
                {
                    slot.SetBlankNumber((int)SpecialSlot.Blank);
                }
            }
        }
    }

    void AddPoint(PathSlot start, PathSlot end)
    {
        var gridManager = levelManager.GetGridManager();
        var slot = gridManager.GetUISlot(end.slot.position);

        if (slot != null)
        {
            if (end.slot.number == (int)SpecialSlot.Blank)
            {
                slot.SetBlankNumber(end.number);
            }

            var startSlot = gridManager.GetUISlot(start.slot.position);
            
            Vector3 dir = slot.transform.position - startSlot.transform.position;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + 90;
            slot.background.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

            //Quaternion rotation = Quaternion.LookRotation(relativePos);
            //slot.background.transform.rotation = rotation;
        }
    }

    void RemovePoint(PathSlot start, PathSlot end)
    {
        var gridManager = levelManager.GetGridManager();
        var slot = gridManager.GetUISlot(start.slot.position);

        if (slot != null)
        {
            if (start.slot.number == (int)SpecialSlot.Blank)
            {
                slot.SetBlankNumber((int)SpecialSlot.Blank);
            }
        }
    }
}
