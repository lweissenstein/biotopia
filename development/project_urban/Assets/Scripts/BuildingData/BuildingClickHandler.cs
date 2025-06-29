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
            //Debug.Log("Klick registriert");

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
                var processor = hit.collider.GetComponent<ProcessInstance>();

                if (instance != null)
                {
                    //Debug.Log("BuildingInstance gefunden! Leite an SelectionManager weiter.");
                    FindFirstObjectByType<BuildingSelectionManager>()?.SelectBuilding(instance);
                    FindAnyObjectByType<ProcessSelectionManager>()?.Deselect(); // Deselect Process if any was selected
                }
                else if (processor != null)
                {
                    Debug.Log("ProcessInstance gefunden! Leite an SelectionManager weiter.");
                    FindFirstObjectByType<ProcessSelectionManager>()?.SelectBuilding(processor);
                    FindFirstObjectByType<BuildingSelectionManager>()?.Deselect(); // Deselect Building if any was selected
                }
                else
                {
                    Debug.Log("Kein BuildingInstance angetroffen.");
                    FindFirstObjectByType<BuildingSelectionManager>()?.Deselect();
                    FindFirstObjectByType<ProcessSelectionManager>()?.Deselect();
                }
            }
            else
            {
                Debug.Log("Raycast hat nichts getroffen.");
                
                FindFirstObjectByType<BuildingSelectionManager>()?.Deselect();
                FindFirstObjectByType<ProcessSelectionManager>()?.Deselect();
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            FindFirstObjectByType<BuildingSelectionManager>()?.Deselect();
            FindFirstObjectByType<ProcessSelectionManager>()?.Deselect();
        }
    }
    
}
