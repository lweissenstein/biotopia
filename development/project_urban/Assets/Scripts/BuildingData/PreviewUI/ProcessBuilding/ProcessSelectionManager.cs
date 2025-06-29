using UnityEngine;
using UnityEngine.UIElements;
using EconomySystem;
using System.Collections.Generic;

public class ProcessSelectionManager : MonoBehaviour
{
    public UIDocument uiDocument;

    private Label cntEfficiency, cntSpeed, cntAmount, enablerStatus, nameLabel;
    private Button upgradeSpeedButton, upgradeAmountButton, upgradeQualityButton, upgradeEfficiencyButton, enablerButton;
    private VisualElement panel;
    private Tab tabButton;

    private ProcessInstance selected;
    [SerializeField] private ProductDescriptionDatabase productDatabase;
    private ProductType selectedType;

    private Label descriptionLabel;
    private Button closer;
    private VisualElement upgradePanel;

    private HashSet<ProductType> activatedProducts = new HashSet<ProductType>();

    private Dictionary<ProductType, bool> productActiveStates = new();

    // Dictionary für Buttons pro Produkt
    private Dictionary<ProductType, Button> productButtons = new();

    private static readonly Color SoftGreen = new Color(0.2f, 0.5f, 0.2f);  // dunkles, sanftes Grün
    private static readonly Color SoftRed = new Color(0.6f, 0.2f, 0.2f); // sanftes rot


    private static readonly Dictionary<ProductType, ProductType?> productChainNext = new()
{
    // Alge-Kette
    { ProductType.AlgePowder, ProductType.AlgeNoodle },
    { ProductType.AlgeNoodle, ProductType.AlgeJelly },
    { ProductType.AlgeJelly, ProductType.AlgePatty },
    { ProductType.AlgePatty, null },
    // Salzpflanze-Kette
    { ProductType.SalzpflanzeSalt, ProductType.SalzpflanzePickles },
    { ProductType.SalzpflanzePickles, ProductType.SalzpflanzeSpread },
    { ProductType.SalzpflanzeSpread, ProductType.SalzpflanzeChips },
    { ProductType.SalzpflanzeChips, null },
    // Qualle-Kette
    { ProductType.QualleNoodle, ProductType.QualleMayo },
    { ProductType.QualleMayo, ProductType.QualleTofu },
    { ProductType.QualleTofu, ProductType.QualleBites },
    { ProductType.QualleBites, null },
    // Grille-Kette
    { ProductType.GrilleFlour, ProductType.GrilleLoaf },
    { ProductType.GrilleLoaf, ProductType.GrilleChips },
    { ProductType.GrilleChips, ProductType.GrilleBar },
    { ProductType.GrilleBar, null }
};
    void Awake()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        descriptionLabel = root.Q<Label>("description");
        upgradePanel = root.Q<VisualElement>("upgradePanel");
        closer = root.Q<Button>("closeWindow");
        upgradeAmountButton = root.Q<Button>("upgAmount");
        upgradeSpeedButton = root.Q<Button>("upgSpeed");
        upgradeEfficiencyButton = root.Q<Button>("upgEfficency");
        enablerButton = root.Q<Button>("enablerButton");
        cntAmount = root.Q<Label>("cntAmount");
        cntEfficiency = root.Q<Label>("cntEfficency");
        cntSpeed = root.Q<Label>("cntSpeed");
        enablerStatus = root.Q<Label>("enablerStatus");
        nameLabel = root.Q<Label>("name");
        var tabView = root.Q<TabView>("tabs");

        var allButtons = root.Query<Button>().Where(b => b.name.StartsWith("btn")).ToList();

