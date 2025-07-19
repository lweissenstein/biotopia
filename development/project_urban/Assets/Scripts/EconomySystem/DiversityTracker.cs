using EconomySystem;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DiversityTracker : MonoBehaviour
{
    private Dictionary<ResourceType, int> consumed = new();

    public void Register(ResourceType type)
    {
        if(!consumed.ContainsKey(type))
        {
            consumed[type] = 0;
        }
        consumed[type]++;
    }

    public float GetDiversityBonus(ResourceType type)
    {
        if (consumed.Count < 2) return 0.6f; // Minimum diversity bonus

        int min = consumed.Values.Min();
        int max = consumed.Values.Max();
        if (max == 0) return 1.0f;

        float balance = (float)min / max;
        Debug.Log($"Diversity balance for {type}: {balance} (min: {min}, max: {max})");
        return Mathf.Lerp(0.7f, 1.2f, balance);
    }

    public void Reset()
    {
        consumed.Clear();
    }
}
