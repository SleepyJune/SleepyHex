using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

public class DialogWindow : MonoBehaviour
{
    public GameObject panel;

    public void Show()
    {
        if (!panel.activeInHierarchy)
        {
            panel.SetActive(true);
        }
    }

    public void Close()
    {
        if (panel.activeInHierarchy)
        {
            panel.SetActive(false);
        }
    }
}
