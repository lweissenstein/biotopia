using UnityEngine;
using UnityEngine.UIElements;
using EconomySystem;
using System.Collections.Generic;
using System;

public class ProcessSelectionManager : MonoBehaviour
{
    public UIDocument uiDocument;

    private Label cntEfficiency, cntSpeed, cntAmount, enablerStatus, nameLabel, enablerText, priceAmount, priceEfficiency, priceSpeed;
    private Button upgradeSpeedButton, upgradeAmountButton, upgradeQualityButton, upgradeEfficiencyButton, enablerButton;
    private VisualElement panel, enablerVisual, descBorder, nameBorder;
    private Tab tabButton;

    private ProcessInstance selected;
    [SerializeField] private ProductDescriptionDatabase productDatabase;
    private ProductType selectedType;

    private Label descriptionLabel;
    private Button closer;
    private VisualElement upgradePanel;

    private HashSet<ProductType> activatedProducts = new HashSet<ProductType>();

    private Dictionary<ProductType, bool> productActiveStates = new();

    public Dictionary<int, ProductType> products = new Dictionary<int, ProductType>();
    public Dictionary<int, bool> purchased = new Dictionary<int, bool>();
    public Dictionary<ProductType, int> productsMirrored = new Dictionary<ProductType, int>();

    // Key ist eine Kombination aus selected und ProcessValue
    private Dictionary<(ProductType, ProcessValue), int> upgradePrices = new Dictionary<(ProductType, ProcessValue), int>();


    // Dictionary fr Buttons pro Produkt
    private Dictionary<ProductType, Button> productButtons = new();

    // Enabler Colors
    private static readonly Color enablerRed = new Color(231f / 255f, 62f / 255f, 17f / 255f);   // #E73E11
    private static readonly Color enablerGreen = new Color(106f / 255f, 191f / 255f, 79f / 255f);  // #6ABF4F
    private static readonly Color enablerRedBorder = new Color(119f / 255f, 31f / 255f, 8f / 255f);    // #771F08
    private static readonly Color enablerGreenBorder = new Color(43f / 255f, 96f / 255f, 26f / 255f);    // #2B601A

    // Background Colors
    private static readonly Color backgroundAlge = new Color(40f / 255f, 99f / 255f, 78f / 255f);     // #28634E
    private static readonly Color backgroundAlgeBorder = new Color(59f / 255f, 112f / 255f, 67f / 255f);    // #3B7043
    private static readonly Color backgroundSalzpflanze = new Color(40f / 255f, 75f / 255f, 99f / 255f);     // #284B63
    private static readonly Color backgroundSalzpflanzeBorder = new Color(60f / 255f, 110f / 255f, 113f / 255f);   // #3C6E71
    private static readonly Color backgroundQualle = new Color(99f / 255f, 40f / 255f, 90f / 255f);     // #63285A
    private static readonly Color backgroundQualleBorder = new Color(112f / 255f, 59f / 255f, 106f / 255f);   // #703B6A
    private static readonly Color backgroundGrille = new Color(99f / 255f, 71f / 255f, 40f / 255f);     // #634728
    private static readonly Color backgroundGrilleBorder = new Color(112f / 255f, 83f / 255f, 59f / 255f);    // #70533B

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
        upgradeEfficiencyButton = root.Q<Button>("upgEfficiency");
        enablerButton = root.Q<Button>("enablerButton");
        enablerText = root.Q<Label>("enablerText");
        nameBorder = root.Q<VisualElement>("nameBorder");
        descBorder = root.Q<VisualElement>("descBorder");
        cntAmount = root.Q<Label>("cntAmount");
        cntEfficiency = root.Q<Label>("cntEfficiency");
        cntSpeed = root.Q<Label>("cntSpeed");
        priceAmount = root.Q<Label>("priceAmount");
        priceEfficiency = root.Q<Label>("priceEfficiency");
        priceSpeed = root.Q<Label>("priceSpeed");
        //enablerStatus = root.Q<Label>("enablerStatus");
        nameLabel = root.Q<Label>("name");
        enablerVisual = root.Q<VisualElement>("enablerVisual");
        var tabView = root.Q<TabView>("tabs");

        var allButtons = root.Query<Button>().Where(b => b.name.StartsWith("btn")).ToList();

        int i = 0;

        foreach (ProductType type in Enum.GetValues(typeof(ProductType)))
        {
            upgradePrices[(type, ProcessValue.Speed)] = 500;
            upgradePrices[(type, ProcessValue.Amount)] = 500;
            upgradePrices[(type, ProcessValue.Efficiency)] = 500;
        }

        foreach (ProductType productType in Enum.GetValues(typeof(ProductType)))
        {
            purchased[i] = false;
            products[i] = productType;
            productsMirrored[productType] = i;
            i++;
        }

