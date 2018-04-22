using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    public Transform parent;

    public void Back()
    {
        Destroy(parent.gameObject);
    }
}