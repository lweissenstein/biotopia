using UnityEngine.UIElements;
using UnityEngine;
using UnityEngine.SceneManagement;
using Util;
using EconomySystem;

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
        private Label _menuLabel, title;
        private Button continueGame, newGame, startTutorial, quitToMenu;
        private bool _isFirstFrame;
        private static readonly Color BackGroundColor = new(.8f, .8f, .8f, 0.5f);


        public void Start()
        {
            _isFirstFrame = true;
            _menuLabel = economyUI.rootVisualElement.Q<Label>("menuButtonLabel");
            _menuLabel.RegisterCallback<ClickEvent>(_ => OnPauseButton());

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

            HideMenu();
            if (GameState.isNewGame)
            {
                OnStartNewGameButton();
            }
        }


        public void Update()
        {

        }

        private void OnStartNewGameButton()
        {
            if (!GameState.isNewGame)
            {
                SceneManager.LoadSceneAsync("SampleScene");
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
            FoodEconomy.Reset();
            SceneManager.LoadSceneAsync("mainMenu");
        }

        private void OnStartTutorialButton()
        {
            GameState.isNewGame = true;
            SceneManager.LoadSceneAsync("tutorial");
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

    }
}