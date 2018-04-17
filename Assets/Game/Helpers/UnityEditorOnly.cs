using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

public class UnityEditorOnly : MonoBehaviour
{
    public GameObject target;

    void Start()
    {
        if(Application.platform == RuntimePlatform.WindowsEditor)
        {
            target.SetActive(true);
        }
    }
}