        foreach (var btn in allButtons)
        {
            if (Enum.TryParse<ProductType>(btn.name.Replace("btn", ""), out var parsedType))
            {
                btn.userData = parsedType;
                productButtons[parsedType] = btn;

                var entry = productDatabase.GetEntry(parsedType);
                if (entry != null)
                {
                    var labelName = "price" + parsedType; 
                    var priceLabel = btn.Q<Label>(labelName);
                    if (priceLabel != null)
                    {
                        priceLabel.text = entry.price.ToString();
                    }
                }
            }

        btn.clicked += () =>
            {
                if (btn.userData is ProductType productType)
                {
                    var entry = productDatabase.GetEntry(productType);
                    if (entry == null)
                    {
                        return;
                    }

                    int price = entry.price;
                    bool isBought = false;

                    purchased.TryGetValue(productsMirrored[productType], out isBought);

                    if (!isBought && CreditSystem.Instance.TrySpendCredits(price))
                    {
                        purchased[productsMirrored[productType]] = true;
                        selectedType = productType;
                        ActivateProductLine(productType);
                        UpdateEnableButtonVisual();
                        UpdateUpgradeButtonVisual();
                        UpdateColors(entry.inputResource);

                        var labelName = "price" + productType;
                        Label child = btn.Q<Label>(labelName);
                        if (child != null)
                        {
                            child.style.visibility = Visibility.Hidden; // Preis-Label ausblenden
                        }
                    }
                    else if (isBought)
                    {
                        selectedType = productType;
                        ActivateProductLine(productType);
                        UpdateEnableButtonVisual();
                        UpdateUpgradeButtonVisual();
                        UpdateColors(entry.inputResource);
                    }
                    else
                    {
                        Debug.Log($"Nicht genug Credits fr {productType}. Preis: {price}");
                    }
                }
            };
        }

        upgradeSpeedButton.clicked += () => HandleUpgrade(ProcessValue.Speed);
        upgradeAmountButton.clicked += () => HandleUpgrade(ProcessValue.Amount);
        upgradeEfficiencyButton.clicked += () => HandleUpgrade(ProcessValue.Efficiency);

