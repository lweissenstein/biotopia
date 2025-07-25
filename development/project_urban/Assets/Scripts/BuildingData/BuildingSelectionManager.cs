using System.Collections.Generic;
using EconomySystem;
using UnityEngine;
using System;
using UnityEngine.UIElements;

public class BuildingSelectionManager : MonoBehaviour
{
    public UIDocument uiDocument;

    private VisualElement previewElement;
    public RenderTexture previewTexture; // vom Inspector zugewiesen

    private Label nameLabel, descriptionLabel, levelLabel, productionLabel, hasSupermarktLabel, descriptionText, compartmentType, buildingResidentsLabel, algePrice, grillePrice, quallePrice, halophytPrice, exitButton, superMarketPriceLabel;
    private Button upgradeButton, algenButton, quallenButton, salzpflanzenButton, grillenButton, supermarktButton;
    private VisualElement panel;

    private TutorialManager tutorialManager;
    private BuildingInstance selected;
    public List<BuildingInstance> superMarkets = new();

    public bool SuperMarketRange = false;
    public int superMarketPrice = 50;

    private CreditSystem credits;

    void Start()
    {
        var root = uiDocument.rootVisualElement;

        credits = FindFirstObjectByType<CreditSystem>();
        tutorialManager = FindFirstObjectByType<TutorialManager>();

        nameLabel = root.Q<Label>("BuildingName");
        levelLabel = root.Q<Label>("BuildingLevel");
        productionLabel = root.Q<Label>("BuildingProduction");
        upgradeButton = root.Q<Button>("UpgradeButton");
        compartmentType = root.Q<Label>("compartmentType");
        algenButton = root.Q<Button>("Alge");
        quallenButton = root.Q<Button>("Qualle");
        salzpflanzenButton = root.Q<Button>("Salzpflanze");
        grillenButton = root.Q<Button>("Grille");
        supermarktButton = root.Q<Button>("Supermarkt");
        hasSupermarktLabel = root.Q<Label>("hasSupermarkt");
        descriptionText = root.Q<Label>("descriptionText");
        panel = root.Q<VisualElement>("BuildingPanel");
        buildingResidentsLabel = root.Q<Label>("BuildingResidents");
        algePrice = root.Q<Label>("algePrice");
        grillePrice = root.Q<Label>("grillePrice");
        quallePrice = root.Q<Label>("quallePrice");
        halophytPrice = root.Q<Label>("halophytPrice");
        exitButton = root.Q<Label>("exitButton");
        superMarketPriceLabel = root.Q<Label>("marketPrice");

        SetupCompartmentButton(algenButton, "Alge", () => selected.UpgradeCompartmentAlge());
        SetupCompartmentButton(quallenButton, "Qualle", () => selected.UpgradeCompartmentQualle());
        SetupCompartmentButton(salzpflanzenButton, "Salzpflanze", () => selected.UpgradeCompartmentSalzpflanze());
        SetupCompartmentButton(grillenButton, "Grille", () => selected.UpgradeCompartmentGrille());

       
        exitButton.RegisterCallback<ClickEvent>(evt => {
            Deselect();
        });

        supermarktButton.clicked += () =>
        {
            if (selected != null)
            { 
                if (selected.countCompartmentsHouse < selected.maxCompartments && credits.TrySpendCredits(superMarketPrice))
                {
                    selected.UpgradeSupermarkt();
                    UpdateUI(selected);
                    SelectBuilding(selected); // Aktualisiert die UI mit dem neuen Level
                    if (!superMarkets.Contains(selected))
                        superMarkets.Add(selected);
                    if (GameState.waitForButton)
                    {
                        tutorialManager.NextStep();
                        GameState.waitForButton = false; // Reset waitForClick after handling the click
                    }
                    if (SuperMarketRange)
                        ActivateSuperMarketRange();
                    superMarketPrice += 50;
                }
            }
        };

        HideUI();
    }

    private void SetupCompartmentButton(Button button, string type, Action upgradeAction)
    {
        button.clicked += () =>
        {
            if (selected == null || selected.countCompartmentsHouse >= selected.maxCompartments)
                return;
            Debug.Log(GameState.waitForButton);
            if (GameState.waitForButton)
            {
                Debug.Log("Klick im gamestate");
                tutorialManager.NextStep();
                GameState.waitForButton = false; // Reset waitForClick after handling the click
            }
            int price = selected.compartmentPrices[type];

            if (credits.TrySpendCredits(price))
            {
                upgradeAction.Invoke();
                selected.compartmentPrices[type] -= 5;
                UpdateUI(selected);
                SelectBuilding(selected);

                // Preis erh�hen nur nach erfolgreichem Upgrade

            }
        };
    }

