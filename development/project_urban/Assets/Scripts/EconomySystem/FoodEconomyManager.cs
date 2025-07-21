using EconomySystem;
using UnityEngine;

public class FoodEconomyManager : MonoBehaviour
{
    private FoodEconomy foodEconomy;
    [SerializeField] private ProductDescriptionDatabase productDatabase;
    [SerializeField] private DiversityTracker diversityTracker;
    [SerializeField] private CreditSystem creditSystem;

    private void Start()
    {
        BuildingInstance.Consume += HandleConsumeProduct;
        foodEconomy = FoodEconomy.Instance;
        if (GameState.isTutorial)
        {
            Debug.Log("jop jop");
            foodEconomy.AddTutorial();
        }

    }

    private void HandleConsumeProduct(ProductType prod)
    {
        if (!foodEconomy.IsProductAvailable(prod, 1)) return;

        var desc = productDatabase.GetEntry(prod);
        if (desc == null) return;

        Debug.Log($"Consuming product: {desc.name} with saturation: {desc.saturation} and credits: {desc.credits}");

        foodEconomy.ConsumeProduct(prod, 1);
        foodEconomy.OnProduceFood(desc.saturation * diversityTracker.GetDiversityBonus(desc.inputResource));
        creditSystem.AddCredits(desc.credits);
        diversityTracker.Register(desc.inputResource);
    }
}
