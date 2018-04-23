using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

public class DialogueLoader : MonoBehaviour
{
    public GameObject dialoguePrefab;
    public Transform parent;

    public void Show()
    {
        if(parent != null)
        {
            Instantiate(dialoguePrefab, parent);
        }
        else
        {
            Instantiate(dialoguePrefab, transform);
        }
    }
}