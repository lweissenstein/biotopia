using UnityEngine;
using UnityEngine.UIElements;

public class BuildingSelectionManager : MonoBehaviour
{
    public UIDocument uiDocument;

    private VisualElement previewElement;
    public RenderTexture previewTexture; // vom Inspector zugewiesen

    private Label nameLabel;
    private Label levelLabel;
    private Label productionLabel;
    private Button upgradeButton;

    private BuildingInstance selected;

    void Start()
    {
        var root = uiDocument.rootVisualElement;

        nameLabel = root.Q<Label>("BuildingName");
        levelLabel = root.Q<Label>("BuildingLevel");
        productionLabel = root.Q<Label>("BuildingProduction");
        upgradeButton = root.Q<Button>("UpgradeButton");

        upgradeButton.clicked += () =>
        {
            if (selected != null)
            {
                selected.Upgrade();
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

        nameLabel.text = selected.data.buildingName;
        levelLabel.text = "Level: " + selected.level;
        productionLabel.text = "Production: " + selected.GetConsumption();

        if (building.previewPrefab != null)
        {
            var previewManager = FindFirstObjectByType<BuildingPreviewManager>();
            if (previewManager != null)
            {
                previewManager.ShowPreview(building.previewPrefab, building.level);
            }
        }

        var level = building.level;
        Color background = Color.gray;

        if (level == 1)
            background = new Color(0.6f, 0.6f, 0.6f); // grau
        else if (level == 2)
            background = new Color(1.0f, 0.8f, 0.2f); // gelblich
        else
            background = new Color(0.4f, 1f, 0.4f); // grünlich

        var root = uiDocument.rootVisualElement;
        var panel = root.Q<VisualElement>("BuildingPanel"); // Name im UXML
        panel.style.backgroundColor = background;
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
