using UnityEngine;
using EconomySystem;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "ProductDescriptions", menuName = "Scriptable Objects/ProductDescriptions")]
public class ProductDescriptionDatabase : ScriptableObject  // imagine name as "ProductDatabase" because its not only for entries anymore
{
    [System.Serializable]
    public class ProductDescriptionEntry
    {
        [Header("Produkt-Informationen")]
        public ProductType productType;
        [TextArea]
        public string description;
        public string name;
        public Sprite icon;
        public int price;

        [Header("Produktions-Basiswerte")]
        [InspectorName("Cooldown"), Tooltip("Cooldown in Seconds (baseSpeed)")]
        public float baseSpeed; // controls cooldown
        [InspectorName("Output"), Tooltip("Amount of Output (baseAmount)")]
        public float baseAmount; // controls amount produced
        [InspectorName("Quality"), Tooltip ("Money per Unit (baseQuality)")]
        public float baseQuality; // controls quality of product and thus // the price it can be sold for
        [InspectorName("Efficiency"), Tooltip("Amount of input needed (baseEfficiency)")]
        public float baseEfficiency; // controls how much resources are consumed per product produced
        public ResourceType inputResource; // the resource type that is consumed to produce this product

        [Header("Produkt Werte für die Consumption")]
        public float saturation;
        public int credits; // how much credits you get for selling one unit of this product

        [Header("Upgrade Limits")]
        public int maxUpgradesSpeed;
        public int maxUpgradesAmount;
        public int maxUpgradesQuality;
        public int maxUpgradesEfficiency;

    }

    public ProductDescriptionEntry[] entries;
    private Dictionary<ProductType, ProductDescriptionEntry> entryDict;


    public void Initialize()
    {
        entryDict = new Dictionary<ProductType, ProductDescriptionEntry>();
        foreach (var entry in entries)
        {
            if (!entryDict.ContainsKey(entry.productType))
            {
                entryDict[entry.productType] = entry;
            }
            else
            {
                Debug.LogWarning($"Duplicate product type in ProductDescriptionDatabase: {entry.productType}");
            }
        }
    }

    public ProductDescriptionEntry GetEntry(ProductType type)
    {
        if (entryDict == null)
            Initialize();

        if (entryDict.TryGetValue(type, out var entry))
            return entry;

        Debug.LogWarning($"Product type not found in database: {type}");
        return null;
    }
}
