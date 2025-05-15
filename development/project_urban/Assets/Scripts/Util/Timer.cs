using System;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using UnityEngine;
using Object = System.Object;

namespace Util
{
    public static class Timer
    {
        private static long? _oncePerSecondTime = null;

        public static void OncePerSecondDebugLog(string message) => OncePerSecondCall(() => Debug.Log(message));

        public static void OncePerSecondCall(Action callable)
        {
            var now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            // this is not good enough for anything other than logging. could be fine if it were no longer static.
            if (_oncePerSecondTime is null || now > _oncePerSecondTime + 1000)
            {
                _oncePerSecondTime = now;
                callable();
            }
        }

        [CanBeNull]
        public static T OncePerSecondCallGeneric<T>(Func<T> callable) where T : class
        {
            var now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            if (_oncePerSecondTime is null || now > _oncePerSecondTime + 1000)
            {
                _oncePerSecondTime = now;
                return callable();
            }

            return null;
        }
    }
}