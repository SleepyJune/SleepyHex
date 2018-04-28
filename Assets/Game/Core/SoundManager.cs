using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using UnityEngine.SceneManagement;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    public AudioSource musicSource;
    public AudioSource soundSource;

    public AudioClip homeMusic;
    public AudioClip[] gameMusic;

    public int musicFadeDelay = 5;
    
    public int musicPlayTime = 300;
    
    float musicStartTime;

    [NonSerialized]
    public int currentGameMusicIndex = 0;
    [NonSerialized]
    public bool isPlayingHomeMusic = false;
    [NonSerialized]
    public bool isChangingMusic = false;

    public AudioClip[] clickSounds;

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

        InvokeRepeating("ChangeMusic", 0, 5);

        if(gameMusic != null)
        {
            currentGameMusicIndex = UnityEngine.Random.Range(0, gameMusic.Length-1);
        }

        PlayHomeMusic();
    }

    void ChangeMusic()
    {
        if (!isChangingMusic && Time.time - musicStartTime >= musicPlayTime)
        {
            isChangingMusic = true;

            if (gameMusic != null)
            {
                currentGameMusicIndex += 1;
                if (currentGameMusicIndex + 1 > gameMusic.Length)
                {
                    currentGameMusicIndex = 0;
                }

                StartCoroutine(SoundFadeOut(musicFadeDelay));
            }
        }
    }

    IEnumerator SoundFadeOut(int delay)
    {
        while (musicSource.volume > 0.01f)
        {
            if (isPlayingHomeMusic)
            {
                break;
            }

            musicSource.volume -= Time.deltaTime / delay;
            yield return null;
        }

        if (!isPlayingHomeMusic)
        {
            //musicSource.volume = 0;
            //musicSource.Stop();

            musicSource.volume = 1;

            musicSource.clip = gameMusic[currentGameMusicIndex];
            musicSource.Play();
        }

        isChangingMusic = false;
    }

    public void PlayHomeMusic()
    {
        if (!isPlayingHomeMusic)
        {
            musicSource.volume = 1;

            musicSource.clip = homeMusic;
            musicSource.Play();

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
            musicSource.clip = gameMusic[currentGameMusicIndex];
            musicSource.Play();

            isPlayingHomeMusic = false;
        }
    }
}
