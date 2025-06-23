using System;

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
        public float totalAlge { get; private set; }
        public float totalSalzpflanze { get; private set; }
        public float totalQualle { get; private set; }
        public float totalGrille { get; private set; }

        public float maxAlgeAmount { get; private set; }
        public float minAlgeAmount { get; private set; }
      


        private FoodEconomy()
        {
            CurrentProteinAmount = 10_000;
            MaxProteinAmount = 10_000;
            MinProteinAmount = 0;
            _totalConsumption = 0;
            _totalProduction = 0;

            totalAlge = 0;
            totalSalzpflanze = 0;
            totalQualle = 0;
            totalGrille = 0;
            //maxAlgeAmount = 10_000;
            //minAlgeAmount = 0;
            totalSalzpflanze = 0;
            totalQualle = 0;
            totalGrille = 0;

            //BuildingInstance.ConsumeFood += OnConsumeFood;
            //BuildingInstance.ProduceFood += OnProduceFood;

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
            totalSalzpflanze += salzpflanzeProSek;
        }
        private void OnProduceQualle(float qualleProSek)
        {
            totalQualle += qualleProSek;
        }
        private void OnProduceGrille(float grilleProSek)
        {
            totalGrille += grilleProSek;
        }

        private void OnProduceAlge(float algeProSek)
        {
            // var nextAlgeAmount = Math.Min(maxAlgeAmount, totalAlge + algeProSek);
            totalAlge += algeProSek;
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