using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

class AnimatorHelpers : MonoBehaviour
{
    public Animator anim;

    public string targetName;

    void Start()
    {
        if(anim == null)
        {
            anim.GetComponent<Animator>();
        }
    }

    public void SetTrigger(string name)
    {
        anim.SetTrigger(name);
    }

    public void SetBool(bool isOn)
    {
        if(targetName != "")
        {
            anim.SetBool(targetName, isOn);
        }
    }

    public void SetFloat(float number)
    {
        if (targetName != "")
        {
            anim.SetFloat(targetName, number);
        }
    }
}
