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

    public LineRenderer linePrefab;

    public GameObject startPrefab;
    public GameObject endPrefab;

    GameObject startIcon;
    GameObject endIcon;

    Path path;
    LineRenderer line;

    public Text sumText;

    bool isMouseDown = false;

    void Start()
    {
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

    public void OnGameSlotPressed(UIGameSlot gameSlot)
    {
        var slot = gameSlot.uiSlot.slot;

        if(path != null)
        {
            var lastPoint = path.GetLastPoint();
            if (lastPoint != null && lastPoint == slot)
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

        path = new Path(slot);

        UpdateSumText();

        if(line != null)
        {
            Destroy(line.gameObject);
        }

        line = Instantiate(linePrefab, transform);
        line.positionCount += 1;
        line.SetPosition(line.positionCount - 1, gameSlot.transform.position);

        if(startIcon != null)
        {
            Destroy(startIcon);
        }

        if(endIcon != null)
        {
            Destroy(endIcon);
        }

        startIcon = Instantiate(startPrefab, transform);
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

            if (slot != null && path.GetPreviousPoint() == slot) //retracting
            {
                path.RemovePoint(path.GetLastPoint());
                line.positionCount -= 1;

                UpdateSumText();
            }
            else
            {
                if (path.AddPoint(slot))
                {
                    line.positionCount += 1;
                    line.SetPosition(line.positionCount - 1, gameSlot.transform.position);

                    UpdateSumText();

                    if (slot.number == (int)SpecialSlot.Reverse)
                    {
                        Debug.Log("Reverse");
                    }
                }
            }
        }
    }

    public void UpdateSumText()
    {
        if(path != null)
        {
            sumText.text = path.sum.ToString();
        }
    }
}
