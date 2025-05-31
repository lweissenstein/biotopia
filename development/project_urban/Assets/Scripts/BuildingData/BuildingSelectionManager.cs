using UnityEngine;
using UnityEngine.UIElements;

public class BuildingSelectionManager : MonoBehaviour
{
    public UIDocument uiDocument;

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
        nameLabel.text = selected.data.buildingName;
        levelLabel.text = "Level: " + selected.level;
        productionLabel.text = "Production: " + selected.GetProduction();

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
        HideUI();
    }

    void HideUI() => uiDocument.rootVisualElement.style.display = DisplayStyle.None;
    void ShowUI() => uiDocument.rootVisualElement.style.display = DisplayStyle.Flex;
}
