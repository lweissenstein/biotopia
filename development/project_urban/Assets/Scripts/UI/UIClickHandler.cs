using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class ClickHandler : MonoBehaviour, IPointerClickHandler
{ 
    [SerializeField] public GameObject uiElement;
    public GameObject currentUI;
    UIHandler handler;

    private void Start()
    {
        uiElement.SetActive(false);
        handler = currentUI.GetComponent<UIHandler>();
        Debug.Log(handler);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("Esc!");
            uiElement.SetActive(false);
            handler.currentUI = null;
        }
    }
    public void OnPointerClick(PointerEventData pointerEventData)
    {
        if (handler.currentUI != null)
        {
            if (uiElement.activeSelf == true)
            {
                return;
            }
            handler.currentUI.SetActive(false);
            uiElement.SetActive(true);
            handler.currentUI = uiElement;
        }
        else
        {
            uiElement.SetActive(true);
            handler.currentUI = uiElement;
        }
    }
}