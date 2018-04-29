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

    public void MuteMusic(bool mute)
    {
        SoundManager.instance.musicSource.mute = mute;
    }

    public void MuteSound(bool mute)
    {
        AudioListener.volume = mute ? 0 : 1;
    }
}