using System.Collections.Generic;
using EconomySystem;
using UnityEngine;
using UnityEngine.UIElements;

public class BuildingSelectionManager : MonoBehaviour
{
    public UIDocument uiDocument;

    private VisualElement previewElement;
    public RenderTexture previewTexture; // vom Inspector zugewiesen

    private Label nameLabel, descriptionLabel, levelLabel, productionLabel, hasSupermarktLabel, descriptionText, compartmentType, buildingResidentsLabel;
    private Button upgradeButton, algenButton, quallenButton, salzpflanzenButton, grillenButton, supermarktButton;
    private VisualElement panel;

    private BuildingInstance selected;
    private List<BuildingInstance> superMarkets = new List<BuildingInstance>();

    public bool SuperMarketRange = false;

    public CreditSystem credits;

    void Start()
    {
        var root = uiDocument.rootVisualElement;

        credits = FindFirstObjectByType<CreditSystem>();

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

        algenButton.clicked += () =>
        {
            if (selected != null)
            {
                if (selected.countCompartmentsHouse < selected.maxCompartments && credits.TrySpendCredits(50))
                {
                    selected.UpgradeCompartmentAlge();
                    UpdateUI(selected);
                    SelectBuilding(selected); // Aktualisiert die UI mit dem neuen Level

                }
            }
        };

        quallenButton.clicked += () =>
        {
            if (selected != null)
            {
                if (selected.countCompartmentsHouse < selected.maxCompartments && credits.TrySpendCredits(50))
                {
                    selected.UpgradeCompartmentQualle();
                    UpdateUI(selected);
                    SelectBuilding(selected); // Aktualisiert die UI mit dem neuen Level
                } 
            }
        };

        salzpflanzenButton.clicked += () =>
        {
            if (selected != null)
            {
                if (selected.countCompartmentsHouse < selected.maxCompartments && credits.TrySpendCredits(50))
                {
                    selected.UpgradeCompartmentSalzpflanze();
                    UpdateUI(selected);
                    SelectBuilding(selected); // Aktualisiert die UI mit dem neuen Level 
                }
            }
        };

        grillenButton.clicked += () =>
        {
            if (selected != null)
            {
                if (selected.countCompartmentsHouse < selected.maxCompartments && credits.TrySpendCredits(50))
                {
                    selected.UpgradeCompartmentGrille();
                    UpdateUI(selected);
                    SelectBuilding(selected); // Aktualisiert die UI mit dem neuen Level 
                }
            }
        };

        supermarktButton.clicked += () =>
        {
            if (selected != null)
            {
                if (selected.countCompartmentsHouse < selected.maxCompartments && credits.TrySpendCredits(100))
                {
                    selected.UpgradeSupermarkt();
                    UpdateUI(selected);
                    SelectBuilding(selected); // Aktualisiert die UI mit dem neuen Level
                    if (!superMarkets.Contains(selected))
                        superMarkets.Add(selected);
                }
            }
        };

        HideUI();
    }

    public void SelectBuilding(BuildingInstance building)
    {
        selected = building;
        UpdateUI(selected);
        ShowUI();
    }

