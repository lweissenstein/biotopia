using System;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.VFX;

public class MainMenuController : MonoBehaviour
{
    public VisualElement ui;

    public Button playButton;
    public Button optionsButton;
    public Button quitButton;

    private void Awake()
    {
        ui = GetComponent<UIDocument>().rootVisualElement;
    }

    private void OnEnable()
    {
        playButton = ui.Q<Button>("PlayButton");
        optionsButton = ui.Q<Button>("OptionsButton");
        quitButton = ui.Q<Button>("QuitButton");
        playButton.clicked += OnPlayButtonClicked;
        optionsButton.clicked += OnOptionsButtonClicked;
        quitButton.clicked += OnQuitButtonClicked;
    }

    private void OnQuitButtonClicked()
    {
       Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    private void OnOptionsButtonClicked()
    {
        Debug.Log("Options button clicked. Implement options menu here. Da kommt noch was hin");
    }

    private void OnPlayButtonClicked()
    {
        gameObject.SetActive(false);
    }
}
