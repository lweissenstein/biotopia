using System;
using UnityEngine;
using EconomySystem;
using System.Collections.Generic;
using System.Linq;
using Debug = UnityEngine.Debug;

public class BuildingInstance : MonoBehaviour
{
    // General

    [SerializeField] private BoxCollider boxColliderHouse;
    public BuildingData data;
    public GameObject previewPrefab;
    public static event Action<ProductType> Consume;
    private ObjectPlacer objectPlacer;
    private ProcessSelectionManager processSelectionManager;
    private FoodEconomy foodEconomy;
    public Vector3 pos;
    public RandomGridManipulation gridManipulation;
    public PlacementSystem placementSystem;
    public ProductDescriptionDatabase productDatabase;
    public SoundFeedback soundFeedback;

    // Hochhaus

    public static event Action<ResourceType, float> ProduceResource;
    

    public Dictionary<string, int> compartmentPrices = new Dictionary<string, int>
{
    { "Alge", 100 },
    { "Qualle", 100 },
    { "Salzpflanze", 100 },
    { "Grille", 100 },
    { "Supermarkt", 100 }
};

    public float countCompartmentsHouse = 0f;
    public int maxCompartments = 1;
    public int residents;
    public int height = 1;
    public int compartmentTypeHouse = 0; // 7 = supermarkt, 3 = alge, 4 = salzpflanze, 5 = qualle, 6 = grille
    private bool isProducing = false;
    public bool hasSupermarket;
    public string productionAsString;
    public bool compartmentUpgradeableBeaconOn = false;

    public float produceAlgeValue = 0.5f;
    public float produceSalzpflanzeValue = 0.5f;
    public float produceQualleValue = 0.5f;
    public float produceGrillevalue = 0.5f;
    public float producePerSecond = 0f; // Gesamtproduktion pro Sekunde, abh�ngig von den Upgrades



    // Water

    // Park

    // Therme

    // Supermarkt

    // Fabrik? 



    // General: Awake and Update
    public void Awake()
    {
        // General
        residents = 50;
        foodEconomy = FoodEconomy.Instance;
        processSelectionManager = FindFirstObjectByType<ProcessSelectionManager>();
        placementSystem = FindFirstObjectByType<PlacementSystem>();
        soundFeedback = FindFirstObjectByType<SoundFeedback>();


    }

    // ------------ General ------------



    public void TryConsumeProduct()
    {
        if (hasSupermarket)
        {
            var unlocked = processSelectionManager.GetUnlockedProducts();
            var available = unlocked.Where(p => foodEconomy.HasProduct(p)).ToList();
            if (available.Count == 0) return;

            ProductType chosen = ChooseProduct(available);
            UnityEngine.Debug.Log($"Chosen product: {chosen}");
            Consume?.Invoke(chosen);
        }
        else
        {


        }
    }
    public void ConsumeTotalSaturation()
    {
        foodEconomy.OnConsumeFood(0.00013f * residents);
    }

    private ProductType ChooseProduct(List<ProductType> list)
    {
        if (productDatabase == null)
        {
            Debug.LogError("productDB ist null in ChooseProduct!");
            return list[0]; // fallback
        }


        foreach (var p in list)
        {
            if (productDatabase.GetEntry(p) == null)
            {
                Debug.LogError($"Kein Produkt gefunden für {p}");
            }
        }

            // Zufällig mit leichter Gewichtung für bessere Produkte
            var weighted = list.Select(p => new {
            product = p,
            weight = Mathf.Pow(productDatabase.GetEntry(p).saturation, 1.2f)
        }).ToList();

        float total = weighted.Sum(w => w.weight);
        float rnd = UnityEngine.Random.Range(0, total);
        float current = 0;

        foreach (var w in weighted)
        {
            current += w.weight;
            if (rnd <= current) { 
                Debug.Log($"Selected product: {w.product} with weight: {w.weight}");
                return w.product;
            }
        }
        return list[0]; // fallback
    }

