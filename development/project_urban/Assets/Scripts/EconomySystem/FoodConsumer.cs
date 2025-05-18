using System;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Serialization;
using Util;

namespace EconomySystem
{
    public class FoodConsumer : MonoBehaviour
    {
        public static event Action<float> ConsumeFood;
        public int foodConsumptionPerSecond = 10;

        public void Start()
        {
            Debug.Log($"This building consumes {foodConsumptionPerSecond}/second.");
        }

        public void FixedUpdate()
        {
            ConsumeFood?.Invoke(foodConsumptionPerSecond * Time.fixedDeltaTime);
        }
    }
}