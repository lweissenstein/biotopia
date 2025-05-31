using UnityEngine;

public class UpgradeableObject : MonoBehaviour
{
    private void Awake()
    {
        SetActiveChild(0);
    }

    public void UpgradeTo(int newID)
    {
        SetActiveChild(newID);
    }

    private void SetActiveChild(int activeID)
    {
        for (int i = 0; i < 3; i++)
        {
            Transform child = transform.Find(i.ToString());
            if (child != null)
                child.gameObject.SetActive(i == activeID);
        }
    }
}
