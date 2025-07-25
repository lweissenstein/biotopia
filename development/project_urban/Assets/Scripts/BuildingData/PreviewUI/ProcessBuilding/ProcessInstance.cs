using System;
using System.Collections.Generic;
using UnityEngine;
using EconomySystem;

public class ProcessInstance : MonoBehaviour
{

    private FoodEconomy foodEconomy;
    public static event Action<ProductType, int, ResourceType, float> ProduceProduct;

    // Aktivierungsstatus je Produkt
    private Dictionary<ProductType, bool> productActive = new();

    private Dictionary<ProductType, bool> productProducing = new();

    // Timer je Produkt
    private Dictionary<ProductType, float> productionTimers = new();

    // Upgrades je Produkt & Wert
    private Dictionary<(ProductType, ProcessValue), int> upgradeValues = new();

    [SerializeField] private ProductDescriptionDatabase productDatabase;

    //can i merge pls
    private void Start()
    {
        // Alle Timer initialisieren
        foreach (var pt in Enum.GetValues(typeof(ProductType)))
        {
            productActive[(ProductType)pt] = false;
            productionTimers[(ProductType)pt] = 0f;
        }
    }

    private void FixedUpdate()
    {
        float delta = Time.fixedDeltaTime;

        foreach (var kvp in productActive)
        {
            var product = kvp.Key;
            if (!kvp.Value) continue;

            productionTimers[product] -= delta;

            if (productionTimers[product] <= 0f )
            {
                TryProduce(product);
                productionTimers[product] = GetCooldown(product);
            }
        }
    }

    private void TryProduce(ProductType product)
    {
        var resource = productDatabase.GetEntry(product).inputResource;

        int amount = CalculateAmount(product);
        int inputNeeded = CalculateInputNeeded(product);

        if (!HasEnoughInput(resource, inputNeeded)) return;

        ConsumeInput(resource, inputNeeded);

        ProduceProduct?.Invoke(product, amount, resource, inputNeeded);
    }

    private int CalculateAmount(ProductType product)
    {
        return 4 + GetUpgrade(product, ProcessValue.Amount);
    }

    private int CalculateEfficiency(ProductType product)
    {
        return 0 + GetUpgrade(product, ProcessValue.Efficiency);
    }

    private int CalculateInputNeeded(ProductType product)
    {
        int amount = CalculateAmount(product);
        int efficiency = CalculateEfficiency(product);
        return Mathf.CeilToInt((float)amount / (1f + 0.25f * efficiency));
    }

    private float GetCooldown(ProductType product)
    {
        var entry = productDatabase.GetEntry(product);
        float baseTime = entry.baseSpeed;
        int speed = GetUpgrade(product, ProcessValue.Speed);
        return baseTime / (1f + 0.25f * speed); // 25 prozent pro Upgrade-Stufe
    }

    public int GetUpgrade(ProductType product, ProcessValue val)
    {
        return upgradeValues.TryGetValue((product, val), out var result) ? result : 0;
    }

    public void Upgrade(ProductType product, ProcessValue valueType)
    {
        var key = (product, valueType);
        if (!upgradeValues.ContainsKey(key))
            upgradeValues[key] = 0;

        if (upgradeValues[key] < GetMaxUpgrade(product, valueType))
        { 
            upgradeValues[key]++;
        }
        else
        {
            Debug.LogWarning($"Maximalwert für Upgrade {valueType} von Produkt {product} erreicht.");
        }
    }

    public void SetProductActive(ProductType product, bool active)
    {
        productActive[product] = active;
    }

    public void ToggleProducing(ProductType product)
    {
        if (productActive.TryGetValue(product, out bool isActive))
        {
            productActive[product] = !isActive;
            if (isActive)
            {
                productionTimers[product] = GetCooldown(product); // Reset timer on activation
            }
        }
        else
        {
            productActive[product] = true; // Aktivieren, wenn nicht vorhanden
        }
    }

    private bool HasEnoughInput(ResourceType type, int needed)
    {
        if (foodEconomy == null)
            foodEconomy = FoodEconomy.Instance;

        if (foodEconomy.totalResources.TryGetValue(type, out float available))
        {
            return available >= needed;
        }
        return false;
    }

    private void ConsumeInput(ResourceType type, int amount)
    {
        // Hier Input-Ressourcen abziehen
    }
    public int GetMaxUpgrade(ProductType product, ProcessValue val)
    {
        var entry = productDatabase.GetEntry(product);
        if (entry == null)
        {
            return 0;
        }

        return val switch
        {
            ProcessValue.Speed => entry.maxUpgradesSpeed,
            ProcessValue.Amount => entry.maxUpgradesAmount,
            ProcessValue.Quality => entry.maxUpgradesQuality,
            ProcessValue.Efficiency => entry.maxUpgradesEfficiency,
            _ => 0
        };

    }
}
