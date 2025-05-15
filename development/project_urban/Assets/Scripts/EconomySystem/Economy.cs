using System;
using NUnit.Framework.Constraints;
using UnityEngine;
using Util;

namespace EconomySystem
{
    public class Economy : MonoBehaviour
    {
        private readonly FoodEconomy _foodEconomy = FoodEconomy.Instance;
        public bool isDebug = false;

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
                Timer.OncePerSecondDebugLog($"FOOD: {_foodEconomy.Report()}");
            }
        }
    }
}