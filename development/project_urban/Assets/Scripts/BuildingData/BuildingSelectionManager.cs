using UnityEngine;
using UnityEngine.UIElements;

public class BuildingSelectionManager : MonoBehaviour
{
    public UIDocument uiDocument;

    private VisualElement previewElement;
    public RenderTexture previewTexture; // vom Inspector zugewiesen

    private Label nameLabel, descriptionLabel, levelLabel, productionLabel;
    private Button upgradeButton, algenButton, quallenButton, salzpflanzenButton, grillenButton;
    private VisualElement panel;

    private BuildingInstance selected;

    void Start()
    {
        var root = uiDocument.rootVisualElement;

        nameLabel = root.Q<Label>("BuildingName");
        levelLabel = root.Q<Label>("BuildingLevel");
        productionLabel = root.Q<Label>("BuildingProduction");
        upgradeButton = root.Q<Button>("UpgradeButton");

        algenButton = root.Q<Button>("Alge");
        quallenButton = root.Q<Button>("Qualle");
        salzpflanzenButton = root.Q<Button>("Salzpflanze");
        grillenButton = root.Q<Button>("Grille");

        panel = root.Q<VisualElement>("BuildingPanel");

        algenButton.clicked += () =>
        {
            if (selected != null)
            {
                selected.UpgradeCompartmentAlge();
                UpdateUI(selected);
                SelectBuilding(selected); // Aktualisiert die UI mit dem neuen Level
            }
        };

        quallenButton.clicked += () =>
        {
            if (selected != null)
            {
                selected.UpgradeCompartmentQualle();
                UpdateUI(selected);
                SelectBuilding(selected); // Aktualisiert die UI mit dem neuen Level
            }
        };

        salzpflanzenButton.clicked += () =>
        {
            if (selected != null)
            {
                selected.UpgradeCompartmentSalzpflanze();
                UpdateUI(selected);
                SelectBuilding(selected); // Aktualisiert die UI mit dem neuen Level
            }
        };

        grillenButton.clicked += () =>
        {
            if (selected != null)
            {
                selected.UpgradeCompartmentGrille();
                UpdateUI(selected);
                SelectBuilding(selected); // Aktualisiert die UI mit dem neuen Level
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
            levelLabel.text = "Compartments " + selected.countCompartmentsHouse;
            productionLabel.text = "Max Comp " + selected.maxCompartments;

            if (selected.compartmentTypeHouse == 3)
            {
                nameLabel.text = "Algen Produktion";

                productionLabel.text += "\nAlge/s: " + selected.producePerSecond;
                algenButton.SetEnabled(true);
                quallenButton.SetEnabled(false);
                salzpflanzenButton.SetEnabled(false);
                grillenButton.SetEnabled(false);
            }
            else if (building.compartmentTypeHouse == 4)
            {
                nameLabel.text = "Salzpflanzen Produktion";
                productionLabel.text += "\nSalzpflanze/s: " + selected.producePerSecond;
                algenButton.SetEnabled(false);
                quallenButton.SetEnabled(false);
                salzpflanzenButton.SetEnabled(true);
                grillenButton.SetEnabled(false);
            }
            else if (building.compartmentTypeHouse == 5)
            {
                nameLabel.text = "Quallen Produktion";
                productionLabel.text += "\nQualle/s: " + selected.producePerSecond;
                algenButton.SetEnabled(false);
                quallenButton.SetEnabled(true);
                salzpflanzenButton.SetEnabled(false);
                grillenButton.SetEnabled(false);
            }
            else if (building.compartmentTypeHouse == 6)
            {
                nameLabel.text = "Grillen Produktion";
                productionLabel.text += "\nGrille/s: " + selected.producePerSecond;
                algenButton.SetEnabled(false);
                quallenButton.SetEnabled(false);
                salzpflanzenButton.SetEnabled(false);
                grillenButton.SetEnabled(true);
            }
            else
            {
                nameLabel.text = "Hochhaus";
                productionLabel.text = "Produktion: ";
                algenButton.SetEnabled(true);
                quallenButton.SetEnabled(true);
                salzpflanzenButton.SetEnabled(true);
                grillenButton.SetEnabled(true);
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
}
