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

    public void ActivateChild(int ID)
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

    public void SetActiveSolarPanels(int height)
    {
        for (int i = 9; i < 12; i++)
        {
            Transform child = transform.Find((i).ToString());
            if (child != null)
                child.gameObject.SetActive(i == height + 8);

        }
    }

    public void ToggleCompartmentUpgradeableBeacon(int compartmentType, bool beaconOn)
    {
        Transform child = transform.Find((compartmentType + 9).ToString());
        if (child != null)
            child.gameObject.SetActive(beaconOn);
    }

    public void ChangeColor(string productionType, int height)
    {
        switch (height)
        {
            case 1:
                ChangeColorSmall(productionType);
                break;
            case 2:
                ChangeColorMedium(productionType);
                break;
            case 3:
                ChangeColorLarge(productionType);
                break;
        }
    }

    public void ChangeColorSmall(string productionType)
    {
        Transform child = transform.Find((0).ToString());

        switch (productionType)
        {
            case "Alge":
                child.Find("House1_Body").gameObject.SetActive(false);
                child.Find("House1_Body_Alge").gameObject.SetActive(true);
                break;
            case "Halophyte":
                child.Find("House1_Body").gameObject.SetActive(false);
                child.Find("House1_Body_Halophyte").gameObject.SetActive(true);
                break;
            case "Qualle":
                child.Find("House1_Body").gameObject.SetActive(false);
                child.Find("House1_Body_Qualle").gameObject.SetActive(true);
                break;
            case "Grille":
                child.Find("House1_Body").gameObject.SetActive(false);
                child.Find("House1_Body_Grille").gameObject.SetActive(true);
                break;
        }
    }

    public void ChangeColorMedium(string productionType)
    {
        Transform child = transform.Find((1).ToString());

        switch (productionType)
        {
            case "Alge":
                child.Find("House2_Body1").gameObject.SetActive(false);
                child.Find("House2_Body1_Alge").gameObject.SetActive(true);
                break;
            case "Halophyte":
                child.Find("House2_Body1").gameObject.SetActive(false);
                child.Find("House2_Body1_Halophyte").gameObject.SetActive(true);
                break;
            case "Qualle":
                child.Find("House2_Body1").gameObject.SetActive(false);
                child.Find("House2_Body1_Qualle").gameObject.SetActive(true);
                break;
            case "Grille":
                child.Find("House2_Body1").gameObject.SetActive(false);
                child.Find("House2_Body1_Grille").gameObject.SetActive(true);
                break;
        }
    }

    public void ChangeColorLarge(string productionType)
    {
        Transform child = transform.Find((2).ToString());

        switch (productionType)
        {
            case "Alge":
                child.Find("House3_Body").gameObject.SetActive(false);
                child.Find("House3_Body_Alge").gameObject.SetActive(true);
                break;
            case "Halophyte":
                child.Find("House3_Body").gameObject.SetActive(false);
                child.Find("House3_Body_Halophyte").gameObject.SetActive(true);
                break;
            case "Qualle":
                child.Find("House3_Body").gameObject.SetActive(false);
                child.Find("House3_Body_Qualle").gameObject.SetActive(true);
                break;
            case "Grille":
                child.Find("House3_Body").gameObject.SetActive(false);
                child.Find("House3_Body_Grille").gameObject.SetActive(true);
                break;
        }
    }

    private void ActivateAdditionalChild(int id)
    {
        if (id >= 3 && id <= 6)
        {
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

        for (int i = 0; i < 3; i++)
        {
            Transform notChild = transform.Find(i.ToString());
            if (notChild != null)
                notChild.gameObject.SetActive(false);
        }
    }
}
