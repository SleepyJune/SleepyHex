﻿using System;
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

    public bool canFillSlots = true;
    
    void Start()
    {
        levelManager = GameManager.instance.levelManager;

        TouchInputManager.instance.touchStart += OnTouchStart;
        TouchInputManager.instance.touchMove += OnTouchMove;
        TouchInputManager.instance.touchEnd += OnTouchEnd;

        sumText.text = "";
    }

    private void OnTouchStart(Touch touch)
    {
        isMouseDown = true;
    }

    private void OnTouchMove(Touch touch)
    {
        if (isMouseDown && path != null)
        {
            var grid = levelManager.GetGridManager();

            var lastSlot = path.GetLastPoint();

            var uiSlot = grid.GetUISlot(lastSlot.slot.position);
            if (uiSlot != null)
            {               
                if(uiSlot.gameSlot != null && uiSlot.gameSlot.isSelected)
                {
                    return;
                }

                var touchPos = new Vector3(touch.position.x, touch.position.y, 0);
                var slotPos = Camera.main.WorldToScreenPoint(uiSlot.transform.position);
                slotPos.z = 0;

                var dir = (touchPos - slotPos).normalized;

                foreach(var neighbour in uiSlot.slot.neighbours)
                {
                    var neighbourUiSlot = grid.GetUISlot(neighbour.position);
                    if (neighbourUiSlot != null)
                    {
                        var neighbourDir = (neighbourUiSlot.transform.position - uiSlot.transform.position).normalized;

                        var dot = Vector3.Dot(neighbourDir, dir);

                        if (dot >= .99)
                        {
                            var gameSlot = neighbourUiSlot.GetComponent<UIGameSlot>();
                            OnGameSlotEnter(gameSlot);
                            
                            return;
                        }
                    }
                }
            }
        }
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
        canFillSlots = true;

        ResetAllBlanks();
        UpdateSumText();
    }

    public void OnGameSlotPressed(UIGameSlot gameSlot)
    {
        if (!canFillSlots)
        {
            return;
        }

        var slot = gameSlot.uiSlot.slot;

        if(path != null)
        {
            var lastPoint = path.GetLastPoint();
            if (lastPoint.slot == slot)
            {
                return;
            }
        }

        selectedSlot = gameSlot;

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
        if (!isMouseDown || !canFillSlots)
        {
            return;
        }

        selectedSlot = gameSlot;

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
                    
                    if(path.waypoints.Count == levelManager.GetCurrentLevel().map.Values.Count)
                    {
                        canFillSlots = false;
                        Invoke("CheckSolution", .25f);
                    }                   
                }
            }
        }
    }

    void CheckSolution()
    {
        levelManager.Check();
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
            sumText.text = "";
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
                else if(slot.slot.number == (int)SpecialSlot.Reverse)
                {
                    slot.SetIconState(0);
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
            else if(slot.slot.number == (int)SpecialSlot.Reverse)
            {
                if (end.isDescending)
                {
                    slot.SetIconState(1);
                }
                else
                {
                    slot.SetIconState(2);
                }
            }
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
            else if (slot.slot.number == (int)SpecialSlot.Reverse)
            {
                slot.SetIconState(0);
            }
        }
    }
}
