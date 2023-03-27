using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ActionHandler : MonoBehaviour, IHandler
{
    public static ActionHandler Instance;

    public void Initialize()
    {
        Instance = this;
    }
    public void ActionOnDelayUnscaled(float time, Action OnDelayFunc)
    {
        StartCoroutine(ActionOnDelayUnscaledRoutine(time, OnDelayFunc));
    }
    public IEnumerator ActionOnDelayUnscaledRoutine(float time, Action OnDelayFunc)
    {
        yield return new WaitForSecondsRealtime(time);
        if (OnDelayFunc != null)
            OnDelayFunc();
    }
}
