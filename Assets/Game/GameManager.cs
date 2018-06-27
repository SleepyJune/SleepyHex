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

    [NonSerialized]
    public LevelManager levelManager;

    [NonSerialized]
    public GlobalStatsManager globalStatsManager;

    [NonSerialized]
    public TutorialManager tutorialManager;

    [NonSerialized]
    public HintManager hintManager;

    [NonSerialized]
    public CharacterAnimationController characterController;
    
    public DevModeManager devModeManager;

    public ScoreManager scoreManager;

    public LevelSelector2 levelSelector;

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
        levelManager = GetComponent<LevelManager>();
        globalStatsManager = GetComponent<GlobalStatsManager>();
        tutorialManager = GetComponent<TutorialManager>();
        hintManager = GetComponent<HintManager>();
        characterController = GetComponent<CharacterAnimationController>();
    }

    public void LoadLevel(string levelName)
    {
        pathManager.ClearPath();
        levelManager.LoadLevel(levelName);
    }
}
