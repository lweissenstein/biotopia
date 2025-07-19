using System;
using UnityEngine;

namespace EconomySystem
{
public class CreditSystem : MonoBehaviour
{
    public static CreditSystem Instance { get; private set; }

    public int currentCredits { get; private set; } = 10000;

    public event Action<int> OnCreditsChanged;
    

    // Singleton pattern to ensure only one instance of CreditSystem exists
    private void Awake()  
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }

    }

    // add credits to the current credits
    public void AddCredits(int amount)
    {
        currentCredits += amount;
        OnCreditsChanged?.Invoke(currentCredits);
    }

    // remove credits from the current credits if possible
    public bool TrySpendCredits(int amount)
    {
        if (amount <= currentCredits)
        {
            currentCredits -= amount;
            OnCreditsChanged?.Invoke(currentCredits);
            return true;
        }
        return false;
    }

    // check if the current credits are enough to spend the given amount
    public bool HasEnoughCredits(int amount)
    {
        return amount <= currentCredits;
    }
}

}
