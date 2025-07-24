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
        public float CurrentProteinAmount = 500;
        //public float CurrentProteinAmount { get; private set; }
        public float MaxProteinAmount = 1000;
        public float MinProteinAmount = 0;


        public Dictionary<ResourceType, float> totalResources = new Dictionary<ResourceType, float>();
        public Dictionary<ProductType, int> totalProducts = new Dictionary<ProductType, int>();


        private FoodEconomy()
        {
            _totalConsumption = 0;
            _totalProduction = 0;

            foreach (ResourceType type in Enum.GetValues(typeof(ResourceType)))
            {
                totalResources[type] = 0f;
            }
            foreach (ProductType type in Enum.GetValues(typeof(ProductType)))
            {
                totalProducts[type] = 0;
            }


            BuildingInstance.ProduceResource += OnProduceResource;
            ProcessInstance.ProduceProduct += OnProduceProduct;

        }

        public void OnConsumeFood(float consumption)
        {
            if (CurrentProteinAmount <= 0)
            {
                return; // No food to consume
            }
            CurrentProteinAmount -= consumption;
        }

        public void OnProduceFood(float consumption)
        {
            if (CurrentProteinAmount >= MaxProteinAmount)
            {
                return; // Cannot produce more food than max capacity
            }
            CurrentProteinAmount += consumption;
        }

        private void OnProduceResource(ResourceType type, float amount)
        {
            totalResources[type] += amount;
        }
        private void OnProduceProduct(ProductType prod, int prodAmount, ResourceType res, float resAmount)
        {
            totalProducts[prod] += prodAmount;
            totalResources[res] -= resAmount;
        }

        private void OnConsumeResource(ResourceType type, float amount)
        {
            if (!totalResources.ContainsKey(type)) return;
            totalResources[type] = Math.Max(0, totalResources[type] - amount);
        }

        public void ConsumeProduct(ProductType prod, int amount)
        {
            if (!totalProducts.ContainsKey(prod)) return;
            totalProducts[prod] = Math.Max(0, totalProducts[prod] - amount);
        }

        public bool IsProductAvailable(ProductType type, int amount)
        {
            return totalProducts.ContainsKey(type) && totalProducts[type] >= amount;
        }

        public bool HasProduct(ProductType type)
        {
            return totalProducts.TryGetValue(type, out int count) && count > 0;
        }

        public void AddTutorial()
        {
            totalProducts[ProductType.AlgePowder] += 20;
            totalResources[ResourceType.Alge] += 10;
        }
        public void RemoveTutorial()
        {
            totalProducts[ProductType.AlgePowder] = 0;
            totalResources[ResourceType.Alge] = 0;
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

        public static void Reset()
        {
            _instance = null;
        }
    }
}