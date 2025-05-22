using UnityEngine;
using Util;

namespace EconomySystem
{
    public class Economy : MonoBehaviour
    {
        private readonly FoodEconomy _foodEconomy = FoodEconomy.Instance;
        private readonly Timer _timer = new();
        [SerializeField] private bool isDebug = false;

        private void Start()
        {
            if (isDebug)
            {
                Debug.Log("starting economy");
            }
        }

        private void Update()
        {
            if (isDebug)
            {
                _timer.OncePerSecondDebugLog($"FOOD: {_foodEconomy.Report()}");
            }
        }
    }
}