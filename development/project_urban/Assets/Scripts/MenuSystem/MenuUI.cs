using UnityEngine.UIElements;
using UnityEngine;
using UnityEngine.SceneManagement;
using Util;

namespace MenuSystem
{
    internal static class State
    {
        public static bool IsNewGame = true;
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
        private Button _pauseButton;
        private Label _menuLabel;
        private Button _continueButton;
        private Button _startNewGameButton;
        private VisualElement _visualElement;
        private static readonly Color BackGroundColor = new(.8f, .8f, .8f, 0.5f);


        public void Start()
        {
            _menuLabel = economyUI.rootVisualElement.Q<Label>("menuButtonLabel");
            _menuLabel.RegisterCallback<ClickEvent>(Object => OnPauseButton());
            //ShowGame();
            // pause the game on startup
            if (State.IsNewGame) OnPauseButton();
        }

        private void OnStartNewGameButton()
        {
            // only reload the scene if this it isn't a new game. No reason to reload a new game.
            
            if (!State.IsNewGame) SceneManager.LoadSceneAsync(0);
            State.IsNewGame = false;
            
            HideMenu();
            //ShowGame();
            ShowOtherUIs();
            GamePauser.ContinueGame();
            economyUI.rootVisualElement.style.display = DisplayStyle.Flex;
        }

        private void OnPauseButton()
        {
            GamePauser.PauseGame();
            //HideGame();
            HideOtherUIs();
            ShowMenu();
        }

        private void OnContinueButton()
        {
            economyUI.rootVisualElement.style.display = DisplayStyle.Flex;
            // do nothing if this is a new game.
            if (State.IsNewGame) return;
            HideMenu();
            //ShowGame();
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
            economyUI.rootVisualElement.style.display = DisplayStyle.Flex;
            ShowOtherUITemplate(upgradeWindowUI, State.UpgradeWindowUIDisplayStyle);
            ShowOtherUITemplate(processBuildingUI, State.ProcessBuildingUIDisplayStyle);
            return;

            void ShowOtherUITemplate(UIDocument ui, StyleEnum<DisplayStyle>? displayStyle)
            {
                if (displayStyle != null)
                {
                    ui.rootVisualElement.style.display = (StyleEnum<DisplayStyle>)displayStyle;
                }
            }
        }


        //private void ShowGame()
        //{
        //    _pauseButton.clicked += OnPauseButton;
        //    menuUI?.rootVisualElement?.Add(_pauseButton);
        //}

        //private void HideGame()
        //{
        //    menuUI?.rootVisualElement?.Remove(_pauseButton);
        //}

        private void ShowMenu()
        {
            _visualElement = new VisualElement
            {
                style =
                {
                    height = Length.Percent(100),
                    width = Length.Percent(100),
                    backgroundColor = BackGroundColor,
                }
            };

            _startNewGameButton = new Button
            {
                text = "New Game",
                name = "StartNewGameButton",
                style =
                {
                    marginTop = Length.Auto(),
                    alignSelf = Align.FlexEnd,
                    width = Length.Percent(10),
                }
            };
            _startNewGameButton.clicked += OnStartNewGameButton;

            _continueButton = new Button
            {
                text = "Continue",
                name = "ContinueButton",
                style =
                {
                    alignSelf = Align.FlexEnd,
                    marginTop = Length.Auto(),
                    width = Length.Percent(10),
                }
            };
            _continueButton.clicked += OnContinueButton;
            if (State.IsNewGame) _continueButton.style.backgroundColor = BackGroundColor;
            _visualElement.Add(_startNewGameButton);
            _visualElement.Add(_continueButton);

            menuUI?.rootVisualElement?.Add(_visualElement);
        }

        private void HideMenu()
        {
            menuUI?.rootVisualElement?.Remove(_visualElement);
        }
    }
}