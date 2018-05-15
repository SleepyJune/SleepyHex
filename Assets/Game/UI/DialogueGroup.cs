using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

public class DialogueGroup : MonoBehaviour
{
    public DialogWindow[] dialogues;

    Dictionary<string, DialogWindow> windows;

    void Start()
    {
        windows = new Dictionary<string, DialogWindow>();

        foreach(var dialogue in dialogues)
        {
            windows.Add(dialogue.name, dialogue);
        }
    }

    public void SetActive(string name)
    {
        DialogWindow target;

        if (windows.TryGetValue(name, out target))
        {
            foreach (var dialogue in dialogues)
            {
                if (dialogue.name != name)
                {
                    if (!target.transform.IsChildOf(dialogue.transform))
                    {
                        dialogue.Close();
                    }
                    else
                    {
                        dialogue.Show();
                    }
                }
                else
                {
                    dialogue.Show();
                }
            }
        }
    }
}