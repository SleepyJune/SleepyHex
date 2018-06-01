using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

public class AnimatorStateManager : MonoBehaviour
{
    Animator anim;

    int intState;

    string intKey;

    void Awake()
    {
        anim = GetComponent<Animator>();
    }

    void OnEnable()
    {
        anim.SetInteger(intKey, intState);
    }

    public void SetInteger(string key, int state)
    {
        intKey = key;
        intState = state;

        anim.SetInteger(intKey, intState);
    }
}
