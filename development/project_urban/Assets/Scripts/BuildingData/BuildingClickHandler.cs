using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class BuildingClickHandler : MonoBehaviour
{
    [SerializeField] private Camera sceneCamera;
    [SerializeField] private LayerMask buildingLayer;
    [SerializeField] private TutorialManager tutorialManager;


    void Update()
    {

        if (GameState.allowPlayerInput)
        {
            if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
            {
                Debug.Log("Klick registriert");

                if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
                {
                    Debug.Log("Klick auf UI ignorieren");
                    return;
                }

                Vector2 mousePos = Touchscreen.current.primaryTouch.position.ReadValue();
                Ray ray = sceneCamera.ScreenPointToRay(mousePos);

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
                        if (GameState.waitForClick)
                        {
                            tutorialManager.NextStep();
                            GameState.waitForClick = false; // Reset waitForClick after handling the click
                            Debug.Log("ressetet");// Reset waitForClick after handling the click

                        }
                    }
                    else if (processor != null)
                    {

                        //Debug.Log("ProcessInstance gefunden! Leite an SelectionManager weiter.");
                        FindFirstObjectByType<ProcessSelectionManager>()?.SelectBuilding(processor);
                        FindFirstObjectByType<BuildingSelectionManager>()?.Deselect(); // Deselect Building if any was selected
                        if (GameState.waitForClick)
                        {
                            tutorialManager.NextStep();
                            GameState.waitForClick = false;
                            Debug.Log("ressetet");// Reset waitForClick after handling the click
                        }
                    }
                    else
                    {
                        //Debug.Log("Kein BuildingInstance angetroffen.");
                        FindFirstObjectByType<BuildingSelectionManager>()?.Deselect();
                        FindFirstObjectByType<ProcessSelectionManager>()?.Deselect();
                    }
                }
                else
                {
                    //Debug.Log("Raycast hat nichts getroffen.");

                    FindFirstObjectByType<BuildingSelectionManager>()?.Deselect();
                    FindFirstObjectByType<ProcessSelectionManager>()?.Deselect();
                }
            } 
        }

    }
    
}