    void UpdateUI(BuildingInstance building)
    {
        // Gut kombinierbar mit "if" statements, um verschiedene ELemente zu aktivieren oder deaktivieren bei unterschiedlichen gebäuden
        //element.SetEnabled(false/true); - deaktiviert nur, ist aber noch ausgegraut existent
        //element.style.display = DisplayStyle.None; - blendet das element vollständig aus und gibt den platz frei
        //element.style.display = DisplayStyle.Flex; - blende das element wieder ein
        //element.style.visibility = Visibility.Hidden; - blendet aus, aber lässt den Platz bestehen - keine interaktion möglich
        //element.style.visibility = Visibility.Visible; - blendet wieder ein

        if (building.data is House house)
        {
            levelLabel.text = selected.countCompartmentsHouse +"/6";
            buildingResidentsLabel.text = selected.residents.ToString();

            if (selected.compartmentTypeHouse == 3)
            {
                nameLabel.text = "Algen Produktion";
                compartmentType.text = "Alge";
                productionLabel.text = selected.producePerSecond +"/s";
                descriptionText.text = "Vom Meeresgrund auf den Teller – Algen sind echte Nährstoffbomben. Reich an Eiweiß, Jod, Eisen und Omega-3. Sie wachsen schneller als jede Landpflanze und brauchen dafür nur Licht, CO2 und Meerwasser. Ob zart wie Meersalat oder fest wie Kelp: Algen sind das Gemüse der Zukunft. Nachhaltig, vielseitig und mit einer Prise Ozean in jedem Biss.";
                productionLabel.style.display = DisplayStyle.Flex;
                descriptionText.style.visibility = Visibility.Visible;
                compartmentType.style.visibility = Visibility.Visible;
                productionLabel.style.visibility = Visibility.Visible;
                algenButton.style.visibility = Visibility.Visible;
                quallenButton.style.visibility = Visibility.Hidden;
                salzpflanzenButton.style.visibility = Visibility.Hidden;
                grillenButton.style.visibility = Visibility.Hidden;
                supermarktButton.style.visibility = Visibility.Hidden;
                hasSupermarktLabel.text = (selected.hasSupermarket ? "Ja" : "Nein");
            }
            else if (building.compartmentTypeHouse == 4)
            {
                nameLabel.text = "Halophyten Produktion";
                compartmentType.text = "Halophyt";
                productionLabel.text = selected.producePerSecond + "/s";
                descriptionText.text = "Salzig, knackig, robust – Halophyten wie Salicornia wachsen dort, wo andere Pflanzen aufgeben: auf salzigen Böden, in trockenen Küstenregionen, ganz ohne Süßwasser. Diese grünen Überlebenskünstler bringen Mineralstoffe, Umami-Geschmack und zarte Sukkulenz auf den Teller. Sie sind die ideale Ressource für eine Welt mit Wassermangel – und schmecken dabei wie das Meer in Pflanzenform.";
                productionLabel.style.display = DisplayStyle.Flex;
                descriptionText.style.visibility = Visibility.Visible;
                compartmentType.style.visibility = Visibility.Visible;
                productionLabel.style.visibility = Visibility.Visible;
                algenButton.style.visibility = Visibility.Hidden;
                quallenButton.style.visibility = Visibility.Hidden;
                salzpflanzenButton.style.visibility = Visibility.Visible;
                grillenButton.style.visibility = Visibility.Hidden;
                supermarktButton.style.visibility = Visibility.Hidden;
                hasSupermarktLabel.text = (selected.hasSupermarket ? "Ja" : "Nein");
            }
            else if (building.compartmentTypeHouse == 5)
            {
                nameLabel.text = "Quallen Produktion";
                compartmentType.text = "Qualle";
                productionLabel.text = selected.producePerSecond + "/s";
                descriptionText.text = "Schwebend, transparent, fast außerirdisch – Quallen sind wahre Überlebenskünstler. Sie brauchen kaum Energie, vermehren sich rasant und bestehen zu 95% aus Wasser. Was viele nicht wissen: Sie sind auch essbar! Ihr hoher Kollagen- und Proteingehalt macht sie zur exotischen Proteinquelle der Zukunft. In der Küche? Ein Erlebnis zwischen Crunch und Glibber – für alle, die bereit sind, kulinarisches Neuland zu betreten.";
                productionLabel.style.display = DisplayStyle.Flex;
                descriptionText.style.visibility = Visibility.Visible;
                compartmentType.style.visibility = Visibility.Visible;
                productionLabel.style.visibility = Visibility.Visible;
                algenButton.style.visibility = Visibility.Hidden;
                quallenButton.style.visibility = Visibility.Visible;
                salzpflanzenButton.style.visibility = Visibility.Hidden;
                grillenButton.style.visibility = Visibility.Hidden;
                supermarktButton.style.visibility = Visibility.Hidden;
                hasSupermarktLabel.text = (selected.hasSupermarket ? "Ja" : "Nein");
            }
            else if (building.compartmentTypeHouse == 6)
            {
                nameLabel.text = "Grillen Produktion";
                compartmentType.text = "Grille";
                productionLabel.text = selected.producePerSecond + "/s";
                descriptionText.text = "Knusprig, nussig, unglaublich effizient – Grillen liefern hochwertiges Protein, Vitamine und gesunde Fette. Sie brauchen wenig Platz, kaum Wasser und stoßen kaum CO2 aus. Was sie leisten? Mehr als man denkt. Als Snack, Mehl oder Fleischersatz sind sie die leisen Superhelden der Ernährung. Klein, aber oho – Insekten sind nicht nur die Zukunft. Sie sind die Gegenwart.\r\n\r\n";
                productionLabel.style.display = DisplayStyle.Flex;
                descriptionText.style.visibility = Visibility.Visible;
                compartmentType.style.visibility = Visibility.Visible;
                productionLabel.style.visibility = Visibility.Visible;
                algenButton.style.visibility = Visibility.Hidden;
                quallenButton.style.visibility = Visibility.Hidden;
                salzpflanzenButton.style.visibility = Visibility.Hidden;
                grillenButton.style.visibility = Visibility.Visible;
                supermarktButton.style.visibility = Visibility.Hidden;
                hasSupermarktLabel.text = (selected.hasSupermarket ? "Ja" : "Nein");
            }
            else if (building.compartmentTypeHouse == 7)
            {
                nameLabel.text = "Supermarkt";
                descriptionText.style.visibility = Visibility.Hidden;
                compartmentType.style.visibility = Visibility.Hidden;
                productionLabel.style.visibility = Visibility.Hidden;
                algenButton.style.visibility = Visibility.Hidden;
                quallenButton.style.visibility = Visibility.Hidden;
                salzpflanzenButton.style.visibility = Visibility.Hidden;
                grillenButton.style.visibility = Visibility.Hidden;
                supermarktButton.style.visibility = Visibility.Visible;
            }
            else
            {
                nameLabel.text = "Hochhaus";
   
                descriptionText.style.visibility = Visibility.Hidden;
                compartmentType.style.visibility = Visibility.Hidden;
                productionLabel.style.visibility = Visibility.Hidden;
                algenButton.style.visibility = Visibility.Visible;
                quallenButton.style.visibility = Visibility.Visible;
                salzpflanzenButton.style.visibility = Visibility.Visible;
                grillenButton.style.visibility = Visibility.Visible;
                supermarktButton.style.visibility = Visibility.Visible;
                hasSupermarktLabel.text = (selected.hasSupermarket ? "Ja" : "Nein");
            }
        }

        else if (building.data is Water water)
        {
            nameLabel.style.visibility = Visibility.Hidden;
            levelLabel.text = "Level: " + selected.height;
            productionLabel.text = "Production: ";
        }

        if (building.previewPrefab != null)
        {
            var previewManager = FindFirstObjectByType<BuildingPreviewManager>();
            if (previewManager != null)
            {
                previewManager.ShowPreview(building.previewPrefab, building.height);
            }
        }

        //var level = building.height;
        //Color background = Color.gray;

        //if (level == 1)
        //    background = new Color(0.6f, 0.6f, 0.6f); // grau
        //else if (level == 2)
        //    background = new Color(1.0f, 0.8f, 0.2f); // gelblich
        //else
        //    background = new Color(0.4f, 1f, 0.4f); // grünlich
        // // Name im UXML
        //panel.style.backgroundColor = background;
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
}
