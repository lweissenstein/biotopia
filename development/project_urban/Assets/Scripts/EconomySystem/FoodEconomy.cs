using System;

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
        public float CurrentProteinAmount { get; private set; } = 10_000;
        public float MaxProteinAmount { get; private set; } = 10_000;
        public float MinProteinAmount { get; private set; } = 0;
        public float totalAlge => Alge.Total;
        public float totalQualle => Qualle.Total;
        public float totalSalzpflanze => SalzPflanze.Total;
        public float totalGrille => Grille.Total;
        public float maxAlgeAmount => Alge.Max;
        public float minAlgeAmount => Alge.Min;

        private FoodEconomy()
        {
            _totalConsumption = 0;
            _totalProduction = 0;

            BuildingInstance.ProduceAlge += OnProduceAlge;
            BuildingInstance.ProduceSalzpflanze += OnProduceSalzpflanze;
            BuildingInstance.ProduceQualle += OnProduceQualle;
            BuildingInstance.ProduceGrille += OnProduceGrille;
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

        private void OnConsumeAlge(float consumptionPerFixedUpdate)
        {
        }

        private void OnProduceSalzpflanze(float salzpflanzeProSek)
        {
            SalzPflanze.Total += salzpflanzeProSek;
        }

        private void OnProduceQualle(float qualleProSek)
        {
            Qualle.Total += qualleProSek;
        }

        private void OnProduceGrille(float grilleProSek)
        {
            Grille.Total += grilleProSek;
        }

        private void OnProduceAlge(float algeProSek)
        {
            // var nextAlgeAmount = Math.Min(maxAlgeAmount, totalAlge + algeProSek);
            Alge.Total += algeProSek;
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