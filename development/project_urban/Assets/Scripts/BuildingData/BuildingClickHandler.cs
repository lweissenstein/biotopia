using UnityEngine;
using UnityEngine.EventSystems;

public class BuildingClickHandler : MonoBehaviour
{
    [SerializeField] private Camera sceneCamera;
    [SerializeField] private LayerMask buildingLayer;
   

    void Update()
    {

        if (Input.GetMouseButtonDown(0))
           {
            Debug.Log("Klick registriert");

            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            {
                Debug.Log("Klick auf UI ignorieren");
                return;
            }

            Ray ray = sceneCamera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit, 100f, buildingLayer))
            {
                Debug.Log("Clicked on: " + hit.collider.name);

                var instance = hit.collider.GetComponent<BuildingInstance>();
                if (instance != null)
                {
                    Debug.Log("BuildingInstance gefunden! Leite an SelectionManager weiter.");
                    FindFirstObjectByType<BuildingSelectionManager>()?.SelectBuilding(instance);
                }
                else
                {
                    Debug.Log("Kein BuildingInstance angetroffen.");
                    FindFirstObjectByType<BuildingSelectionManager>()?.Deselect();
                }
            }
            else
            {
                Debug.Log("Raycast hat nichts getroffen.");
                
                FindFirstObjectByType<BuildingSelectionManager>()?.Deselect();
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            FindFirstObjectByType<BuildingSelectionManager>()?.Deselect();
        }
    }
    
}
