using System.Collections.Generic;
using UnityEngine;
using EconomySystem;

public class ProductDescriptionManager : MonoBehaviour
{
    [SerializeField] private ProductDescriptionDatabase descriptionDatabase;

    private Dictionary<ProductType, string> descriptionLookup;

    private void Awake()
    {
        descriptionLookup = new Dictionary<ProductType, string>();
        foreach (var entry in descriptionDatabase.entries)
        {
            if (!descriptionLookup.ContainsKey(entry.productType))
            {
                descriptionLookup.Add(entry.productType, entry.description);
            }
        }
    }

    public string GetDescription(ProductType productType)
    {
        if (descriptionLookup.TryGetValue(productType, out string desc))
        {
            return desc;
        }
        return "Keine Beschreibung vorhanden.";
    }
}
