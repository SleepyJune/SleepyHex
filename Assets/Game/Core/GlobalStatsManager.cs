using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

public class GlobalStatsManager : MonoBehaviour
{
    void Start()
    {

    }

    public int IncrementGlobalPlayCount()
    {
        string key = "GlobalPlayCount";

        var playCount = PlayerPrefs.GetInt(key, 0) + 1;

        PlayerPrefs.SetInt(key, playCount);

        return playCount;
    }

    public int GetGlobalPlayCount()
    {
        string key = "GlobalPlayCount";
        
        return PlayerPrefs.GetInt(key, 0);
    }

    public void SetUserRatedGame()
    {
        string key = "UserRatedGame";

        PlayerPrefs.SetInt(key, 1);
    }

    public bool GetUserRatedGame()
    {
        string key = "UserRatedGame";

        return PlayerPrefs.GetInt(key, 0) > 0;
    }
}