    // ------------ Hochhaus ------------
    public void UpgradeHouse()
    {
        if(height > 2)
        {
            height = 2;
        }
        height++;
        if (height == 2)
            maxCompartments = 3;
        else if (height == 3)
            maxCompartments = 6;
        if (isProducing)
        {
            var upgradeable = GetComponent<UpgradeableObject>();
            if (upgradeable != null)
                upgradeable.SetActiveSolarPanels(height);
                if (isProducing)
                    upgradeable.ChangeColor(productionAsString, height);
            if (countCompartmentsHouse < maxCompartments)
            {
                compartmentUpgradeableBeaconOn = true;
                upgradeable.ToggleCompartmentUpgradeableBeacon(compartmentTypeHouse, compartmentUpgradeableBeaconOn);
            }
            else 
            {
                compartmentUpgradeableBeaconOn = false;
                upgradeable.ToggleCompartmentUpgradeableBeacon(compartmentTypeHouse, compartmentUpgradeableBeaconOn);
            }
        }
        UpdateCollider();
    }

    public void UpgradeCompartmentAlge()
    {
        var upgradeable = GetComponent<UpgradeableObject>();
        if (countCompartmentsHouse == 0 && countCompartmentsHouse <= maxCompartments)
        {
            if (upgradeable != null)
                upgradeable.SetActiveSolarPanels(height);
            productionAsString = "Alge";
                upgradeable.ChangeColor(productionAsString, height);

            compartmentTypeHouse = 3;
            countCompartmentsHouse++;
            isProducing = true;
            producePerSecond = produceAlgeValue * countCompartmentsHouse;
            soundFeedback.PlaceCompartmentAlge();
        }
        else if (countCompartmentsHouse != 0 && countCompartmentsHouse < maxCompartments) { 
            countCompartmentsHouse++;
            producePerSecond = produceAlgeValue * countCompartmentsHouse;

        }
        else
        {
            Debug.LogWarning("Maximale Anzahl an Upgrades erreicht.");
        }
        if (countCompartmentsHouse < maxCompartments)
        {
            compartmentUpgradeableBeaconOn = true;
            upgradeable.ToggleCompartmentUpgradeableBeacon(compartmentTypeHouse, compartmentUpgradeableBeaconOn);
        }
        else
        {
            compartmentUpgradeableBeaconOn = false;
            upgradeable.ToggleCompartmentUpgradeableBeacon(compartmentTypeHouse, compartmentUpgradeableBeaconOn);
        }
    }

    public void UpgradeCompartmentSalzpflanze()
    {
        var upgradeable = GetComponent<UpgradeableObject>();
        if (countCompartmentsHouse == 0 && countCompartmentsHouse <= maxCompartments)
        {
            if (upgradeable != null)
                upgradeable.SetActiveSolarPanels(height);
            productionAsString = "Halophyte";
            upgradeable.ChangeColor(productionAsString, height);

            compartmentTypeHouse = 4;
            countCompartmentsHouse++;
            isProducing = true;
            producePerSecond = produceSalzpflanzeValue * countCompartmentsHouse;
            soundFeedback.PlaceCompartmentHalophyte();
        }
        else if (countCompartmentsHouse != 0 && countCompartmentsHouse < maxCompartments) { 
            countCompartmentsHouse++;
            producePerSecond = produceSalzpflanzeValue * countCompartmentsHouse;
        }
        else
        {
            Debug.LogWarning("Maximale Anzahl an Upgrades erreicht.");
        }
        if (countCompartmentsHouse < maxCompartments)
        {
            compartmentUpgradeableBeaconOn = true;
            upgradeable.ToggleCompartmentUpgradeableBeacon(compartmentTypeHouse, compartmentUpgradeableBeaconOn);
        }
        else
        {
            compartmentUpgradeableBeaconOn = false;
            upgradeable.ToggleCompartmentUpgradeableBeacon(compartmentTypeHouse, compartmentUpgradeableBeaconOn);
        }
    }

    public void UpgradeCompartmentQualle()
    {
        var upgradeable = GetComponent<UpgradeableObject>();
        if (countCompartmentsHouse == 0 && countCompartmentsHouse <= maxCompartments)
        {
            if (upgradeable != null)
                upgradeable.SetActiveSolarPanels(height);
            productionAsString = "Qualle";
            upgradeable.ChangeColor(productionAsString, height);

            compartmentTypeHouse = 5;
            countCompartmentsHouse++;
            isProducing = true;
            producePerSecond = produceQualleValue * countCompartmentsHouse;
            soundFeedback.PlaceCompartmentQualle();
        }
        else if (countCompartmentsHouse != 0 && countCompartmentsHouse < maxCompartments) { 
            countCompartmentsHouse++;
            producePerSecond = produceQualleValue * countCompartmentsHouse;
        }
        else
        {
            Debug.LogWarning("Maximale Anzahl an Upgrades erreicht.");
        }
        if (countCompartmentsHouse < maxCompartments)
        {
            compartmentUpgradeableBeaconOn = true;
            upgradeable.ToggleCompartmentUpgradeableBeacon(compartmentTypeHouse, compartmentUpgradeableBeaconOn);
        }
        else
        {
            compartmentUpgradeableBeaconOn = false;
            upgradeable.ToggleCompartmentUpgradeableBeacon(compartmentTypeHouse, compartmentUpgradeableBeaconOn);
        }
    }