        enablerButton.clicked += () =>
        {
            if (selected == null)
                return;

            bool isCurrentlyActive = productActiveStates.ContainsKey(selectedType) && productActiveStates[selectedType];
            bool newState = !isCurrentlyActive;
            productActiveStates[selectedType] = !isCurrentlyActive;
            selected.ToggleProducing(selectedType);

            enablerText.text = newState ? "Deaktivieren" : "Aktivieren";
            enablerVisual.style.backgroundColor = new StyleColor(isCurrentlyActive ? enablerRed : enablerGreen);
            SetBorderColor(enablerVisual, isCurrentlyActive ? enablerRedBorder : enablerGreenBorder);

            // Update Product's activeVisual (e.g. activeAlgeNoodle)
            if (productButtons.TryGetValue(selectedType, out var productBtn))
            {
                string activeName = $"active{selectedType}";
                VisualElement activeVisual = productBtn.Q<VisualElement>(activeName);

                if (activeVisual != null)
                {
                    activeVisual.style.backgroundColor = new StyleColor(newState ? enablerGreen : enablerRed);
                    SetBorderColor(activeVisual, newState ? enablerGreenBorder : enablerRedBorder);
                }
            }
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

    private void UpdateColors(ResourceType resType)
    {
        switch (resType)
        {
            case ResourceType.Alge:
                upgradePanel.style.backgroundColor = new StyleColor(backgroundAlge);
                SetBorderColor(enablerButton, backgroundAlgeBorder);
                SetBorderColor(upgradeSpeedButton, backgroundAlgeBorder);
                SetBorderColor(upgradeAmountButton, backgroundAlgeBorder);
                SetBorderColor(upgradeEfficiencyButton, backgroundAlgeBorder);
                SetBorderColor(nameBorder, backgroundAlgeBorder);
                SetBorderColor(descBorder, backgroundAlgeBorder);
                break;
            case ResourceType.Salzpflanze:
                upgradePanel.style.backgroundColor = new StyleColor(backgroundSalzpflanze);
                SetBorderColor(enablerButton, backgroundSalzpflanzeBorder);
                SetBorderColor(upgradeSpeedButton, backgroundSalzpflanzeBorder);
                SetBorderColor(upgradeAmountButton, backgroundSalzpflanzeBorder);
                SetBorderColor(upgradeEfficiencyButton, backgroundSalzpflanzeBorder);
                SetBorderColor(nameBorder, backgroundSalzpflanzeBorder);
                SetBorderColor(descBorder, backgroundSalzpflanzeBorder);
                break;
            case ResourceType.Qualle:
                upgradePanel.style.backgroundColor = new StyleColor(backgroundQualle);
                SetBorderColor(enablerButton, backgroundQualleBorder);
                SetBorderColor(upgradeSpeedButton, backgroundQualleBorder);
                SetBorderColor(upgradeAmountButton, backgroundQualleBorder);
                SetBorderColor(upgradeEfficiencyButton, backgroundQualleBorder);
                SetBorderColor(nameBorder, backgroundQualleBorder);
                SetBorderColor(descBorder, backgroundQualleBorder);
                break;
            case ResourceType.Grille:
                upgradePanel.style.backgroundColor = new StyleColor(backgroundGrille);
                SetBorderColor(enablerButton, backgroundGrilleBorder);
                SetBorderColor(upgradeSpeedButton, backgroundGrilleBorder);
                SetBorderColor(upgradeAmountButton, backgroundGrilleBorder);
                SetBorderColor(upgradeEfficiencyButton, backgroundGrilleBorder);
                SetBorderColor(nameBorder, backgroundGrilleBorder);
                SetBorderColor(descBorder, backgroundGrilleBorder);
                break;
            default:
                Debug.LogWarning($"Unbekannter ResourceType: {resType}");
                break;

        }
    }

    private void UpdateEnableButtonVisual()
    {
        if (productActiveStates.TryGetValue(selectedType, out bool isActive))
        {
            enablerText.text = isActive ? "Deaktivieren" : "Aktivieren";
            enablerVisual.style.backgroundColor = new StyleColor(isActive ? enablerGreen : enablerRed);
            SetBorderColor(enablerVisual, isActive ? enablerGreenBorder : enablerRedBorder);
        }
        else
        {
            // Standardfarbe, z.B. grn
            enablerText.text = "Deaktivieren";
            enablerVisual.style.backgroundColor = new StyleColor(enablerGreen);
            SetBorderColor(enablerVisual, enablerGreenBorder);

        }
    }

    void HandleUpgrade(ProcessValue value)
    {
        if (selected == null) return;

        var key = (selectedType, value);
        if (!upgradePrices.TryGetValue(key, out int price))
        {
            Debug.LogWarning($"Kein Preis für {key} gefunden.");
            return;
        }

        if (CreditSystem.Instance.TrySpendCredits(price))
        {
            selected.Upgrade(selectedType, value);
            upgradePrices[key] = (price * 2) + 250;
            UpdateUpgradeButtonVisual();
        }
        else
        {
            Debug.Log($"Nicht genug Credits für {value} Upgrade");
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
        priceSpeed.text = "€ " + upgradePrices[(selectedType, ProcessValue.Speed)].ToString();

        cntAmount.text = selected.GetUpgrade(selectedType, ProcessValue.Amount).ToString() + "/" +
                         selected.GetMaxUpgrade(selectedType, ProcessValue.Amount);
        priceAmount.text = "€ " + upgradePrices[(selectedType, ProcessValue.Amount)].ToString();

        cntEfficiency.text = selected.GetUpgrade(selectedType, ProcessValue.Efficiency).ToString() + "/" +
                             selected.GetMaxUpgrade(selectedType, ProcessValue.Efficiency);
        priceEfficiency.text = "€ " + upgradePrices[(selectedType, ProcessValue.Efficiency)].ToString();
    }


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
        
    }

    public void Deselect()
    {
        HideUI();
    }



    private void ActivateProductLine(ProductType productType)
    {
        if (!productActiveStates.ContainsKey(productType))
        {
            productActiveStates[productType] = true; // standardmig aktiv
        }

        if (!activatedProducts.Contains(productType))
        {
            selected.SetProductActive(productType, true);

            // Aktuellen Button "freischalten" (Bild entfernen)
            if (productButtons.TryGetValue(productType, out var currentBtn))
            {
                var childElement = currentBtn.Q<VisualElement>("lock");
                childElement.style.backgroundImage = null;
            }

            // Nchsten Button aktivieren, falls vorhanden
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
        var entry = productDatabase.GetEntry(productType);
        descriptionLabel.text = entry.description;
        nameLabel.text = entry.name;
    }

    public void SetBorderColor(VisualElement element, Color color)
    {
        var styleColor = new StyleColor(color);
        element.style.borderTopColor = styleColor;
        element.style.borderRightColor = styleColor;
        element.style.borderBottomColor = styleColor;
        element.style.borderLeftColor = styleColor;
    }


    void HideUI() => uiDocument.rootVisualElement.style.display = DisplayStyle.None;
    void ShowUI() => uiDocument.rootVisualElement.style.display = DisplayStyle.Flex;
}




