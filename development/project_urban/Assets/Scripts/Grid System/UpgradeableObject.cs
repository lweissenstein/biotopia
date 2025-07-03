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

    public void ActivateChild (int ID)
    {
        ActivateAdditionalChild(ID);
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

    private void ActivateAdditionalChild(int id)
    {
        if (id >= 3 && id <= 6) {
            Transform child = transform.Find(id.ToString());
            if (child != null)
                child.gameObject.SetActive(true);
            else
                Debug.LogWarning("Child mit ID " + id + " nicht gefunden.");
        }
    }

    public void SetToSupermarket()
    {

        Transform child = transform.Find("7");
        if (child != null)
            child.gameObject.SetActive(true);

        //Transform childIndicator = transform.Find("8");
        //if (childIndicator != null)
        //    childIndicator.gameObject.SetActive(true);


        for (int i = 0; i < 3; i++)
        {
            Transform notChild = transform.Find(i.ToString());
            if (notChild != null)
                notChild.gameObject.SetActive(false);
        }
    }
}
