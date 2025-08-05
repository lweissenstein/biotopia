using UnityEngine;

public class ObjectPlacer : MonoBehaviour
{
    [SerializeField] public System.Collections.Generic.List<GameObject> placedGameObject = new();


    public GameObject currentUI;
    public int PlaceObject(GameObject prefab, Vector3 position)
    {
        GameObject newObject = Instantiate(prefab);
        newObject.transform.position = position;
        placedGameObject.Add(newObject);
        if (newObject.TryGetComponent(out BuildingInstance building))
        {
            building.pos = position;
            if (BuildingUpdateManager.Instance != null)
            {
                BuildingUpdateManager.Instance.RegisterBuilding(building);
            }
        }

        return placedGameObject.Count - 1;
    }

    internal void RemoveObjectAt(int gameObjectIndex)
    {
        if (placedGameObject.Count <= gameObjectIndex || placedGameObject[gameObjectIndex] == null)
            return;
        Destroy(placedGameObject[gameObjectIndex]);
        placedGameObject[gameObjectIndex] = null;
    }
}
