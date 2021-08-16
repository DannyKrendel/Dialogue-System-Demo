using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EnumeratorUtils
{
    public static bool RunCoroutineWithoutYields (this IEnumerator enumerator, int maxYields = 1000)
    {
        var enumStack = new Stack<IEnumerator> ();
        enumStack.Push (enumerator);

        var step = 0;
        while (enumStack.Count > 0) {
            var activeEnum = enumStack.Pop ();
            while (activeEnum.MoveNext ()) {
                if (activeEnum.Current is IEnumerator current) {
                    enumStack.Push (activeEnum);
                    activeEnum = current;
                } else if (activeEnum.Current is Coroutine) {
                    throw new System.NotSupportedException (
                        "RunCoroutineWithoutYields can not be used with an IEnumerator that calls StartCoroutine inside itself.");
                }
                step += 1;
                if (step >= maxYields) {
                    return false;
                }
            }
        }
        return true;
    }
}