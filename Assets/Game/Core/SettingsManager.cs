using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    public Transform parent;

    public Toggle muteMusicToggle;

    void Awake()
    {
        var window = GetComponent<DialogWindow>();
        if (window)
        {
            window.Close();
        }

        var muteMusicSetting = GetMuteMusicSetting();

        MuteMusic(muteMusicSetting);

        if (muteMusicToggle)
        {
            muteMusicToggle.isOn = muteMusicSetting;
        }
    }

    public void Back()
    {
        Destroy(parent.gameObject);
    }

    public bool GetMuteMusicSetting()
    {
        return PlayerPrefs.GetInt("MuteMusicSetting", 0) == 1;
    }

    public void MuteMusic(bool mute)
    {
        //SoundManager.instance.musicSource.mute = mute;

        AudioListener.volume = mute ? 0 : 1;

        PlayerPrefs.SetInt("MuteMusicSetting", Convert.ToInt32(mute));
    }

    public void MuteSound(bool mute)
    {
        AudioListener.volume = mute ? 0 : 1;
    }
}