    public void UpgradeCompartmentGrille()
    {
        var upgradeable = GetComponent<UpgradeableObject>();
        if (countCompartmentsHouse == 0 && countCompartmentsHouse <= maxCompartments)
        {
            if (upgradeable != null)
                upgradeable.SetActiveSolarPanels(height);
            productionAsString = "Grille";
            upgradeable.ChangeColor(productionAsString, height);

            compartmentTypeHouse = 6;
            countCompartmentsHouse++;
            isProducing = true;
            producePerSecond = produceGrillevalue * countCompartmentsHouse;
            soundFeedback.PlaceCompartmentGrille();
        }
        else if (countCompartmentsHouse != 0 && countCompartmentsHouse < maxCompartments) { 
            countCompartmentsHouse++;
            producePerSecond = produceGrillevalue * countCompartmentsHouse;
        } 
        else
        {
            Debug.LogWarning("Maximale Anzahl an Upgrades erreicht.");
        }
        if (countCompartmentsHouse < maxCompartments)
        {
            compartmentUpgradeableBeaconOn = true;
            upgradeable.ToggleCompartmentUpgradeableBeacon(compartmentTypeHouse, compartmentUpgradeableBeaconOn);
        }
        else
        {
            compartmentUpgradeableBeaconOn = false;
            upgradeable.ToggleCompartmentUpgradeableBeacon(compartmentTypeHouse, compartmentUpgradeableBeaconOn);
        }
    }


    public void UpgradeSupermarkt()
    {
        countCompartmentsHouse = maxCompartments;
        residents = 0;
        var upgradeable =
            GetComponent<UpgradeableObject>();
        if (upgradeable != null)
            upgradeable.SetToSupermarket();
        compartmentTypeHouse = 7;
        placementSystem.PingSuperMarket(pos);
        soundFeedback.PlaceCompartmentSupermarkt();
    }


    public void GetProduction()
    {
        switch (compartmentTypeHouse)
        {
            case 3: // Alge
                ProduceResource?.Invoke(ResourceType.Alge, produceAlgeValue * countCompartmentsHouse);
                break;
            case 4: // Salzpflanze
                ProduceResource?.Invoke(ResourceType.Salzpflanze, produceSalzpflanzeValue * countCompartmentsHouse);
                break;
            case 5: // Qualle
                ProduceResource?.Invoke(ResourceType.Qualle, produceQualleValue * countCompartmentsHouse);
                break;
            case 6: // Grille
                ProduceResource?.Invoke(ResourceType.Grille, produceGrillevalue * countCompartmentsHouse);
                break;
            default:
                break;
        }
   
    }

    // ------------ Water ------------
    // ------------ Park ------------
    // ------------ Therme ------------
    // ------------ Supermarkt ------------
    // ------------ Utility ------------

    // Passt die Gr??e und Position des Colliders je nach Level des Geb?udes an
    private void UpdateCollider()
    {
        if (boxColliderHouse == null) return;

        if (height == 1)
        {
            boxColliderHouse.size = new Vector3(1f, 0.5f, 1f);
            boxColliderHouse.center = new Vector3(0.5f, 0.25f, 0.5f);
        }
        else if (height == 2)
        {
            boxColliderHouse.size = new Vector3(1f, 1.25f, 1f);
            boxColliderHouse.center = new Vector3(0.5f, 0.6f, 0.5f);
        }
        else if (height >= 3)
        {
            boxColliderHouse.size = new Vector3(1f, 2f, 1f);
            boxColliderHouse.center = new Vector3(0.5f, 1f, 0.5f);
        }
    }


    public void PrintInfo()
    {
        if (data is House house) { }

        else if (data is Water water) { }
    }

    
}
