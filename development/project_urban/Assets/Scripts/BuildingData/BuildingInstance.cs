using UnityEngine;

public class BuildingInstance : MonoBehaviour
{
    [SerializeField] private BoxCollider boxColliderHouse;
    public BuildingData data;
    public int level;
    public int algenCompartments;

    public int GetProduction()
    {
        return data.baseProduction * level;
    }

    public void Upgrade()
    {
        level++;
        Debug.Log("Building upgraded to level: " + level);
        UpdateCollider();
    }
    public void PrintInfo()
    {
        Debug.Log("Name: " + data.buildingName);
        Debug.Log("Basisproduktion: " + data.baseProduction);
        Debug.Log("Level: " + level);
        

        // Extra-Infos je nach Typ:
        if (data is House house)
        {
            Debug.Log("Einwohner: " + house.baseResidents);
            Debug.Log("Aktuelle Produktion: " + GetProduction() + (house.algeCompartmentProduction * algenCompartments));
        }
        else if (data is Water water)
        {
            Debug.Log("Temperatur: " + water.baseTemperature);
        }
    }

    private void UpdateCollider()
    {
        if (boxColliderHouse == null) return;

        if (level == 1)
        {
            boxColliderHouse.size = new Vector3(1f, 0.5f, 1f);
            boxColliderHouse.center = new Vector3(0.5f, 0.25f, 0.5f);
        }
        else if (level == 2)
        {
            boxColliderHouse.size = new Vector3(1f, 1.25f, 1f);
            boxColliderHouse.center = new Vector3(0.5f, 0.6f, 0.5f);
        }
        else if (level >= 3)
        {
            boxColliderHouse.size = new Vector3(1f, 2f, 1f);
            boxColliderHouse.center = new Vector3(0.5f, 1f, 0.5f);
        }
    }
}
