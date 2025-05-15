using System;
using UnityEngine;
using UnityEngine.Serialization;
using Util;

namespace EconomySystem
{
    public class FoodProducer : MonoBehaviour
    {
        public static event Action<int> ProduceFood;
        public int foodProductionPerFrame = 30;

        private void Start()
        {
            Debug.Log($"this building produces {foodProductionPerFrame}/frame.");
        }

        public void Update()
        {
            ProduceFood?.Invoke(foodProductionPerFrame);
            Timer.OncePerSecondDebugLog($"FOOD: produce {foodProductionPerFrame}/frame");
        }
    }
}