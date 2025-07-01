using UnityEngine;
using UnityEngine.UIElements;
using EconomySystem;

public class CreditUISystem : MonoBehaviour
{


    public UIDocument ui;
    private Label creditLabel;

    private void Start()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        creditLabel = root.Q<Label>("credits");

        CreditSystem.Instance.OnCreditsChanged += UpdateCreditDisplay;
        UpdateCreditDisplay(CreditSystem.Instance.currentCredits);
    }

    private void UpdateCreditDisplay(int currentCredits)
    {
        if (creditLabel != null)
        {
            creditLabel.text = $"Credits: {currentCredits}";
        }
        else
        {
            Debug.LogWarning("Credit label not found in the UI document.");
        }
    }
}
