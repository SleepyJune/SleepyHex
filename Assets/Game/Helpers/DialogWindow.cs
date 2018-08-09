using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

public class DialogWindow : MonoBehaviour
{
    public GameObject panel;

    public DialogueGroup dialogueGroup;

    public bool isPopup = false;

    public void Show()
    {
        if (!panel.activeSelf)//if (!panel.activeInHierarchy)
        {
            panel.SetActive(true);
        }
    }

    public void Close()
    {
        if (panel.activeSelf)
        {
            panel.SetActive(false);
        }
    }

    public void CloseDialogue()
    {
        if (dialogueGroup != null)
        {
            dialogueGroup.CloseWindow();
        }
    }
}
