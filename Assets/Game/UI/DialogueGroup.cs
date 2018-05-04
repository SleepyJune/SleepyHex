using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

public class DialogueGroup : MonoBehaviour
{
    public DialogWindow[] dialogues;

    public void SetActive(string name)
    {
        foreach (var dialogue in dialogues)
        {
            if(dialogue.name != name)
            {
                dialogue.Close();
            }
            else
            {
                dialogue.Show();
            }
        }
    }
}