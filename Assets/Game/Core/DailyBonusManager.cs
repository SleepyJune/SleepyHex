using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using UnityEngine.UI;

public class DailyBonusManager : MonoBehaviour
{
    [NonSerialized]
    HintManager hintManager;

    public DialogWindow window;

    public GameObject[] activeStars;

    public Text hintCount;

    void Awake()
    {
        hintManager = GameManager.instance.hintManager;
    }

    void OnEnable()
    {
        AddDailyBonus();

        DisplayHintCount();

        var dailyBonus = GetDailyBonusStars();
        DisplayStars(dailyBonus);
    }

    public bool CheckDailyBonus()
    {
        string dateNow = DateTime.UtcNow.ToString();
        string lastPlayedDate = PlayerPrefs.GetString("LastPlayedDate", dateNow);

        DateTime oldDate = DateTime.Parse(lastPlayedDate);
        DateTime newDate = DateTime.UtcNow;

        TimeSpan difference = newDate.Subtract(oldDate);

        if(difference.Seconds >= 60 || !PlayerPrefs.HasKey("LastPlayedDate")) //60 seconds since last played
        {
            window.Show();
            PlayerPrefs.SetString("LastPlayedDate", dateNow);
            return true;
        }

        return false;
    }

    public int GetDailyBonusStars()
    {
        return PlayerPrefs.GetInt("DailyBonus", 0);
    }

    public int AddDailyBonus()
    {
        var dailyBonus = PlayerPrefs.GetInt("DailyBonus", 0) + 1;

        if(dailyBonus > 7)
        {
            dailyBonus = 1;
        }

        PlayerPrefs.SetInt("DailyBonus", dailyBonus);

        hintManager.AddHint();

        return dailyBonus;
    }

    public void DisplayHintCount()
    {
        hintCount.text = hintManager.GetPrefabHints().ToString();
    }

    public void DisplayStars(int starCount)
    {
        for (int i = 0; i < activeStars.Length; i++)
        {
            var star = activeStars[i];

            if (i < starCount)
            {
                star.SetActive(true);
            }
            else
            {
                star.SetActive(false);
            }
        }
    }
}
