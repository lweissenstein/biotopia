//using System;
//using UnityEngine;

//namespace EconomySystem
//{
//    public class FoodConsumer : MonoBehaviour
//    {
//        // Event, das ausgelöst wird, wenn Nahrung verbraucht wird (Parameter: Menge der verbrauchten Nahrung)
//        public static event Action<float> ConsumeFood;

//        // Referenz auf die BuildingInstance-Komponente dieses GameObjects
//        private BuildingInstance buildingInstance;

//        // Aktueller Nahrungsverbrauch pro Sekunde (wird aus BuildingInstance gelesen)
//        [SerializeField] private float foodConsumptionPerSecond;

//        private void Awake()
//        {
//            // Sucht die BuildingInstance-Komponente am selben GameObject
//            buildingInstance = GetComponent<BuildingInstance>();
//            if (buildingInstance == null)
//            {
//                // Gibt eine Fehlermeldung aus, falls keine BuildingInstance gefunden wurde
//                Debug.LogError("BuildingInstance-Komponente nicht gefunden!");
//            }
//        }

//        public void Start()
//        {
//            // Initialisiert den Verbrauchswert beim Start
//            UpdateConsumption();
//            Debug.Log($"This building consumes {foodConsumptionPerSecond}/second.");
//        }

//        // Holt den aktuellen Verbrauchswert von der BuildingInstance
//        private void UpdateConsumption()
//        {
//            foodConsumptionPerSecond = buildingInstance.GetConsumption(); //methoden in BuildingInstance.cs schreiben
//        }

//        public void FixedUpdate()
//        {
//            // Falls keine BuildingInstance vorhanden ist, wird abgebrochen
//            if (buildingInstance == null) return;

//            // Aktualisiert den Verbrauchswert, um immer aktuelle Werte zu verwenden
//            UpdateConsumption();

//            // Löst das ConsumeFood-Event aus und übergibt die verbrauchte Menge für diesen FixedUpdate-Zyklus
//            ConsumeFood?.Invoke(foodConsumptionPerSecond * Time.fixedDeltaTime);
//        }
//    }
//}
