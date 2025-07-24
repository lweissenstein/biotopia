using UnityEngine.UIElements;
using UnityEngine;
using UnityEngine.SceneManagement;
using Util;
using EconomySystem;
using System.Collections;

namespace MenuSystem
{
    internal static class State
    {
        public static StyleEnum<DisplayStyle>? UpgradeWindowUIDisplayStyle = null;
        public static StyleEnum<DisplayStyle>? ProcessBuildingUIDisplayStyle = null;
        public static StyleEnum<DisplayStyle>? EconomyUIDisplayStyle = null;
    }

    public class MenuUI : MonoBehaviour
    {
        public UIDocument menuUI;
        public UIDocument upgradeWindowUI;
        public UIDocument processBuildingUI;
        public UIDocument economyUI;
        public UIDocument pauseUI;
        public UIDocument loadingScreen;
        public UIDocument deathScreen;
        private Label _menuLabel, title;
        private Button continueGame, newGame, startTutorial, quitToMenu, newGameDeath, startTutorialDeath, quitToMenuDeath;
        private ProgressBar loadingBar;
        private bool _isFirstFrame;
        private FoodEconomy foodEconomy;
        private Timer _timer = new();
        private static readonly Color BackGroundColor = new(.8f, .8f, .8f, 0.5f);

        public SoundFeedback soundFeedback;

        public void Start()
        {
            _isFirstFrame = true;
            _menuLabel = economyUI.rootVisualElement.Q<Label>("menuButtonLabel");
            _menuLabel.RegisterCallback<ClickEvent>(_ => OnPauseButton());

            foodEconomy = FoodEconomy.Instance;

            var pauseRoot = pauseUI.rootVisualElement;
            title = pauseRoot.Q<Label>("title");
            title.enableRichText = true;
            title.text = "<color=#379B1B>Bio</color>topia";
            continueGame = pauseRoot.Q<Button>("continue");
            continueGame.clicked += OnContinueButton;
            newGame = pauseRoot.Q<Button>("newgame");
            newGame.clicked += OnStartNewGameButton;
            startTutorial = pauseRoot.Q<Button>("tutorial");
            startTutorial.clicked += OnStartTutorialButton;
            quitToMenu = pauseRoot.Q<Button>("quit");
            quitToMenu.clicked += OnQuitToMenuButton;

            var deathRoot = deathScreen.rootVisualElement;
            newGameDeath = deathRoot.Q<Button>("newgame");
            newGameDeath.clicked += OnStartNewGameButton;
            startTutorialDeath = deathRoot.Q<Button>("tutorial");
            startTutorialDeath.clicked += OnStartTutorialButton;
            quitToMenuDeath = deathRoot.Q<Button>("quit");
            quitToMenuDeath.clicked += OnQuitToMenuButton;
            deathScreen.rootVisualElement.style.display = DisplayStyle.None;

            var loadingRoot = loadingScreen.rootVisualElement;
            loadingBar = loadingRoot.Q<ProgressBar>("loading");
            loadingScreen.rootVisualElement.style.display = DisplayStyle.None;
            HideMenu();
            if (GameState.isNewGame)
            {
                OnStartNewGameButton();
            }
        }

        private void Update()
        {
            _timer.OncePerSecond(CheckNoSaturation);
        }

        private void CheckNoSaturation()
        {
            if(foodEconomy.CurrentProteinAmount <= foodEconomy.MinProteinAmount)
            {
                soundFeedback.Lose(); // this does not work for some reason
                deathScreen.rootVisualElement.style.display = DisplayStyle.Flex;
                HideOtherUIs();
            }
        }

        private void OnStartNewGameButton()
        {
            if (!GameState.isNewGame)
            {
                LoadSceneWithProgress("SampleScene");
                FoodEconomy.Reset();
            }
            GameState.isNewGame = false;
            HideMenu();
            ShowOtherUIs();
            GamePauser.ContinueGame();
            economyUI.rootVisualElement.style.display = DisplayStyle.Flex;
        }

        private void OnQuitToMenuButton()
        {
            HideMenu();
            FoodEconomy.Reset();
            LoadSceneWithProgress("mainMenu");
        }

        private void OnStartTutorialButton()
        {
            HideMenu();
            GameState.isNewGame = true;
            LoadSceneWithProgress("tutorial");
        }

        private void OnPauseButton()
        {
            GamePauser.PauseGame();
            HideOtherUIs();
            ShowMenu();
        }

        private void OnContinueButton()
        {
 
            HideMenu();
            ShowOtherUIs();
            GamePauser.ContinueGame();
        }

        private void HideOtherUIs()
        {
            // backup
            State.EconomyUIDisplayStyle = economyUI.rootVisualElement.style.display;
            State.UpgradeWindowUIDisplayStyle = upgradeWindowUI.rootVisualElement.style.display;
            State.ProcessBuildingUIDisplayStyle = processBuildingUI.rootVisualElement.style.display;

            economyUI.rootVisualElement.style.display = DisplayStyle.None;
            upgradeWindowUI.rootVisualElement.style.display = DisplayStyle.None;
            processBuildingUI.rootVisualElement.style.display = DisplayStyle.None;
        }

        private void ShowOtherUIs()
        {
            ShowOtherUITemplate(upgradeWindowUI, State.UpgradeWindowUIDisplayStyle);
            ShowOtherUITemplate(processBuildingUI, State.ProcessBuildingUIDisplayStyle);
            ShowOtherUITemplate(economyUI, State.EconomyUIDisplayStyle);
            return;

            void ShowOtherUITemplate(UIDocument ui, StyleEnum<DisplayStyle>? displayStyle)
            {
                if (displayStyle != null)
                {
                    ui.rootVisualElement.style.display = (StyleEnum<DisplayStyle>)displayStyle;
                }
            }
        }
        void HideMenu() => pauseUI.rootVisualElement.style.display = DisplayStyle.None;
        void ShowMenu() => pauseUI.rootVisualElement.style.display = DisplayStyle.Flex;
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
}