    public void SelectBuilding(BuildingInstance building)
    {
        selected = building;
        UpdateUI(selected);
        ShowUI();
    }

    void UpdateUI(BuildingInstance building)
    {
        if (building.data is House house)
        {
            algePrice.text = selected.countCompartmentsHouse >= selected.maxCompartments ? "MAX" : $"€ {selected.compartmentPrices["Alge"]}";
            quallePrice.text = selected.countCompartmentsHouse >= selected.maxCompartments ? "MAX" : $"€ {selected.compartmentPrices["Qualle"]}";
            halophytPrice.text = selected.countCompartmentsHouse >= selected.maxCompartments ? "MAX" : $"€ {selected.compartmentPrices["Salzpflanze"]}";
            grillePrice.text = selected.countCompartmentsHouse >= selected.maxCompartments ? "MAX" : $"€ {selected.compartmentPrices["Grille"]}";
            superMarketPriceLabel.text = selected.countCompartmentsHouse >= selected.maxCompartments ? "-" : $"€ {superMarketPrice}";

            buildingResidentsLabel.text = selected.residents.ToString();
            hasSupermarktLabel.text = selected.hasSupermarket ? "Ja" : "Nein";

            HideAllProductionButtons();
            ShowStandardFields();

            switch (building.compartmentTypeHouse)
            {
                case 3:
                    SetProductionUI(
                        "Makroalgen Produktion", "Makroalgen", selected.producePerSecond + "/s", selected.hasSupermarket,
                        "Makroalgen wachsen rasant, brauchen kein Süßwasser oder Ackerland und sind reich an Proteinen, Antioxidantien und weiteren wichtigen Nährstoffen. In regionalem Sole-Wasser kultiviert, sind sie ein nachhaltiges Lebensmittel der Zukunft und vielseitig einsetzbar. Zentral um die Bevölkerung von Biotopia zu ernähren." 
                    );
                    algenButton.style.visibility = Visibility.Visible;
                    //ToggleSuperMarketRange(false);
                    break;

                case 4:
                    SetProductionUI(
                        "Halophyten Produktion", "Halophyten", selected.producePerSecond + "/s", selected.hasSupermarket,
                        "Halophyten, auch Salzpflanzen genannt, wachsen dort, wo andere Pflanzen versagen: auf salzigen Böden, ganz ohne Süßwasser. Sie sind ideal für eine nachhaltige, urbane Gemüseproduktion der Zukunft und liefern wertvolle Nährstoffe mit einem Hauch von Umami."
                    );
                    salzpflanzenButton.style.visibility = Visibility.Visible;
                    //ToggleSuperMarketRange(false);
                    break;

                case 5:
                    SetProductionUI(
                        "Quallen Produktion", "Quallen", selected.producePerSecond + "/s", selected.hasSupermarket,
                        "Quallen sind reich an Proteinen, Omega‑3‑Fettsäuren und Vitaminen, ähnlich wie andere Meeresfrüchte. Mangrovenquallen, die in Biotopia kultiviert werden, besitzen eine außergewöhnliche Eigenschaft: Durch ihre Algensymbiose produzieren sie tierisches Protein mithilfe von Photosynthese direkt aus Sonnenlicht."
                    );
                    quallenButton.style.visibility = Visibility.Visible;
                    //ToggleSuperMarketRange(false);
                    break;

                case 6:
                    SetProductionUI(
                        "Grillen Produktion", "Grillen", selected.producePerSecond + "/s", selected.hasSupermarket,
                        "Grillen liefern hochwertiges Protein, gesunde Fettsäuren und wichtige Mikronährstoffe. Im Vergleich zur konventionellen Tierhaltung benötigen sie nur einen Bruchteil an Wasser, Fläche und Futter. Ihre ressourcenschonende Kultivierung macht sie zu effizienten Alternative nachhaltiger Ernährungssysteme der Zukunft."
                    );
                    grillenButton.style.visibility = Visibility.Visible;
                    //ToggleSuperMarketRange(false);
                    break;

                case 7: // Supermarkt
                    nameLabel.text = "Supermarkt";
                    descriptionText.text = "Hier kaufen die Bewohner von Biotopia die aus den alternativen Nahrungsquellen entstandenen Produkte und du erhältst Sättigung und Credits dafür.";
                    HideStandardFields();
                    supermarktButton.style.visibility = Visibility.Visible;
                    //ToggleSuperMarketRange(true);
                    break;

                case 8:
                    nameLabel.text = "Therme";
                    descriptionText.text = "Die Therme liefert lokal verfügbares Salzwasser an die Kompartimente, um eine nachhaltige und süßwasserschonende Nahrungsmittelproduktion zu ermöglichen.\r\n";
                    HideStandardFields();
                    ShowAllProductionButtons();
                    supermarktButton.style.visibility = Visibility.Hidden;
                    //ToggleSuperMarketRange(true);
                    break;

                default: // Hochhaus
                    nameLabel.text = "Hochhaus";
                    HideStandardFields();
                    ShowAllProductionButtons();
                    if (building.height > 1)
                    {
                        supermarktButton.style.visibility = Visibility.Hidden;
                    }
                    //ToggleSuperMarketRange(false);
                    break;
            }
        }

        if (building.previewPrefab != null)
        {
            var previewManager = FindFirstObjectByType<BuildingPreviewManager>();
            if (previewManager != null)
                previewManager.ShowPreview(building.previewPrefab, building.height);
        }
    }

