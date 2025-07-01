using UnityEngine;

namespace Util
{
    public static class GamePauser
    {
        private static float? _previousTimeScale = null;
        private static Timer _timer = new();
        public static bool IsPaused => _previousTimeScale != null;

        public static void PauseGame()
        {
            if (_previousTimeScale is null)
            {
                _previousTimeScale = Time.timeScale;
                // this sets the speed of the game. 0 is frozen, 1 is regular speed.
                Time.timeScale = 0;
            }
            else
            {
                _timer.OncePerSecond(() => Debug.LogError("already frozen"));
            }
        }

        public static void ContinueGame()
        {
            if (_previousTimeScale is not null)
            {
                Time.timeScale = (float)_previousTimeScale;
                _previousTimeScale = null;
            }
            else
            {
                _timer.OncePerSecond(() => Debug.LogError("not frozen"));
            }
        }
    }
}