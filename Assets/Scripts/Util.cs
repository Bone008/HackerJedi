using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


public static class Util
{
    public enum CoroutineOptions
    {
        // idea to manage coroutines more easily, not implemented yet

        Default = 0,
        NonOverlappingStopOld,
        NonOverlappingFailNew,
        NonOverlappingEnqueue
    }



    /// <summary>
    /// A thin wrapper around StartCoroutine for simple scenarios where you just want to execute something after some time has passed.
    /// </summary>
    /// <param name="component">the script that wants to delay something</param>
    /// <param name="delay">delay in ingame seconds</param>
    /// <param name="callback">the callback that should be invoked; useful with lambda expressions: () => ...</param>
    /// <returns>the coroutine as created by Unity</returns>
    public static Coroutine Delayed(this MonoBehaviour component, float delay, Action callback)
    {
        return component.StartCoroutine(_DelayedCoroutine(new WaitForSeconds(delay), callback));
    }


    /// <summary>
    /// A thin wrapper around StartCoroutine for simple scenarios where you just want to execute something after some time has passed.
    /// </summary>
    /// <param name="component">the script that wants to delay something</param>
    /// <param name="delay">something you could "yield return" from a coroutine</param>
    /// <param name="callback">the callback that should be invoked; useful with lambda expressions: () => ...</param>
    /// <returns>the coroutine as created by Unity</returns>
    public static Coroutine Delayed(this MonoBehaviour component, YieldInstruction delayObject, Action callback)
    {
        return component.StartCoroutine(_DelayedCoroutine(delayObject, callback));
    }


    private static IEnumerator _DelayedCoroutine(YieldInstruction delayObject, Action callback)
    {
        yield return delayObject;
        callback();
    }

}
