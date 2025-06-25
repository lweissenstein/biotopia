using System;
using UnityEngine;
using Util;

public class BuildingInstance : MonoBehaviour
{
// General

    [SerializeField] private BoxCollider boxColliderHouse;
    public BuildingData data;
    public GameObject previewPrefab;
    //public static event Action<float> ConsumeFood;
    //public static event Action<float> ProduceFood;
    private ObjectPlacer objectPlacer;
    private float fixedTimer = 0f;
    private Timer _timer = new();


    // Hochhaus

    public static event Action<float> ProduceAlge;
    public static event Action<float> ProduceSalzpflanze;
    public static event Action<float> ProduceQualle;
    public static event Action<float> ProduceGrille;

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
    public float producePerSecond = 0f; // Gesamtproduktion pro Sekunde, abhängig von den Upgrades


    // Water

    // Park

    // Therme

    // Supermarkt

    // Fabrik? 



    // General: Awake and Update
    public void Awake()
    {
    // General
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
                //Debug.Log("Script läuft auf: " + gameObject.name);
            }

            _timer.OncePerSecondDebugLog("produce PerSecond: " + producePerSecond);
        }

        

        // Hochhaus
        // Water
        // Park
        // Therme
        // Supermarkt
        //ProduceFood?.Invoke(foodProductionPerSecond);
        //ConsumeFood?.Invoke(foodConsumptionPerSecond * Time.fixedDeltaTime);
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

    public void GetProduction()
    {
        switch (compartmentTypeHouse)
        {
            case 3: // Alge
                ProduceAlge?.Invoke(produceAlgeValue * countCompartmentsHouse);
                break;
            case 4: // Salzpflanze
                ProduceSalzpflanze?.Invoke(produceSalzpflanzeValue * countCompartmentsHouse);
                break;
            case 5: // Qualle
                ProduceQualle?.Invoke(produceQualleValue * countCompartmentsHouse);
                break;
            case 6: // Grille
                ProduceGrille?.Invoke(produceGrillevalue * countCompartmentsHouse);
                break;
            default:
                Debug.Log("Keine Produktion für diesen Typ: " + compartmentTypeHouse);
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
