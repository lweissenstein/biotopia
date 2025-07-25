using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using EconomySystem;
public class TutorialManager : MonoBehaviour
{
    [Header("Tutorial Settings")]
    public TutorialSequence tutorialSequence;

    [Header("UI Toolkit References")]
    public UIDocument uiDocument;
    private VisualElement leftPanel, rightPanel, menuRessourcePanel, moneyVisual, bottomVisual, grayOutElement;
    private Label tutorialTextLabelR, tutorialTextLabelL, titleL, titleR, bottomText, creditsLabel;
    private Button continueLeftButton, continueRightButton;
    public ProgressBar loadingBar;
    public UIDocument uiProcess;
    public UIDocument uiUpgrade;
    public UIDocument uiMenu;
    public UIDocument uiGrayOut;
    public UIDocument loadingScreen;
    public bool tessa, anton;
    Sprite grayOutImage2, grayOutImage3;

    [Header("Camera Input Controller")]
    public CameraInputController cameraInputController;

    private int currentStepIndex = 0;
    private Vector3 initialCamera = new Vector3(0, 0, 0);

    void Awake()
    {
        // freeze the game state for the tutorial
            GameState.allowProduction = false;
            GameState.allowSaturation = false;
            GameState.allowPlayerInput = false;
            GameState.allowBuildingSpawn = false;
            GameState.allowUpdates = false;
            GameState.allowConsumption = false;
            GameState.isTutorial = true;
    }


    void Start()
    {
        var root = uiDocument.rootVisualElement;
        leftPanel = root.Q<VisualElement>("tutorialLeft");
        rightPanel = root.Q<VisualElement>("tutorialRight");
        titleL = root.Q<Label>("titleLeft");
        titleR = root.Q<Label>("titleRight");
        tutorialTextLabelL = root.Q<Label>("descriptionLeft");
        tutorialTextLabelR = root.Q<Label>("descriptionRight");
        continueLeftButton = root.Q<Button>("continueLeft");
        continueRightButton = root.Q<Button>("continueRight");
        bottomVisual = root.Q<VisualElement>("bottomVisual");
        bottomText = root.Q<Label>("bottomText");

        var menuRoot = uiMenu.rootVisualElement;
        menuRessourcePanel = menuRoot.Q<VisualElement>("ressourcePanel");
        moneyVisual = menuRoot.Q<VisualElement>("moneyVisual");
        creditsLabel = menuRoot.Q<Label>("creditsLabel");

        continueLeftButton.clicked += NextStep;
        continueRightButton.clicked += NextStep;

        uiGrayOut.rootVisualElement.style.display = DisplayStyle.None;
        grayOutElement = uiGrayOut.rootVisualElement.Q<VisualElement>("grayoutElement");

        var loadingRoot = loadingScreen.rootVisualElement;
        loadingBar = loadingRoot.Q<ProgressBar>("loading");
        loadingScreen.rootVisualElement.style.display = DisplayStyle.None;
        HideUI();
        ShowCurrentStep();
    }

    void ShowCurrentStep()
    {
        if (currentStepIndex >= tutorialSequence.steps.Count)
        {
            Debug.Log("Tutorial abgeschlossen");
            EndTutorial();
            return;
        }
        var step = tutorialSequence.steps[currentStepIndex];
        HideShowSpecificUI(step);
        SetGameState(step);
        switch (step.stepType)
        {
            case TutorialStepType.TextPopup:
                titleL.text = step.titleText;
                titleR.text = step.titleText;
                leftPanel.style.display = step.isLeft ? DisplayStyle.Flex : DisplayStyle.None;
                rightPanel.style.display = step.isRight ? DisplayStyle.Flex : DisplayStyle.None;
                continueLeftButton.style.display = step.buttonLeft ? DisplayStyle.Flex : DisplayStyle.None;
                continueRightButton.style.display = step.buttonRight ? DisplayStyle.Flex : DisplayStyle.None;
                tutorialTextLabelL.text = step.tutorialText;
                tutorialTextLabelR.text = step.tutorialText;
                ShowUI();
                break;

            case TutorialStepType.CameraMovement:
                if (step.spinCam360)
                {
                    cameraInputController.StartCameraSpinSmooth(step.cameraPosition, step.cameraOrthogrpahicSize, step.cameraVerticalAxis, step.cameraMoveTime);
                } else
                {
                    cameraInputController.StartCameraMoveSmooth(step.cameraPosition, step.cameraOrthogrpahicSize, step.cameraHorizontalAxis, step.cameraVerticalAxis, step.cameraMoveTime);
                }
                StartCoroutine(WaitThenNextStep(step.secondsToWait));
                break;

            case TutorialStepType.WaitForClick:
                Debug.Log("not implemented yet");
                break; 

            case TutorialStepType.Cutscene:
                Debug.Log("not implemented yet");
                break; 

            case TutorialStepType.SpectateForSeconds:
                StartCoroutine(WaitThenNextStep(step.secondsToWait));
                break;

            case TutorialStepType.UntenText:
                bottomText.text = step.tutorialText;
                bottomVisual.style.display = DisplayStyle.Flex;
                leftPanel.style.display = DisplayStyle.None;
                rightPanel.style.display = DisplayStyle.None;
                ShowUI();
                StartCoroutine(WaitThenNextStep(step.secondsToWait));
                break;
            default:
                Debug.LogWarning($"Unknown step type: {step.stepType}");
                return;
        }

        
    }

