using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

public class DialogueGroup : MonoBehaviour
{
    public DialogWindow[] dialogues;

    Dictionary<string, DialogWindow> windows;

    Stack<string> dialogueStack = new Stack<string>();

    void Start()
    {
        windows = new Dictionary<string, DialogWindow>();

        foreach(var dialogue in dialogues)
        {
            windows.Add(dialogue.name, dialogue);
        }
    }

    public string GetCurrentWindow()
    {
        if (dialogueStack.Count > 0)
        {
            return dialogueStack.Peek();
        }

        return null;
    }

    public void ShowWindow(string name)
    {
        DialogWindow target;

        if (windows.TryGetValue(name, out target))
        {
            Debug.Log("Open: " + name);

            //previousActiveWindow = currentActiveWindow;
            //currentActiveWindow = name;

            dialogueStack.Push(name);

            target.Show();
        }
    }

    public void CloseWindow()
    {
        DialogWindow target;

        name = dialogueStack.Peek();

        if (windows.TryGetValue(name, out target))
        {
            //currentActiveWindow = returnWindow;
            dialogueStack.Pop();

            Debug.Log("Top: " + GetCurrentWindow());

            target.Close();
        }
    }

    public void SetActive(string name)
    {
        DialogWindow target;

        if (windows.TryGetValue(name, out target))
        {
            //previousActiveWindow = currentActiveWindow;
            //currentActiveWindow = name;

            dialogueStack = new Stack<string>();
            dialogueStack.Push(name);

            foreach (var dialogue in dialogues)
            {
                if (dialogue.isPopup)
                {
                    continue;
                }

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