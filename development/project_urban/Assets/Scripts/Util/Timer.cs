using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Debug = UnityEngine.Debug;

namespace Util
{
    public class Timer
    {
        private readonly Dictionary<string, long> _sourceTimes = new();

        /// <summary>
        /// Convenience method for <c>Timer.OncePerSecond(() => Debug.Log(message));</c>.
        /// Call <c>UnityEngine.Debug.Log(message)</c> unless it was executed from the calling location in the last second. In this case do nothing.
        /// Don't call this method twice on the same line.
        /// </summary>
        /// <param name="message">Will be passed to `UnityEngine.Debug.Log` whenever one second has passed since the last call, or if it is the first call.</param>
        /// <param name="sourceFilePath">Ignore. Don't set manually. Correct file path is filled in by the compiler.</param>
        /// <param name="sourceLineNumber">Ignore. Don't set manually. Correct line number is filled in by the compiler.</param>
        [SuppressMessage("ReSharper", "ExplicitCallerInfoArgument")]
        [SuppressMessage("ReSharper", "Unity.PerformanceCriticalCodeInvocation")]
        public void OncePerSecondDebugLog(
            object message,
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0
        ) => OncePerSecond(() => Debug.Log(message), sourceFilePath, sourceLineNumber);

        /// <summary>
        /// Call <c>callable()</c> unless it was executed from the calling location in the last second. In this case do nothing.
        /// Don't call this method twice on the same line.
        /// </summary>
        /// <param name="callable">Will be called whenever one second has passed since the last call, or if it is the first call.</param>
        /// <param name="sourceFilePath">Ignore. Don't set manually. Correct file path is filled in by the compiler.</param>
        /// <param name="sourceLineNumber">Ignore. Don't set manually. Correct line number is filled in by the compiler.</param>
        [SuppressMessage("ReSharper", "ExplicitCallerInfoArgument")]
        [SuppressMessage("ReSharper", "Unity.PerformanceCriticalCodeInvocation")]
        public void OncePerSecond(
            Action callable,
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0
        ) => OncePerIntervallSeconds(callable, 1, sourceFilePath, sourceLineNumber);

        /// <summary>
        /// Call <c>callable()</c> unless it was executed from the calling location in the last intervall. In this case do nothing.
        /// Don't call this method twice on the same line.
        /// </summary>
        /// <param name="callable">Called whenever the intervall has elapsed, or if it is the first call.</param>
        /// <param name="intervallSeconds">The time between calls.</param>
        /// <param name="sourceFilePath">Ignore. Don't set manually. Correct file path is filled in by the compiler.</param>
        /// <param name="sourceLineNumber">Ignore. Don't set manually. Correct line number is filled in by the compiler.</param>
        [SuppressMessage("ReSharper", "ExplicitCallerInfoArgument")]
        [SuppressMessage("ReSharper", "Unity.PerformanceCriticalCodeInvocation")]
        public void OncePerIntervallSeconds(
            Action callable,
            int intervallSeconds,
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0
        ) => OncePerIntervallMilliseconds(callable, intervallSeconds * 1000, sourceFilePath, sourceLineNumber);

        /// <summary>
        /// Call <c>callable()</c> unless it was executed from the calling location in the last intervall. In this case do nothing.
        /// Don't call this method twice on the same line.
        /// </summary>
        /// <param name="callable">Called whenever the intervall has elapsed, or if it is the first call.</param>
        /// <param name="intervallMilliseconds">The time between calls.</param>
        /// <param name="sourceFilePath">Ignore. Don't set manually. Correct file path is filled in by the compiler.</param>
        /// <param name="sourceLineNumber">Ignore. Don't set manually. Correct line number is filled in by the compiler.</param>
        [SuppressMessage("ReSharper", "Unity.PerformanceCriticalCodeInvocation")]
        public void OncePerIntervallMilliseconds(
            Action callable,
            int intervallMilliseconds,
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0
        ) => CallIfTimeElapsed($"{sourceFilePath}:{sourceLineNumber}", callable, intervallMilliseconds);

        [SuppressMessage("ReSharper", "Unity.PerformanceCriticalCodeInvocation")]
        private void CallIfTimeElapsed(string id, Action callable, int ms)
        {
            var now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

            // add the caller id on the first call
            var isFirstCall = _sourceTimes.TryAdd(id, now);

            // return if this is not the first call and the time between calls has not yet elapsed
            if (!isFirstCall && now <= _sourceTimes[id] + ms) return;

            _sourceTimes[id] = now;
            callable();
        }
    }
}