using System;
using System.Collections.Generic;

namespace EconomySystem
{
    public record FoodResource(float Total, float Min, float Max)
    {
        public float Total { get; set; } = Total;
        public float Min { get; set; } = Min;
        public float Max { get; set; } = Max;
    }

    public sealed class FoodEconomy
    {
        private static FoodEconomy _instance = null;

        public readonly FoodResource Alge = new(Total: 1000, Min: 0, Max: 10_000);
        public readonly FoodResource Qualle = new(Total: 2000, Min: 0, Max: 10_000);
        public readonly FoodResource SalzPflanze = new(Total: 3000, Min: 0, Max: 10_000);
        public readonly FoodResource Grille = new(Total: 4000, Min: 0, Max: 10_000);

        private float _totalConsumption;
        private float _totalProduction;
        public float CurrentProteinAmount = 556;
        //public float CurrentProteinAmount { get; private set; }
        public float MaxProteinAmount = 1000;
        public float MinProteinAmount = 0;

        public Dictionary<ResourceType, float> totalResources = new Dictionary<ResourceType, float>();
        public Dictionary<ProductType, float> totalProducts = new Dictionary<ProductType, float>();


        private FoodEconomy()
        {
            _totalConsumption = 0;
            _totalProduction = 0;

            BuildingInstance.ConsumeFood += OnConsumeFood;
            BuildingInstance.ProduceFood += OnProduceFood;

            foreach (ResourceType type in Enum.GetValues(typeof(ResourceType)))
            {
                totalResources[type] = 0f;
            }
            foreach (ProductType type in Enum.GetValues(typeof(ProductType)))
            {
                totalProducts[type] = 0f;
            }


            BuildingInstance.ProduceResource += OnProduceResource;
            BuildingInstance.ConsumeProduct += OnConsumeProduct;
            ProcessInstance.ProduceProduct += OnProduceProduct;

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

        private void OnProduceResource(ResourceType type, float amount)
        {
            totalResources[type] += amount;
        }
        private void OnProduceProduct(ProductType prod, float prodAmount, ResourceType res, float resAmount)
        {
            totalProducts[prod] += prodAmount;
            totalResources[res] -= resAmount;
        }

        private void OnConsumeProduct(ProductType prod, float amount)
        {
            totalProducts[prod] -= amount;
        }

        public bool IsProductAvailable(ProductType type, float amount)
        {
            return totalProducts.ContainsKey(type) && totalProducts[type] >= amount;
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