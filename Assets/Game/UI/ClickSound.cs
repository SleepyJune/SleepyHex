using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using UnityEngine.UI;

public class ClickSound : MonoBehaviour
{
    public enum ClickSounds
    {
        clickSound1, clickSound2, clickSound3
    };

    public ClickSounds clickSounds;

    SoundManager manager;
    AudioSource source;

    void Start()
    {
        manager = SoundManager.instance;
        source = manager.soundSource;

        GetComponent<Button>().onClick.AddListener(PlayAudioClip);
    }

    public void PlayAudioClip()
    {
        source.PlayOneShot(manager.clickSounds[(int)clickSounds]);
    }
}