        foreach (var btn in allButtons)
        {
            ProductType? type = btn.name switch
            {
                var n when n.Contains("AlgePowder") => ProductType.AlgePowder,
                var n when n.Contains("AlgeNoodle") => ProductType.AlgeNoodle,
                var n when n.Contains("AlgeJelly") => ProductType.AlgeJelly,
                var n when n.Contains("AlgePatty") => ProductType.AlgePatty,
                var n when n.Contains("SalzpflanzeSalt") => ProductType.SalzpflanzeSalt,
                var n when n.Contains("SalzpflanzePickles") => ProductType.SalzpflanzePickles,
                var n when n.Contains("SalzpflanzeSpread") => ProductType.SalzpflanzeSpread,
                var n when n.Contains("SalzpflanzeChips") => ProductType.SalzpflanzeChips,
                var n when n.Contains("QualleNoodle") => ProductType.QualleNoodle,
                var n when n.Contains("QualleMayo") => ProductType.QualleMayo,
                var n when n.Contains("QualleTofu") => ProductType.QualleTofu,
                var n when n.Contains("QualleBites") => ProductType.QualleBites,
                var n when n.Contains("GrilleFlour") => ProductType.GrilleFlour,
                var n when n.Contains("GrilleLoaf") => ProductType.GrilleLoaf,
                var n when n.Contains("GrilleChips") => ProductType.GrilleChips,
                var n when n.Contains("GrilleBar") => ProductType.GrilleBar,
                _ => (ProductType?)null
            };

            if (type.HasValue)
            {
                btn.userData = type.Value;
                productButtons[type.Value] = btn;
            }

            btn.clicked += () =>
            {
                if (btn.userData is ProductType productType)
                {
                    Debug.Log($"Button clicked for product: {productType}");
                    selectedType = productType;
                    ActivateProductLine(productType);
                    UpdateEnableButtonVisual();
                    UpdateUpgradeButtonVisual();
                }
            };
        }

        enablerButton.clicked += () =>
        {
            if (selected == null)
                return;

            bool isCurrentlyActive = productActiveStates.ContainsKey(selectedType) && productActiveStates[selectedType];
            productActiveStates[selectedType] = !isCurrentlyActive;
            selected.ToggleProducing(selectedType);

            enablerButton.style.backgroundColor = new StyleColor(isCurrentlyActive ? Color.red : Color.green);
            enablerStatus.text = isCurrentlyActive ? "Status: Deaktiviert" : "Status: Aktiviert";
            enablerStatus.style.color = isCurrentlyActive ? SoftRed : SoftGreen;
        };

        upgradeSpeedButton.clicked += () =>
        {
            if (selected != null)
                selected.Upgrade(selectedType, ProcessValue.Speed);
            cntSpeed.text = selected.GetUpgrade(selectedType, ProcessValue.Speed).ToString() + "/" + selected.GetMaxUpgrade(selectedType, ProcessValue.Speed);
        };

        upgradeAmountButton.clicked += () =>
        {
            if (selected != null)
                selected.Upgrade(selectedType, ProcessValue.Amount);
            cntAmount.text = selected.GetUpgrade(selectedType, ProcessValue.Amount).ToString() + "/" + selected.GetMaxUpgrade(selectedType, ProcessValue.Amount);
        };

        upgradeEfficiencyButton.clicked += () =>
        {
            if (selected != null)
                selected.Upgrade(selectedType, ProcessValue.Efficiency);
            cntEfficiency.text = selected.GetUpgrade(selectedType, ProcessValue.Efficiency).ToString() + "/" + selected.GetMaxUpgrade(selectedType, ProcessValue.Efficiency);
        };

        foreach (var kvp in productButtons)
        {
            var product = kvp.Key;
            var button = kvp.Value;

            if (product != ProductType.AlgePowder && product != ProductType.SalzpflanzeSalt && product != ProductType.GrilleFlour && product != ProductType.QualleNoodle)
            {
                button.SetEnabled(false);
            }
        }

        upgradePanel.style.display = DisplayStyle.None;

        closer.clicked += () =>
        {
            HideUI();
        };

