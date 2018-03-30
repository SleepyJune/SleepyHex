using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

public delegate void Callback();

public static class DelayAction
{    
    static List<Action> actions = new List<Action>();
    static List<Action> nextFrameActions = new List<Action>();

    static DelayAction()
    {

    }

    public static void OnUpdate()
    {
        //NextFrame Actions
        for (var i = nextFrameActions.Count - 1; i >= 0; i--)
        {
            if (nextFrameActions[i].callback != null)
            {
                nextFrameActions[i].callback();

                if (nextFrameActions.Count == 0)
                {
                    break;
                }
            }
            nextFrameActions.RemoveAt(i);
        }

        //Delay Actions with Time
        for (var i = actions.Count - 1; i >= 0; i--)
        {
            if (actions[i].time <= Time.time)
            {
                if (actions[i].callback != null)
                {
                    actions[i].callback();

                    if (actions.Count == 0)
                    {
                        break;
                    }
                }
                actions.RemoveAt(i);
            }
        }
    }

    public static void Reset()
    {
        actions = new List<Action>();
    }

    public static void Add(Callback func, float time)
    {
        var action = new Action(time, func);
        actions.Add(action);
    }

    public static void AddNextFrame(Callback func)
    {
        var action = new Action(0, func);
        nextFrameActions.Add(action);
    }

    public struct Action
    {
        public Callback callback;
        public float time;

        public Action(float time, Callback callback)
        {
            this.time = time + Time.time;
            this.callback = callback;
        }
    }
}