using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;

    [NonSerialized]
    public PathManager pathManager;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
            return;
        }

        pathManager = GetComponent<PathManager>();
    }     

}
