using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

public class CharacterAnimationController : MonoBehaviour
{
    public Animator characterAC;

    float lastUpdateTime = 0;

    public bool isAFK = false;

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

    public void TriggerHints(int numHints)
    {
        characterAC.SetInteger("HintCount", numHints);
        characterAC.SetTrigger("Hints");
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
    }
}