using System;
using UnityEngine;

public class ObjectPlacer : MonoBehaviour
{
    [SerializeField] System.Collections.Generic.List<GameObject> placedGameObject = new();
    [SerializeField] System.Collections.Generic.List<GameObject> placedGameObjectUI = new();
    ClickHandler clickHandler;
    UIHandler handler;
    public GameObject currentUI;
    public int PlaceObject(GameObject prefab, GameObject UI, Vector3 position)
    {
        GameObject newObject = Instantiate(prefab);
        GameObject newUIWindow = Instantiate(UI);
        ClickHandler cloneScript = newObject.GetComponent<ClickHandler>();
        handler = currentUI.GetComponent<UIHandler>();
        cloneScript.uiElement = newUIWindow;
        cloneScript.currentUI = currentUI;
        newObject.transform.position = position;
        placedGameObject.Add(newObject);
        placedGameObjectUI.Add(newUIWindow);
        return placedGameObject.Count - 1;
    }

    internal void RemoveObjectAt(int gameObjectIndex)
    {
        if (placedGameObject.Count <= gameObjectIndex || placedGameObject[gameObjectIndex] == null)
            return;
        Destroy(placedGameObject[gameObjectIndex]);
        Destroy(placedGameObjectUI[gameObjectIndex]);
        placedGameObject[gameObjectIndex] = null;
        placedGameObjectUI[gameObjectIndex] = null;
    }
}
