using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

public class HomeSceneManager : MonoBehaviour
{
    public LevelDatabase levelDatabase;

    void Start()
    {
        LevelSelector2.LoadLevelByDatabase(levelDatabase);
    }
}
