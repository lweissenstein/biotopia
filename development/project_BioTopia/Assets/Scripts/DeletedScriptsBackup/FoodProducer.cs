//using System;
//using UnityEngine;
//using UnityEngine.Serialization;
//using Util;

//namespace EconomySystem
//{
//    public class FoodProducer : MonoBehaviour
//    {
//        public static event Action<float> ProduceFood;
//        [SerializeField] private int foodProductionPerSecond = 30;

//        private void Start()
//        {
//            Debug.Log($"this building produces {foodProductionPerSecond}/second.");
//        }

//        public void FixedUpdate()
//        {
//            ProduceFood?.Invoke(foodProductionPerSecond * Time.fixedDeltaTime);
//        }
//    }
//}