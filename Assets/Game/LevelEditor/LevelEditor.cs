using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

public class LevelEditor : MonoBehaviour
{
    public Transform templateSlotParent;
    public Transform editorSlotParent;

    public UISlot slotPrefab;

    [NonSerialized]
    public UITemplateSlot selectedTemplate;

    [NonSerialized]
    public UIEditorSlot selectedEditorSlot;

    void Start()
    {
        GenerateTemplateSlots();
        GenerateEditorSlots();
    }

    void GenerateEditorSlots()
    {
        int gridColumns = 6;
        int gridRows = 6;

        var gridManager = new GridManager(gridColumns, gridRows);
        gridManager.CalculateInitialPos();

        for (int row = 0;row < gridRows; row++)
        {
            for(int column = 0;column < gridColumns; column++)
            {
                var hex = new Hex(column, row);
                var vec = hex.ConvertCube();

                var newSlot = Instantiate(slotPrefab, editorSlotParent);
                var worldPos = gridManager.CalculateWorldPos(hex);
                newSlot.transform.localPosition = worldPos;

                var editorSlot = newSlot.gameObject.AddComponent<UIEditorSlot>();
                editorSlot.uiSlot = newSlot;
                editorSlot.uiSlot.slot = new Slot(0, vec);
                editorSlot.levelEditor = this;
            }
        }
    }

    void GenerateTemplateSlots()
    {
        for (int i = 0; i < 11; i++)
        {
            var newSlot = Instantiate(slotPrefab, templateSlotParent);
            var template = newSlot.gameObject.AddComponent<UITemplateSlot>();

            template.uiSlot = newSlot;
            template.uiSlot.slot = new Slot(i);
            template.levelEditor = this;
        }
    }
}
