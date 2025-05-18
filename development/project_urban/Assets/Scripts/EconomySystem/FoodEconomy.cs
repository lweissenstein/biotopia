using System;
using UnityEngine;
using Util;

namespace EconomySystem
{
    public sealed class FoodEconomy
    {
        private static FoodEconomy _instance = null;

        private float _currentProteinAmount;
        private float _totalConsumption;
        private float _totalProduction;

        private FoodEconomy()
        {
            _currentProteinAmount = 1000;
            _totalConsumption = 0;
            _totalProduction = 0;

            FoodConsumer.ConsumeFood += OnConsumeFood;
            FoodProducer.ProduceFood += OnProduceFood;
        }

        private void OnConsumeFood(float consumptionPerFixedUpdate)
        {
            _currentProteinAmount = Math.Max(0, _currentProteinAmount - consumptionPerFixedUpdate);
            _totalConsumption += consumptionPerFixedUpdate;
        }

        private void OnProduceFood(float productionPerFixedUpdate)
        {
            _currentProteinAmount = Math.Min(1_000_000, _currentProteinAmount + productionPerFixedUpdate);
            _totalProduction += productionPerFixedUpdate;
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