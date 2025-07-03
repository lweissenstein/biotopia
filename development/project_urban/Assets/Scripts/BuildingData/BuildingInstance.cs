using System;
using UnityEngine;
using EconomySystem;
using Util;
using Random = UnityEngine.Random;

public class BuildingInstance : MonoBehaviour
{
// General

    [SerializeField] private BoxCollider boxColliderHouse;
    public BuildingData data;
    public GameObject previewPrefab;
    public static event Action<float> ConsumeFood;
    public static event Action<float> ProduceFood;
    public static event Action<ProductType, float> ConsumeProduct;
    private ObjectPlacer objectPlacer;
    private float fixedTimer = 0f;
    private Timer _timer = new();
    private ProcessSelectionManager processSelectionManager;
    private ProductDescriptionDatabase productDescriptionDatabase;

    // Hochhaus

    public static event Action<ResourceType, float> ProduceResource;

    public float countCompartmentsHouse = 0f;
    public int maxCompartments = 6;
    public int residents;
    public int height = 1;
    public int compartmentTypeHouse; // 7 = default none, 3 = alge, 4 = salzpflanze, 5 = qualle, 6 = grille
    private bool isProducing = false;

    public float produceAlgeValue = 0.05f;
    public float produceSalzpflanzeValue = 0.05f;
    public float produceQualleValue = 0.05f;
    public float produceGrillevalue = 0.05f;
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

        processSelectionManager = FindFirstObjectByType<ProcessSelectionManager>();

    // Hochhaus
    // Water
    // Park
    // Therme
    // Supermarkt
    }
    public void FixedUpdate()
    {
        // General

        if (isProducing)
        {
            fixedTimer += Time.fixedDeltaTime;

            if (fixedTimer >= 1f)
            {
                fixedTimer -= 1f; // oder -= 1f, wenn du es genauer willst
                GetProduction();
                //Debug.Log("Script l�uft auf: " + gameObject.name);
            }
        }

        

        // Hochhaus
        // Water
        // Park
        // Therme
        // Supermarkt
        ProduceFood?.Invoke(1);
        ConsumeFood?.Invoke(1 * Time.fixedDeltaTime);

        if (compartmentTypeHouse >= 3 && compartmentTypeHouse <= 6)
        {
            for (int i = 0; i < 4; i++)
            {
                int available = 0;

                if (processSelectionManager.purchased[4 * i]) available++;
                if (processSelectionManager.purchased[4 * i + 1]) available++;
                if (processSelectionManager.purchased[4 * i + 2]) available++;
                if (processSelectionManager.purchased[4 * i + 3]) available++;

                if (available != 0)
                {
                    int rnd = Random.Range(0, available);
                    ConsumeProduct?.Invoke(processSelectionManager.products[rnd + 4 * i], 1);
                }
            }
        }
    }

    // ------------ General ------------

    // ------------ Hochhaus ------------
    public void UpgradeHouse()
    {
        if(height > 2)
        {
            height = 2;
        }
        height++;
        UpdateCollider();

    }

    public void UpgradeCompartmentAlge()
    {
        if (countCompartmentsHouse == 0 && countCompartmentsHouse <= maxCompartments)
        {
            var upgradeable = GetComponent<UpgradeableObject>();
            if (upgradeable != null)
                upgradeable.ActivateChild(3);

            compartmentTypeHouse = 3;
            countCompartmentsHouse++;
            isProducing = true;
            producePerSecond = produceAlgeValue * countCompartmentsHouse;

        }
        else if (countCompartmentsHouse != 0 && countCompartmentsHouse < maxCompartments) { 
            countCompartmentsHouse++;
            producePerSecond = produceAlgeValue * countCompartmentsHouse;

        }
        else
        {
            Debug.LogWarning("Maximale Anzahl an Upgrades erreicht.");
        }

    }

    public void UpgradeCompartmentSalzpflanze()
    {
        if (countCompartmentsHouse == 0 && countCompartmentsHouse <= maxCompartments)
        {
            var upgradeable = GetComponent<UpgradeableObject>();
            if (upgradeable != null)
                upgradeable.ActivateChild(4);

            compartmentTypeHouse = 4;
            countCompartmentsHouse++;
            isProducing = true;
            producePerSecond = produceSalzpflanzeValue * countCompartmentsHouse;


        }
        else if (countCompartmentsHouse != 0 && countCompartmentsHouse < maxCompartments) { 
            countCompartmentsHouse++;
            producePerSecond = produceSalzpflanzeValue * countCompartmentsHouse;
        }
        else
        {
            Debug.LogWarning("Maximale Anzahl an Upgrades erreicht.");
        }
    }

    public void UpgradeCompartmentQualle()
    {
        if (countCompartmentsHouse == 0 && countCompartmentsHouse <= maxCompartments)
        {
            var upgradeable = GetComponent<UpgradeableObject>();
            if (upgradeable != null)
                upgradeable.ActivateChild(5);

            compartmentTypeHouse = 5;
            countCompartmentsHouse++;
            isProducing = true;
            producePerSecond = produceQualleValue * countCompartmentsHouse;

        }
        else if (countCompartmentsHouse != 0 && countCompartmentsHouse < maxCompartments) { 
            countCompartmentsHouse++;
            producePerSecond = produceQualleValue * countCompartmentsHouse;
        }
        else
        {
            Debug.LogWarning("Maximale Anzahl an Upgrades erreicht.");
        }
    }

    public void UpgradeCompartmentGrille()
    {
        if (countCompartmentsHouse == 0 && countCompartmentsHouse <= maxCompartments)
        {
            var upgradeable = GetComponent<UpgradeableObject>();
            if (upgradeable != null)
                upgradeable.ActivateChild(6);

            compartmentTypeHouse = 6;
            countCompartmentsHouse++;
            isProducing = true;
            producePerSecond = produceGrillevalue * countCompartmentsHouse;

        }
        else if (countCompartmentsHouse != 0 && countCompartmentsHouse < maxCompartments) { 
            countCompartmentsHouse++;
            producePerSecond = produceGrillevalue * countCompartmentsHouse;
        } 
        else
        {
            Debug.LogWarning("Maximale Anzahl an Upgrades erreicht.");
        }
    }

    public void UpgradeSupermarkt()
    {
        var upgradeable =
            GetComponent<UpgradeableObject>();
        if (upgradeable != null)
            upgradeable.SetToSupermarket();

        compartmentTypeHouse = 7;
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
                Debug.Log("Keine Produktion f�r diesen Typ: " + compartmentTypeHouse);
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
