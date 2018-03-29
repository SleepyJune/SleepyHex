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

    Path path;
    LineRenderer line;

    public Text sumText;

    public void OnGameSlotPressed(UIGameSlot gameSlot)
    {
        if (selectedSlot)
        {
            selectedSlot.uiSlot.anim.SetBool("selected", false);
        }

        selectedSlot = gameSlot;
        selectedSlot.uiSlot.anim.SetBool("selected", true);

        var slot = gameSlot.uiSlot.slot;
        path = new Path(slot);

        UpdateSumText();

        if(line != null)
        {
            Destroy(line.gameObject);
        }

        line = Instantiate(linePrefab, transform);
        line.positionCount += 1;
        line.SetPosition(line.positionCount - 1, gameSlot.transform.position);
    }

    public void OnGameSlotEnter(UIGameSlot gameSlot)
    {
        if (selectedSlot)
        {
            selectedSlot.uiSlot.anim.SetBool("selected", false);
        }

        selectedSlot = gameSlot;
        selectedSlot.uiSlot.anim.SetBool("selected", true);

        if (path != null)
        {
            var slot = gameSlot.uiSlot.slot;

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

    public void UpdateSumText()
    {
        if(path != null)
        {
            sumText.text = path.sum.ToString();
        }
    }
}
