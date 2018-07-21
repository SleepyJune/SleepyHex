using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

public enum SpeechBubbleIndex
{
    Default = 0,
    NoMoreHints = 10,
    NoInsaneHints = 11,
}

public class CharacterAnimationController : MonoBehaviour
{
    public Animator characterAC;

    public Animator VFX_AC;

    public Animator speechBubbleAC;

    float lastUpdateTime = 0;

    [NonSerialized]
    public bool isAFK = false;

    int pokeCount = 0;

    void Start()
    {
        lastUpdateTime = Time.time;
    }

    void Update()
    {
        if(Time.time - lastUpdateTime >= 1)
        {
            CheckAFK();

            lastUpdateTime = Time.time;
        }    
    }

    public void CheckAFK()
    {
        if (GameManager.instance.pathManager.GetLastMoveTime() >= 5)
        {
            characterAC.SetBool("isPlayerAFK", true);
            isAFK = true;
        }
        else
        {
            characterAC.SetBool("isPlayerAFK", false);
            isAFK = false;
        }
    }

    public void SetGameStartTrigger()
    {
        CheckAFK();
        characterAC.SetTrigger("gameStart");

        characterAC.ResetTrigger("gameOver");
        characterAC.ResetTrigger("Fill");
        characterAC.ResetTrigger("Unfill");
    }

    public void SetGameOverTrigger()
    {
        characterAC.SetTrigger("gameOver");
    }

    public void TriggerClearAll()
    {
        characterAC.SetTrigger("ClearAll");
    }

    public void TriggerHints(int numHints)
    {
        characterAC.SetInteger("HintCount", numHints);
        characterAC.SetTrigger("Hints");
    }

    public void SetTailColor(int colorIndex)
    {
        characterAC.SetInteger("TailColor", colorIndex);
    }

    public void TriggerFill(bool fill)
    {
        if (fill)
        {
            characterAC.SetTrigger("Fill");
        }
        else
        {
            characterAC.SetTrigger("Unfill");
        }
    }

    public void TriggerPlayerAfk()
    {
        characterAC.SetTrigger("PlayerAFK");
    }

    public void TriggerPlayerPoke()
    {
        characterAC.SetTrigger("PlayerPoke");

        pokeCount += 1;

        if(pokeCount > 10)
        {
            pokeCount = 0;
        }

        characterAC.SetInteger("pokeCount", pokeCount);
    }

    public void SetAPS(int actionsPerSecond)
    {
        VFX_AC.SetInteger("ActionsPerSecond", actionsPerSecond);
        characterAC.SetInteger("ActionsPerSecond", actionsPerSecond);
    }

    public void TriggerSpeechBubble(SpeechBubbleIndex textIndex)
    {
        speechBubbleAC.SetTrigger("StartBubble");
        speechBubbleAC.SetInteger("TextIndex", (int)textIndex);
    }
}