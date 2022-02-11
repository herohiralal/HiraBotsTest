using System.Collections;
using UnityEngine;

namespace AIEngineTest
{
    public static class InterpolationHelper
    {
        public static IEnumerator Parallel(IEnumerator a, IEnumerator b)
        {
            bool aMoveNext, bMoveNext;
            do
            {
                aMoveNext = a.MoveNext();
                bMoveNext = b.MoveNext();

                yield return null;
            } while (aMoveNext || bMoveNext);
        }

        public static IEnumerator PositionLerp(Transform t, Vector3 start, Vector3 end, float time)
        {
            for (var timeElapsed = 0f; timeElapsed < time; timeElapsed += Time.deltaTime)
            {
                t.position = Vector3.Lerp(start, end, timeElapsed / time);
                yield return null;
            }

            t.position = end;
        }

        public static IEnumerator PositionLerpUnscaledTime(Transform t, Vector3 start, Vector3 end, float time)
        {
            for (var timeElapsed = 0f; timeElapsed < time; timeElapsed += Time.unscaledDeltaTime)
            {
                t.position = Vector3.Lerp(start, end, timeElapsed / time);
                yield return null;
            }

            t.position = end;
        }
    }
}