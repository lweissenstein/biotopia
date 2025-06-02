using System;
using UnityEngine;

public class BuildingInstance : MonoBehaviour
{
    [SerializeField] private BoxCollider boxColliderHouse;
    public BuildingData data;
    public int level;
    public int algenCompartments;
    public int residents;
    public GameObject previewPrefab;
    public float foodConsumptionPerSecond;
    public float foodProductionPerSecond;

    public static event Action<float> ConsumeFood;
    public static event Action<float> ProduceFood;

    public void Awake()
    {
        foodProductionPerSecond = 0; // erstmal
        ProduceFood?.Invoke(foodProductionPerSecond);
    }
    public void FixedUpdate()
    {
        foodConsumptionPerSecond = GetConsumption();
        // Löst das ConsumeFood-Event aus und übergibt die verbrauchte Menge für diesen FixedUpdate-Zyklus
        ConsumeFood?.Invoke(foodConsumptionPerSecond * Time.fixedDeltaTime);
    }

    public float GetConsumption() // die Methode kann in anderen Scripts aufgerufen werden
    {
        return data.baseConsumption * level;
    }

    // Erhöht das Level des Gebäudes um 1, gibt eine Debug-Nachricht aus und passt den Collider an das neue Level an
    public void Upgrade()
    {
        level++;
        Debug.Log("Building upgraded to level: " + level);
        UpdateCollider();
    }

    // Gibt Informationen über das Gebäude im Debug-Log aus
    public void PrintInfo()
    {
        Debug.Log("Name: " + data.buildingName);
        Debug.Log("Basisproduktion: " + data.baseProduction);
        Debug.Log("Level: " + level);

        // Gibt zusätzliche Informationen je nach Gebäudetyp aus
        if (data is House house)
        {
            // Für Häuser: Einwohnerzahl und aktueller Verbrauch inkl. Algenkompartiment-Produktion
            Debug.Log("Einwohner: " + house.baseResidents + residents);
            Debug.Log("Aktueller Verbrauch: " + GetConsumption() + (house.algeCompartmentProduction * algenCompartments));
        }
        else if (data is Water water)
        {
            // Für Wassergebäude: Temperatur
            Debug.Log("Temperatur: " + water.baseTemperature);
        }
    }

    // Passt die Größe und Position des Colliders je nach Level des Gebäudes an
    private void UpdateCollider()
    {
        if (boxColliderHouse == null) return;

        if (level == 1)
        {
            boxColliderHouse.size = new Vector3(1f, 0.5f, 1f);
            boxColliderHouse.center = new Vector3(0.5f, 0.25f, 0.5f);
        }
        else if (level == 2)
        {
            boxColliderHouse.size = new Vector3(1f, 1.25f, 1f);
            boxColliderHouse.center = new Vector3(0.5f, 0.6f, 0.5f);
        }
        else if (level >= 3)
        {
            boxColliderHouse.size = new Vector3(1f, 2f, 1f);
            boxColliderHouse.center = new Vector3(0.5f, 1f, 0.5f);
        }
    }
}
