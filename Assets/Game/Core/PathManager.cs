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
    public Slider sumSlider;

    bool isMouseDown = false;
    bool isButtonPressed = false;

    LevelManager levelManager;

    public bool canFillSlots = true;

    float lastMoveTime = 0;

    Queue<float> actionQueue = new Queue<float>();

    float lastUpdateTime = 0;

    void Start()
    {
        levelManager = GameManager.instance.levelManager;

        TouchInputManager.instance.touchStart += OnTouchStart;
        TouchInputManager.instance.touchMove += OnTouchMove;
        TouchInputManager.instance.touchEnd += OnTouchEnd;

        sumText.text = "";
        sumSlider.value = 0;

        lastMoveTime = Time.time;
    }

    void Update()
    {
        if(Time.time - lastUpdateTime >= .25f)
        {
            UpdateActionsPerSecond();

            lastUpdateTime = Time.time;
        }
    }

    public void SetNewLevel()
    {
        lastMoveTime = Time.time;

        GameManager.instance.characterController.SetGameStartTrigger();
    }

    public float GetLastMoveTime()
    {
        return Time.time - lastMoveTime;
    }

    private void OnTouchStart(Touch touch)
    {
        isMouseDown = true;

        lastMoveTime = Time.time;
    }

    private void OnTouchMove(Touch touch)
    {
        lastMoveTime = Time.time;

        if (!canFillSlots)
        {
            return;
        }

        if (isMouseDown && path != null)
        {
            var grid = levelManager.GetGridManager();

            var lastSlot = path.GetLastPoint();

            var uiSlot = grid.GetUISlot(lastSlot.slot.position);
            if (uiSlot != null)
            {
                if (uiSlot.gameSlot != null && uiSlot.gameSlot.isSelected)
                {
                    return;
                }

                var touchPos = new Vector3(touch.position.x, touch.position.y, 0);
                var slotPos = Camera.main.WorldToScreenPoint(uiSlot.transform.position);
                slotPos.z = 0;

                var dir = (touchPos - slotPos).normalized;

                foreach (var neighbour in uiSlot.slot.neighbours)
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

        lastMoveTime = Time.time;
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

        GameManager.instance.characterController.SetTailColor(0);
    }

    public void PlayerPressClear()
    {
        if (canFillSlots)
        {
            GameManager.instance.characterController.TriggerClearAll();
            ClearPath();
        }
    }

    public void OnGameSlotPressed(UIGameSlot gameSlot)
    {
        if (!canFillSlots)
        {
            return;
        }

        var slot = gameSlot.uiSlot.slot;

        if (path != null)
        {
            var lastPoint = path.GetLastPoint();
            if (lastPoint.slot == slot)
            {
                return;
            }

            if (lastPoint.slot.neighbours.Contains(slot))
            {
                isButtonPressed = true;
                OnGameSlotEnter(gameSlot);
                isButtonPressed = false;
                return;
            }

            if (path.waypointsHash.Contains(slot)) //retracting all the way
            {
                var returnSlot = path.waypoints.Find(n => n.slot == slot);

                if (returnSlot.next != null)
                {
                    var deleteSlot = returnSlot.next.slot;

                    while (path.waypoints.Count > 0 && path.waypointsHash.Contains(deleteSlot))
                    {
                        GoBack();
                    }                    
                    return;
                }
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

        FillSlot(path.GetLastPoint());
    }

    public void OnGameSlotEnter(UIGameSlot gameSlot)
    {
        if ((!isMouseDown && !isButtonPressed) || !canFillSlots)
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
                GoBack();
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

                    if (path.waypoints.Count == levelManager.GetCurrentLevel().map.Values.Count)
                    {
                        canFillSlots = false;
                        Invoke("CheckSolution", 2f);
                        SetGameOver();
                    }
                }
            }
        }
    }

    void GoBack()
    {
        if (path != null && path.waypoints.Count > 0)
        {
            var lastPoint = path.GetLastPoint();
            RemovePoint(lastPoint, lastPoint.previous);

            path.GoBack();
            line.positionCount -= 1;

            UpdateSumText();
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
        var level = levelManager.GetCurrentLevel();

        if (level != null)
        {
            float bestScore = level.solution.bestScore;

            if (path != null)
            {
                sumText.text = path.GetTotalPoints().ToString() + " / " + bestScore;
                sumSlider.value = path.GetTotalPoints() / bestScore;
            }
            else
            {
                sumText.text = "0 / " + bestScore;
                sumSlider.value = 0;
            }


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

    void SetGameOver()
    {
        if (line != null)
        {
            var anim = line.gameObject.GetComponent<Animator>();

            if (anim)
            {
                anim.SetTrigger("gameOver");
            }
        }

        if (startIcon != null)
        {
            var anim = startIcon.gameObject.GetComponent<Animator>();

            if (anim)
            {
                anim.SetTrigger("gameOver");
            }
        }

        if (endIcon != null)
        {
            var anim = endIcon.gameObject.GetComponent<Animator>();

            if (anim)
            {
                anim.SetTrigger("gameOver");
            }
        }

        GameManager.instance.characterController.SetGameOverTrigger();

        var gridManager = levelManager.GetGridManager();

        if (gridManager != null)
        {
            foreach (var slot in gridManager.GetUISlots())
            {
                slot.SetGameOverAnimation();
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
                if (slot.slot.isBlank)
                {
                    slot.ResetBlankNumber();
                }
                else if (slot.slot.isAddFill)
                {
                    slot.ResetBlankNumber();
                    slot.SetIconState(0);
                }
                else if (slot.slot.isReverse)
                {
                    slot.SetIconState(0);
                }
            }
        }
    }

    void UpdateActionsPerSecond()
    {
        int actionsPerSecond = 0;

        foreach (var time in actionQueue.ToList())
        {
            if (Time.time - time >= 1)
            {
                actionQueue.Dequeue();
            }

            actionsPerSecond += 1;
        }

        GameManager.instance.characterController.SetAPS(actionsPerSecond);
    }

    void FillSlot(PathSlot slot)
    {
        GameManager.instance.characterController.TriggerFill(true);
        GameManager.instance.characterController.SetTailColor(slot.number);
    }

    void AddPoint(PathSlot start, PathSlot end)
    {
        var gridManager = levelManager.GetGridManager();
        var slot = gridManager.GetUISlot(end.slot.position);

        if (slot != null)
        {
            FillSlot(end);

            actionQueue.Enqueue(Time.time);

            UpdateActionsPerSecond();

            if (end.slot.isBlank)
            {
                slot.SetBlankNumber(end.number);
            }
            else if (end.slot.isAddFill)
            {
                slot.SetBlankNumber(end.number);
                slot.SetIconState(3);
            }
            else if (slot.slot.isReverse)
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
            GameManager.instance.characterController.TriggerFill(false);
            GameManager.instance.characterController.SetTailColor(end.number);

            if (start.slot.isBlank)
            {
                slot.ResetBlankNumber();
            }
            else if (slot.slot.isAddFill)
            {
                slot.ResetBlankNumber();
                slot.SetIconState(0);
            }
            else if (slot.slot.isReverse)
            {
                slot.SetIconState(0);
            }
        }
    }
}