        HideUI();
    }

    private void UpdateEnableButtonVisual()
    {
        if (productActiveStates.TryGetValue(selectedType, out bool isActive))
        {
            enablerButton.style.backgroundColor = new StyleColor(isActive ? Color.green : Color.red);
            enablerStatus.text = isActive ? "Status: Aktiviert" : "Status: Deaktiviert";
            enablerStatus.style.color = isActive ? SoftGreen : SoftRed;
        }
        else
        {
            // Standardfarbe, z.B. grün
            enablerButton.style.backgroundColor = new StyleColor(Color.green);
            enablerStatus.text = "Status: Aktiviert";
            enablerStatus.style.color = SoftGreen;
        }
    }

    void ClosePanel()
    {
        upgradePanel.style.display = DisplayStyle.None;
    }
    
    void UpdateUpgradeButtonVisual()
    {
        if (selected == null)
            return;

        cntSpeed.text = selected.GetUpgrade(selectedType, ProcessValue.Speed).ToString() + "/" +
                        selected.GetMaxUpgrade(selectedType, ProcessValue.Speed);

        cntAmount.text = selected.GetUpgrade(selectedType, ProcessValue.Amount).ToString() + "/" +
                         selected.GetMaxUpgrade(selectedType, ProcessValue.Amount);

        cntEfficiency.text = selected.GetUpgrade(selectedType, ProcessValue.Efficiency).ToString() + "/" +
                             selected.GetMaxUpgrade(selectedType, ProcessValue.Efficiency);
    }

    //private void ActivateProductLine(ProductType productType)
    //{
    //    if (!productActiveStates.ContainsKey(productType))
    //    {
    //        productActiveStates[productType] = true; // standardmäßig aktiv
    //    }

    //    if (!activatedProducts.Contains(productType))
    //    {
    //        selected.SetProductActive(productType, true);
    //        switch (productType)
    //        {
    //            case ProductType.AlgePowder:
    //                if (productButtons.TryGetValue(ProductType.AlgeNoodle, out var btnAlgeNoodle) && productButtons.TryGetValue(ProductType.AlgePowder, out var btnAlgePowder))
    //                {
    //                    btnAlgePowder.style.backgroundImage = null;
    //                    btnAlgeNoodle.SetEnabled(true);
    //                }
    //                break;
    //            case ProductType.AlgeNoodle:
    //                if (productButtons.TryGetValue(ProductType.AlgeJelly, out var btnAlgeJelly) && productButtons.TryGetValue(ProductType.AlgeNoodle, out btnAlgeNoodle))
    //                {
    //                    btnAlgeNoodle.style.backgroundImage = null;
    //                    btnAlgeJelly.SetEnabled(true);
    //                }
    //                break;
    //            case ProductType.AlgeJelly:
    //                if (productButtons.TryGetValue(ProductType.AlgePatty, out var btnAlgePatty) && productButtons.TryGetValue(ProductType.AlgeJelly, out btnAlgeJelly))
    //                {
    //                    btnAlgeJelly.style.backgroundImage = null;
    //                    btnAlgePatty.SetEnabled(true);
    //                }
    //                break;
    //            case ProductType.AlgePatty:
    //                if (productButtons.TryGetValue(ProductType.AlgePatty, out btnAlgePatty))
    //                {
    //                    btnAlgePatty.style.backgroundImage = null; // AlgeOil ist das letzte Produkt, daher kein weiterer Button
    //                }

    //                break;
    //        }
    //        activatedProducts.Add(productType);
    //    }
    //    if(upgradePanel.style.display == DisplayStyle.None)
    //    {
    //        upgradePanel.style.display = DisplayStyle.Flex;
    //    }
    //    descriptionLabel.text = productDatabase.GetEntry(productType).description;
    //    nameLabel.text = productDatabase.GetEntry(productType).name;
    //}

    private bool checkPrice(ProductType productType)
    {
        throw new System.NotImplementedException();
    }

    public void SelectBuilding(ProcessInstance building)
    {
        selected = building;
        UpdateUI(selected);
        ShowUI();
    }

    void UpdateUI(ProcessInstance building)
    {
        // Hier kann z.B. auf productButtons zugegriffen werden
        // Beispiel: productButtons[ProductType.AlgeDry].SetEnabled(false);
    }

    public void Deselect()
    {
        HideUI();
    }



    private void ActivateProductLine(ProductType productType)
    {
        if (!productActiveStates.ContainsKey(productType))
        {
            productActiveStates[productType] = true; // standardmäßig aktiv
        }

        if (!activatedProducts.Contains(productType))
        {
            selected.SetProductActive(productType, true);

            // Aktuellen Button "freischalten" (Bild entfernen)
            if (productButtons.TryGetValue(productType, out var currentBtn))
            {
                currentBtn.style.backgroundImage = null;
            }

            // Nächsten Button aktivieren, falls vorhanden
            if (productChainNext.TryGetValue(productType, out var nextType) && nextType.HasValue)
            {
                if (productButtons.TryGetValue(nextType.Value, out var nextBtn))
                {
                    nextBtn.SetEnabled(true);
                }
            }

            activatedProducts.Add(productType);
        }
        if (upgradePanel.style.display == DisplayStyle.None)
        {
            upgradePanel.style.display = DisplayStyle.Flex;
        }
        descriptionLabel.text = productDatabase.GetEntry(productType).description;
        nameLabel.text = productDatabase.GetEntry(productType).name;
    }


    void HideUI() => uiDocument.rootVisualElement.style.display = DisplayStyle.None;
    void ShowUI() => uiDocument.rootVisualElement.style.display = DisplayStyle.Flex;
}



  
