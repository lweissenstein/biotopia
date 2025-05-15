using System;
using UnityEngine;
using Util;

namespace EconomySystem
{
    public sealed class FoodEconomy
    {
        private static FoodEconomy _instance = null;

        private int _currentProteinAmount;
        private int _totalConsumption;
        private int _totalProduction;

        private FoodEconomy()
        {
            _currentProteinAmount = 1000;
            _totalConsumption = 0;
            _totalProduction = 0;

            FoodConsumer.ConsumeFood += OnConsumeFood;
            FoodProducer.ProduceFood += OnProduceFood;
        }

        private void OnConsumeFood(int consumptionPerFrame)
        {
            _currentProteinAmount = Math.Max(0, _currentProteinAmount - consumptionPerFrame);
            _totalConsumption += consumptionPerFrame;
        }

        private void OnProduceFood(int productionPerFrame)
        {
            _currentProteinAmount = Math.Min(1_000_000, _currentProteinAmount + productionPerFrame);
            _totalProduction += productionPerFrame;
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