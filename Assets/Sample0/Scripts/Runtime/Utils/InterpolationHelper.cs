using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;
using static Unity.Mathematics.math;

namespace AIEngineTest
{
    #region Delta Time Providers

    public interface IDeltaTimeProvider
    {
        float currentValue
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
        }
    }

    public struct ScaledTime : IDeltaTimeProvider
    {
        public float currentValue => Time.deltaTime;
    }

    public struct UnscaledTime : IDeltaTimeProvider
    {
        public float currentValue => Time.unscaledDeltaTime;
    }

    #endregion

    #region Interpolators

    public interface IInterpolator
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        float Modify(float x);
    }

    public struct LinearInterpolator : IInterpolator
    {
        public float Modify(float x)
        {
            return x;
        }
    }

    #region Ease In

    public struct EaseInSineInterpolator : IInterpolator
    {
        public float Modify(float x)
        {
            return 1 - cos((x * PI) / 2);
        }
    }

    public struct EaseInQuadInterpolator : IInterpolator
    {
        public float Modify(float x)
        {
            return pow(x, 2);
        }
    }

    public struct EaseInCubicInterpolator : IInterpolator
    {
        public float Modify(float x)
        {
            return pow(x, 3);
        }
    }

    public struct EaseInQuartInterpolator : IInterpolator
    {
        public float Modify(float x)
        {
            return pow(x, 4);
        }
    }

    public struct EaseInQuintInterpolator : IInterpolator
    {
        public float Modify(float x)
        {
            return pow(x, 5);
        }
    }

    #endregion

    #region Ease Out

    public struct EaseOutSineInterpolator : IInterpolator
    {
        public float Modify(float x)
        {
            return sin((x * PI) / 2);
        }
    }

    public struct EaseOutQuadInterpolator : IInterpolator
    {
        public float Modify(float x)
        {
            return 1 - pow(1 - x, 2);
        }
    }

    public struct EaseOutCubicInterpolator : IInterpolator
    {
        public float Modify(float x)
        {
            return 1 - pow(1 - x, 3);
        }
    }

    public struct EaseOutQuartInterpolator : IInterpolator
    {
        public float Modify(float x)
        {
            return 1 - pow(1 - x, 4);
        }
    }

    public struct EaseOutQuintInterpolator : IInterpolator
    {
        public float Modify(float x)
        {
            return 1 - pow(1 - x, 5);
        }
    }

    #endregion

    #region Ease In & Out

    public struct EaseInOutSineInterpolator : IInterpolator
    {
        public float Modify(float x)
        {
            return -(cos(PI * x) - 1) / 2;
        }
    }

    public struct EaseInOutQuadInterpolator : IInterpolator
    {
        public float Modify(float x)
        {
            return x < 0.5f ? 2  * pow(x, 2) : 1 - pow(-2 * x + 2, 2) / 2;
        }
    }

    public struct EaseInOutCubicInterpolator : IInterpolator
    {
        public float Modify(float x)
        {
            return x < 0.5 ? 4 * pow(x, 3) : 1 - pow(-2 * x + 2, 3) / 2;
        }
    }

    public struct EaseInOutQuartInterpolator : IInterpolator
    {
        public float Modify(float x)
        {
            return x < 0.5 ? 8 * pow(x, 4) : 1 - pow(-2 * x + 2, 4) / 2;
        }
    }

    public struct EaseInOutQuintInterpolator : IInterpolator
    {
        public float Modify(float x)
        {
            return x < 0.5 ? 16 * pow(x, 5) : 1 - pow(-2 * x + 2, 5) / 2;
        }
    }

    #endregion

    #endregion

    public static class InterpolationHelper
    {
        public static IEnumerator Parallel(this MonoBehaviour m, IEnumerator a, IEnumerator b)
        {
            var ac = m.StartCoroutine(a);
            var bc = m.StartCoroutine(b);

            yield return ac;
            yield return bc;
        }

        public static IEnumerator TweenAlpha<TInterpolator, TDeltaTimeProvider>(this CanvasGroup canvasGroup, float start, float end, float time)
            where TInterpolator : unmanaged, IInterpolator
            where TDeltaTimeProvider : unmanaged, IDeltaTimeProvider
        {
            var interpolator = new TInterpolator();
            var deltaTimeProvider = new TDeltaTimeProvider();

            for (var timeElapsed = 0f; timeElapsed < time; timeElapsed += deltaTimeProvider.currentValue)
            {
                var t = interpolator.Modify(timeElapsed / time);
                canvasGroup.alpha = Mathf.LerpUnclamped(start, end, t);
                yield return null;
            }

            canvasGroup.alpha = end;
        }

        public static IEnumerator TweenTimescale<TInterpolator>(float start, float end, float unscaledTime)
            where TInterpolator : unmanaged, IInterpolator
        {
            var interpolator = new TInterpolator();
            var deltaTimeProvider = new UnscaledTime();

            for (var timeElapsed = 0f; timeElapsed < unscaledTime; timeElapsed += deltaTimeProvider.currentValue)
            {
                var t = interpolator.Modify(timeElapsed / unscaledTime);
                Time.timeScale = Mathf.LerpUnclamped(start, end, t);
                yield return null;
            }

            Time.timeScale = end;
        }

        public static IEnumerator TweenPosition<TInterpolator, TDeltaTimeProvider>(this Transform transform, Vector3 start, Vector3 end, float time)
            where TInterpolator : unmanaged, IInterpolator
            where TDeltaTimeProvider : unmanaged, IDeltaTimeProvider
        {
            var interpolator = new TInterpolator();
            var deltaTimeProvider = new TDeltaTimeProvider();

            for (var timeElapsed = 0f; timeElapsed < time; timeElapsed += deltaTimeProvider.currentValue)
            {
                var t = interpolator.Modify(timeElapsed / time);
                transform.position = Vector3.LerpUnclamped(start, end, t);
                yield return null;
            }

            transform.position = end;
        }
    }
}