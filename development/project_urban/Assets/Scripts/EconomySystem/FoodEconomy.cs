using System;
using UnityEngine;

namespace EconomySystem
{
    public sealed class FoodEconomy
    {
        private static FoodEconomy _instance = null;

        private float _totalConsumption;
        private float _totalProduction;
        public float CurrentProteinAmount { get; private set; }
        public float MaxProteinAmount { get; private set; }
        public float MinProteinAmount { get; private set; }

        private FoodEconomy()
        {
            CurrentProteinAmount = 10_000;
            MaxProteinAmount = 10_000;
            MinProteinAmount = 0;
            _totalConsumption = 0;
            _totalProduction = 0;

            BuildingInstance.ConsumeFood += OnConsumeFood;
            BuildingInstance.ProduceFood += OnProduceFood;
        }

        private void OnConsumeFood(float consumptionPerFixedUpdate)
        {
            var nextProteinAmount = Math.Max(MinProteinAmount, CurrentProteinAmount - consumptionPerFixedUpdate);
            _totalConsumption += CurrentProteinAmount - nextProteinAmount;
            CurrentProteinAmount = nextProteinAmount;
        }

        private void OnProduceFood(float productionPerFixedUpdate)
        {
            var nextProteinAmount = Math.Min(MaxProteinAmount, CurrentProteinAmount + productionPerFixedUpdate);
            _totalProduction += nextProteinAmount - CurrentProteinAmount;
            CurrentProteinAmount = nextProteinAmount;
        }

        public string Report()
        {
            return $"total:  consumption={_totalConsumption} production={_totalProduction}";
        }

        /// <summary>
        /// The Singleton instance of FoodEconomy.
        /// </summary>
        public static FoodEconomy Instance
        {
            get
            {
                if (_instance is null)
                {
                    _instance = new FoodEconomy();
                }

                return _instance;
            }
        }
    }
}