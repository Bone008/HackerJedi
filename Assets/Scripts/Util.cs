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

    #region EnumIteration
    /// <summary>
    /// Given an enum value, returns the next value in numerical order
    /// </summary>
    /// <param name="currentValue">the current enum value</param>
    /// <returns>the next value in numerical order</returns>
    public static T Next<T>(this T currentValue) where T : struct, IComparable
    {
        return EnumIterate(currentValue, 1);
    }

    /// <summary>
    /// Given an enum value, returns the previous value in numerical order
    /// </summary>
    /// <param name="currentValue">the current enum value</param>
    /// <returns>the previous value in numerical order</returns>
    public static T Previous<T>(this T currentValue) where T : struct, IComparable
    {
        return EnumIterate(currentValue, -1);
    }

    private static T EnumIterate<T>(this T currentValue, int amount) where T : struct, IComparable
    {
        T[] values = (T[])Enum.GetValues(currentValue.GetType());
        int currentIndex = Array.IndexOf<T>(values, currentValue);

        return values[(currentIndex + values.Length + amount) % values.Length];
    }
    #endregion

    #region UsableInputAxes
    private static Dictionary<string, bool?> storedInputValues = new Dictionary<string, bool?>();

    /// <summary>
    /// Returns information about how a specific axis was changed in the current frame, similar to Input.GetKeyDown()
    /// </summary>
    /// <param name="axisName">axis defined in the input manager</param>
    /// <returns>true(axis 0>1 in current frame) | false(axis 0>-1 in current frame) | null(axis 0>0 or 1>1 in current frame)</returns>
    public static bool? InputGetAxisDown(string axisName)
    {
        float currentValue = Input.GetAxis(axisName);
        bool? result;

        if(!storedInputValues.ContainsKey(axisName))
        {
            if (currentValue == 0) // 0
                result = null;
            else // 1
                result = (currentValue > 0);

            // store current value for next frame
            storedInputValues.Add(axisName, result);            
        }
        else
        {
            bool? lastValue = storedInputValues[axisName];

            if (currentValue == 0) // 0 -> 0
            {
                result = null;
                storedInputValues[axisName] = result;
            }
            else if (lastValue == null) // 0 -> 1
            {                
                result = (currentValue > 0);
                storedInputValues[axisName] = result;
            }
            else // 1 -> 1
            {
                result = null;
                storedInputValues[axisName] = lastValue;
            }            
        }

        return result;
    }
    #endregion
}
