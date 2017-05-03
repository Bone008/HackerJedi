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


    public static float EaseInOut01(float t)
    {
        return Mathf.SmoothStep(0, 1, t);
    }


    public static Coroutine Animate<T>(this MonoBehaviour component, float duration, T from, T to, Func<T, T, float, T> interpolationFunc, Action<T> valueCallback, bool useRealtime = false)
    {
        return Animate(component, duration, progress => valueCallback(interpolationFunc(from, to, progress)), useRealtime);
    }
    public static Coroutine Animate<T>(this MonoBehaviour component, float duration, T from, T to, Func<T, T, float, T> interpolationFunc, AnimationCurve curve, Action<T> valueCallback, bool useRealtime = false)
    {
        return Animate(component, duration, curve.Evaluate, progress => valueCallback(interpolationFunc(from, to, progress)), useRealtime);
    }
    public static Coroutine Animate<T>(this MonoBehaviour component, float duration, T from, T to, Func<T, T, float, T> interpolationFunc, Func<float, float> easingFunc, Action<T> valueCallback, bool useRealtime = false)
    {
        return Animate(component, duration, easingFunc, progress => valueCallback(interpolationFunc(from, to, progress)), useRealtime);
    }

    public static Coroutine AnimateScalar(this MonoBehaviour component, float duration, float from, float to, Action<float> valueCallback, bool useRealtime = false)
    {
        return Animate(component, duration, progress => valueCallback(Mathf.Lerp(from, to, progress)), useRealtime);
    }
    public static Coroutine AnimateScalar(this MonoBehaviour component, float duration, float from, float to, AnimationCurve curve, Action<float> valueCallback, bool useRealtime = false)
    {
        return Animate(component, duration, curve.Evaluate, progress => valueCallback(Mathf.Lerp(from, to, progress)), useRealtime);
    }
    public static Coroutine AnimateScalar(this MonoBehaviour component, float duration, float from, float to, Func<float, float> easingFunc, Action<float> valueCallback, bool useRealtime = false)
    {
        return Animate(component, duration, easingFunc, progress => valueCallback(Mathf.Lerp(from, to, progress)), useRealtime);
    }

    public static Coroutine AnimateVector(this MonoBehaviour component, float duration, Vector3 from, Vector3 to, Action<Vector3> valueCallback, bool useRealtime = false)
    {
        return Animate(component, duration, progress => valueCallback(Vector3.Lerp(from, to, progress)), useRealtime);
    }
    public static Coroutine AnimateVector(this MonoBehaviour component, float duration, Vector3 from, Vector3 to, AnimationCurve curve, Action<Vector3> valueCallback, bool useRealtime = false)
    {
        return Animate(component, duration, curve.Evaluate, progress => valueCallback(Vector3.Lerp(from, to, progress)), useRealtime);
    }
    public static Coroutine AnimateVector(this MonoBehaviour component, float duration, Vector3 from, Vector3 to, Func<float, float> easingFunc, Action<Vector3> valueCallback, bool useRealtime = false)
    {
        return Animate(component, duration, easingFunc, progress => valueCallback(Vector3.Lerp(from, to, progress)), useRealtime);
    }

    public static Coroutine AnimateQuaternion(this MonoBehaviour component, float duration, Quaternion from, Quaternion to, Action<Quaternion> valueCallback, bool useRealtime = false)
    {
        return Animate(component, duration, progress => valueCallback(Quaternion.Slerp(from, to, progress)), useRealtime);
    }
    public static Coroutine AnimateQuaternion(this MonoBehaviour component, float duration, Quaternion from, Quaternion to, AnimationCurve curve, Action<Quaternion> valueCallback, bool useRealtime = false)
    {
        return Animate(component, duration, curve.Evaluate, progress => valueCallback(Quaternion.Slerp(from, to, progress)), useRealtime);
    }
    public static Coroutine AnimateQuaternion(this MonoBehaviour component, float duration, Quaternion from, Quaternion to, Func<float, float> easingFunc, Action<Quaternion> valueCallback, bool useRealtime = false)
    {
        return Animate(component, duration, easingFunc, progress => valueCallback(Quaternion.Slerp(from, to, progress)), useRealtime);
    }

    public static Coroutine Animate(this MonoBehaviour component, float duration, Action<float> progressCallback, bool useRealtime = false)
    {
        return Animate(component, duration, t => t, progressCallback, useRealtime);
    }
    public static Coroutine Animate(this MonoBehaviour component, float duration, AnimationCurve curve, Action<float> progressCallback, bool useRealtime = false)
    {
        return component.StartCoroutine(_AnimateCoroutine(duration, curve.Evaluate, progressCallback, useRealtime));
    }
    public static Coroutine Animate(this MonoBehaviour component, float duration, Func<float, float> easingFunc, Action<float> progressCallback, bool useRealtime = false)
    {
        return component.StartCoroutine(_AnimateCoroutine(duration, easingFunc, progressCallback, useRealtime));
    }

    private static IEnumerator _AnimateCoroutine(float duration, Func<float, float> easingFunc, Action<float> progressCallback, bool realtime)
    {
        float t = 0;
        while(t < duration)
        {
            progressCallback(easingFunc(t / duration));
            yield return null;
            t += (realtime ? Time.unscaledDeltaTime : Time.deltaTime);
        }
        progressCallback(1.0f);
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

    /// <summary>
    /// Searches for a GameObject with a specific tag in the current gameobject and its parents
    /// </summary>
    /// <param name="current">the current gameobject</param>
    /// <param name="searchedTag">the name of the tag that is searched for</param>
    /// <returns>the GameObject with the specified tag or null if the search was unsuccessful</returns>
    public static GameObject GetGoInParentWithTag(this GameObject current, string searchedTag)
    {
        Transform currentParent = current.transform;

        while(currentParent != null)
        {
            if (currentParent.tag.Equals(searchedTag))
                return currentParent.gameObject;

            currentParent = currentParent.parent;
        }

        return null;
    }
}
