using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using UnityEngine.SceneManagement;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    public AudioSource source;

    public AudioClip homeMusic;
    public AudioClip[] gameMusic;

    public int changeMusicDelay = 5;

    [NonSerialized]
    public int currentGameMusicIndex = 0;
    [NonSerialized]
    public bool isPlayingHomeMusic = true;
    [NonSerialized]
    public bool isChangingMusic = false;

    void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else if(instance != this)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(this);
        SceneManager.sceneLoaded += OnSceneLoaded;

        InvokeRepeating("ChangeMusic", 0, 1);

        if(gameMusic != null)
        {
            currentGameMusicIndex = UnityEngine.Random.Range(0, gameMusic.Length-1);
        }
    }

    void ChangeMusic()
    {
        if (!source.isPlaying && !isChangingMusic)
        {
            isChangingMusic = true;
            Invoke("ChangeMusicHelper", changeMusicDelay);
        }
    }

    void ChangeMusicHelper()
    {
        if (!isPlayingHomeMusic)
        {
            if (gameMusic != null)
            {
                currentGameMusicIndex += 1;
                if (currentGameMusicIndex + 1 > gameMusic.Length)
                {
                    currentGameMusicIndex = 0;
                }

                source.clip = gameMusic[currentGameMusicIndex];
                source.Play();
            }
        }
        else
        {
            PlayHomeMusic();
        }

        isChangingMusic = false;
    }

    public void PlayHomeMusic()
    {
        if (!isPlayingHomeMusic)
        {
            source.clip = homeMusic;
            source.Play();

            isPlayingHomeMusic = true;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if(scene.name == "Home")
        {
            PlayHomeMusic();
        }
    }

    public void OnLevelLoaded()
    {
        if (gameMusic != null && isPlayingHomeMusic)
        {
            source.clip = gameMusic[currentGameMusicIndex];
            source.Play();

            isPlayingHomeMusic = false;
        }
    }
}