    void SetProductionUI(string title, string type, string production, bool hasSuper, string description)
    {
        nameLabel.text = title;
        compartmentType.text = type;
        productionLabel.text = production;
        descriptionText.text = description;
        hasSupermarktLabel.text = hasSuper ? "Ja" : "Nein";
        levelLabel.text = selected.countCompartmentsHouse + "/" + selected.maxCompartments;

        descriptionText.style.visibility = Visibility.Visible;
        compartmentType.style.visibility = Visibility.Visible;
        productionLabel.style.visibility = Visibility.Visible;
        levelLabel.style.visibility = Visibility.Visible;
    }

    void HideStandardFields()
    {

        productionLabel.style.visibility = Visibility.Hidden;
        descriptionText.style.visibility = Visibility.Hidden;
        compartmentType.style.visibility = Visibility.Hidden;
        levelLabel.style.visibility = Visibility.Hidden;
    }

    void ShowStandardFields()
    {
 
        productionLabel.style.visibility = Visibility.Visible;
        descriptionText.style.visibility = Visibility.Visible;
        compartmentType.style.visibility = Visibility.Visible;
        levelLabel.style.visibility = Visibility.Visible;
    }

    void HideAllProductionButtons()
    {
        algenButton.style.visibility = Visibility.Hidden;
        quallenButton.style.visibility = Visibility.Hidden;
        salzpflanzenButton.style.visibility = Visibility.Hidden;
        grillenButton.style.visibility = Visibility.Hidden;
        supermarktButton.style.visibility = Visibility.Hidden;
    }

    void ShowAllProductionButtons()
    {
        if(GameState.isTutorial)
        {
            if (GameState.alge)
            {
                algenButton.style.visibility = Visibility.Visible;
            }
            if (GameState.supermarkt)
            {
                supermarktButton.style.visibility = Visibility.Visible;
            }
        } else  {
            algenButton.style.visibility = Visibility.Visible;
            quallenButton.style.visibility = Visibility.Visible;
            salzpflanzenButton.style.visibility = Visibility.Visible;
            grillenButton.style.visibility = Visibility.Visible;
            supermarktButton.style.visibility = Visibility.Visible;
        }
    }



    public void Deselect()
    {

        var previewManager = FindFirstObjectByType<BuildingPreviewManager>();
        previewManager?.ClearPreview();
        HideUI();
    }

    void HideUI() => uiDocument.rootVisualElement.style.display = DisplayStyle.None;
    void ShowUI() => uiDocument.rootVisualElement.style.display = DisplayStyle.Flex;

    public void ToggleSuperMarketRange()
    {
        SuperMarketRange = !SuperMarketRange;
        foreach (BuildingInstance supMar in superMarkets) 
        {
            Transform Indicator = supMar.transform.Find("8");
            if (Indicator != null)
                Indicator.gameObject.SetActive(SuperMarketRange);
        }
    }

    public void ActivateSuperMarketRange()
    {
        foreach (BuildingInstance supMar in superMarkets)
        {
            Transform Indicator = supMar.transform.Find("8");
            if (Indicator != null)
                Indicator.gameObject.SetActive(true);
        }
    }
}