    private IEnumerator WaitThenNextStep(float seconds)
    {
        Debug.Log($"Waiting for {seconds} seconds before proceeding to the next step.");
        yield return new WaitForSeconds(seconds);
        NextStep();
    }

    void SetGameState(TutorialStepData steppy)
    {
        Debug.Log($"Setting GameState for step: {steppy.stepName}");
        Debug.Log($"AllowProduction: {steppy.allowProduction}, AllowSaturation: {steppy.allowSaturation}, AllowPlayerInput: {steppy.allowPlayerInput}, AllowBuildingSpawn: {steppy.allowBuildingSpawn}, AllowUpdates: {steppy.allowUpdates}");
        GameState.allowProduction = steppy.allowProduction;
        GameState.allowSaturation = steppy.allowSaturation;
        GameState.allowConsumption = steppy.allowConsumption;
        GameState.allowPlayerInput = steppy.allowPlayerInput;
        GameState.allowBuildingSpawn = steppy.allowBuildingSpawn;
        GameState.allowUpdates = steppy.allowUpdates;
        GameState.waitForClick = steppy.waitForClick;
        GameState.alge = steppy.alge;
        GameState.supermarkt = steppy.supermarkt;
        GameState.algePowder = steppy.algePowder;
        GameState.waitForButton = steppy.waitForButton;
    }

    public void NextStep()
    {
        HideUI();
        bottomVisual.style.display = DisplayStyle.None;
        uiGrayOut.rootVisualElement.style.display = DisplayStyle.None;
        currentStepIndex++;
        ShowCurrentStep();
    }

    void EndTutorial()
    {
        Debug.Log("Ending tutorial and loading main scene");
        GameState.allowProduction = true;
        GameState.allowSaturation = true;
        GameState.allowPlayerInput = true;
        GameState.allowBuildingSpawn = true;
        GameState.allowUpdates = true;
        GameState.allowConsumption = true;
        GameState.isTutorial = false;
        LoadSceneWithProgress("SampleScene");
        FoodEconomy.Reset();

        if (tessa && anton)
        {
            Debug.Log("<3");
        }
    }

    void HideUI() => uiDocument.rootVisualElement.style.display = DisplayStyle.None;
    void ShowUI() => uiDocument.rootVisualElement.style.display = DisplayStyle.Flex;

    void HideShowSpecificUI(TutorialStepData steppo)
    {
        if (steppo.grayOutOpen)
        {
            ShowGrayOut(steppo);
        }
        if (steppo.menuOpen)
        {
            menuRessourcePanel.style.display = DisplayStyle.Flex;
        }
        if (steppo.showMoney)
        {
            moneyVisual.style.visibility = Visibility.Visible;
        }
        if (steppo.processClose)
        {
            uiProcess.rootVisualElement.style.display = DisplayStyle.None;
        }
        if(steppo.upgradeClose)
        {
            uiUpgrade.rootVisualElement.style.display = DisplayStyle.None;
        }
        if (steppo.menuClose)
        {
            menuRessourcePanel.style.display = DisplayStyle.None;
        }
       
    }

    void ShowGrayOut(TutorialStepData stippi)
    {
        switch (stippi.grayOutIndex)
        {
            case 1:
                uiGrayOut.rootVisualElement.style.display = DisplayStyle.Flex;
                return;
            case 2:
                grayOutElement.style.backgroundImage = new StyleBackground(Resources.Load<Texture2D>("grayout_products"));
                uiGrayOut.rootVisualElement.style.display = DisplayStyle.Flex;
                return;
            case 3:
                grayOutElement.style.backgroundImage = new StyleBackground(Resources.Load<Texture2D>("grayout_moneysaturation"));
                uiGrayOut.rootVisualElement.style.display = DisplayStyle.Flex;
                return;
            default:
                Debug.LogWarning($"Unknown gray out index: {stippi.grayOutIndex}");
                return;
        }
    }

    public void LoadSceneWithProgress(string sceneName)
    {
        StartCoroutine(LoadSceneAsync(sceneName));
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        loadingScreen.rootVisualElement.style.display = DisplayStyle.Flex;
        loadingBar.value = 0;

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        asyncLoad.allowSceneActivation = false;

        while (!asyncLoad.isDone)
        {
            float progress = Mathf.Clamp01(asyncLoad.progress / 0.9f); // Bis max. 0.9
            loadingBar.value = progress * 100f;

            // Optional: Sofort aktivieren
            if (asyncLoad.progress >= 0.9f)
            {
                asyncLoad.allowSceneActivation = true;
            }

            yield return null;
        }
    }
}
