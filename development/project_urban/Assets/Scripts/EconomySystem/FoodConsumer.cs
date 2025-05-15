using System;
using JetBrains.Annotations;
using UnityEngine;
using Util;

namespace EconomySystem
{
    public class FoodConsumer : MonoBehaviour
    {
        public static event Action<int> ConsumeFood;
        public int foodConsumptionPerFrame = 10;

        public void Start()
        {
            Debug.Log($"This building consumes {foodConsumptionPerFrame}/frame.");
        }

        public void Update()
        {
            ConsumeFood?.Invoke(foodConsumptionPerFrame);
            Timer.OncePerSecondDebugLog($"FOOD: consume {foodConsumptionPerFrame}/frame.");
        }
    }
}