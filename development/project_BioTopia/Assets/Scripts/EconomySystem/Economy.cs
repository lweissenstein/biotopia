
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UIElements;
using Util;

namespace EconomySystem
{
    public class Economy : MonoBehaviour
    {
        private readonly FoodEconomy _foodEconomy = FoodEconomy.Instance;
        private readonly Timer _timer = new();
        [SerializeField] private bool isDebug = false;
        public UIDocument economyUI;
        public UIDocument ressourceUI;
        private ProgressBar _foodProgressBar;
        private ProgressBar _qualleProgressBar;
        private ProgressBar _algeProgressBar;
        private ProgressBar _grilleProgressBar;
        private ProgressBar _salzPflanzeProgressBar;
        private float timeElapsed, timeElapsedFood;
        private int lastDisplayedSeconds;
        private float lastProteinAmount;
        private float proteinsPerSecond = 0;

        private Dictionary<ProductType, Label> _productLabels = new();
        private Label algeLabel, qualleLabel, salzpflanzeLabel, grilleLabel, creditsLabel, mainMenu, timeLabel, foodPerSecondLabel, foodTotalLabel;
        private VisualElement resourcePanel, showResources, mainmenu, moneyVisual;
        private Button enableRange;

        [SerializeField] private BuildingSelectionManager buildingSelectionManager;


        private void Start()
        {
            // no idea why this would be null, sometimes they are null for a while. just skipping them in the update works good enough.
            _foodProgressBar = economyUI?.rootVisualElement?.Q<ProgressBar>("FoodProgressBar");
            _qualleProgressBar = economyUI?.rootVisualElement?.Q<ProgressBar>("QualleProgressBar");
            _algeProgressBar = economyUI?.rootVisualElement?.Q<ProgressBar>("AlgeProgressBar");
            _grilleProgressBar = economyUI?.rootVisualElement?.Q<ProgressBar>("GrilleProgressBar");
            _salzPflanzeProgressBar = economyUI?.rootVisualElement?.Q<ProgressBar>("SalzpflanzeProgressBar");


            // hide economyUI for now
            economyUI.rootVisualElement.style.display = DisplayStyle.Flex;

            if (isDebug)
            {
                Debug.Log("starting economy");
            }

            var root = economyUI.rootVisualElement;
            algeLabel = root.Q<Label>("algeAmountLabel");
            qualleLabel = root.Q<Label>("qualleAmountLabel");
            salzpflanzeLabel = root.Q<Label>("salzpflanzeAmountLabel");
            grilleLabel = root.Q<Label>("grilleAmountLabel");
            creditsLabel = root.Q<Label>("creditsLabel");
            resourcePanel = root.Q<VisualElement>("ressourcePanel");
            mainMenu = root.Q<Label>("menuButtonLabel");
            showResources = root.Q<VisualElement>("showResources");
            timeLabel = root.Q<Label>("timeLabel");
            foodPerSecondLabel = root.Q<Label>("foodPerSecondLabel");
            foodTotalLabel = root.Q<Label>("foodTotalLabel");
            moneyVisual = root.Q<VisualElement>("moneyVisual");
            mainmenu = root.Q<VisualElement>("mainmenu");
            enableRange = root.Q<Button>("showrange");

            _productLabels = new Dictionary<ProductType, Label>
                {
                    { ProductType.AlgePowder, root.Q<Label>("countAlgePowder") },
                    { ProductType.AlgeNoodle, root.Q<Label>("countAlgeNoodle") },
                    { ProductType.AlgeJelly, root.Q<Label>("countAlgeJelly") },
                    { ProductType.AlgePatty, root.Q<Label>("countAlgePatty") },

                    { ProductType.QualleNoodle, root.Q<Label>("countQualleNoodle") },
                    { ProductType.QualleMayo, root.Q<Label>("countQualleMayo") },
                    { ProductType.QualleTofu, root.Q<Label>("countQualleTofu") },
                    { ProductType.QualleBites, root.Q<Label>("countQualleBites") },

                    { ProductType.SalzpflanzeSalt, root.Q<Label>("countSalzpflanzeSalt") },
                    { ProductType.SalzpflanzePickles, root.Q<Label>("countSalzpflanzePickles") },
                    { ProductType.SalzpflanzeSpread, root.Q<Label>("countSalzpflanzeSpread") },
                    { ProductType.SalzpflanzeChips, root.Q<Label>("countSalzpflanzeChips") },

                    { ProductType.GrilleFlour, root.Q<Label>("countGrilleFlour") },
                    { ProductType.GrilleLoaf, root.Q<Label>("countGrilleLoaf") },
                    { ProductType.GrilleChips, root.Q<Label>("countGrilleChips") },
                    { ProductType.GrilleBar, root.Q<Label>("countGrilleBar") }
                };

            showResources.RegisterCallback<ClickEvent>(Object => ToggleResourcePanel());

            CreditSystem.Instance.OnCreditsChanged += UpdateCreditDisplay;
            UpdateCreditDisplay(CreditSystem.Instance.currentCredits);
            HideResourcePanel();

            enableRange.clicked += () =>
            {
                RangeEnabler();
            };

            if (GameState.isTutorial)
            {
                moneyVisual.style.visibility = Visibility.Hidden;
                mainmenu.style.visibility = Visibility.Hidden;
            }
            //HideUI();

        }

        private void ToggleResourcePanel()
        {
            if (resourcePanel.style.display == DisplayStyle.Flex)
            {
                HideResourcePanel();
            }
            else
            {
                resourcePanel.style.display = DisplayStyle.Flex;
            }

        }


        void HideUI() => economyUI.rootVisualElement.style.display = DisplayStyle.None;
        void HideResourcePanel() => resourcePanel.style.display = DisplayStyle.None;



        private void FixedUpdate()
        {
            timeElapsed += Time.fixedDeltaTime;
            timeElapsedFood += Time.fixedDeltaTime; // reset every second

            int totalSeconds = Mathf.FloorToInt(timeElapsed);
            if (totalSeconds != lastDisplayedSeconds)
            {
                lastDisplayedSeconds = totalSeconds;
                int minutes = totalSeconds / 60;
                int seconds = totalSeconds % 60;

                timeLabel.text = string.Format("{0:00}:{1:00}", minutes, seconds);
            }

            if (timeElapsedFood >= 1f)
            {
                float delta = _foodEconomy.CurrentProteinAmount - lastProteinAmount;
                proteinsPerSecond = Mathf.RoundToInt(delta);
                lastProteinAmount = _foodEconomy.CurrentProteinAmount;
                timeElapsedFood = 0f;
                UpdateFoodPerSecond();
            }


            UpdateEconomyUI();


            algeLabel.text = "" + (int) _foodEconomy.totalResources[ResourceType.Alge];
            qualleLabel.text = "" + (int) _foodEconomy.totalResources[ResourceType.Qualle];
            salzpflanzeLabel.text = "" + (int) _foodEconomy.totalResources[ResourceType.Salzpflanze];
            grilleLabel.text = "" + (int) _foodEconomy.totalResources[ResourceType.Grille];

            if (isDebug)
            {
                _timer.OncePerSecondDebugLog(_foodEconomy.Report());
            }
        }

        private void UpdateEconomyUI()
        {
            if (_foodProgressBar is not null) // no idea why, sometimes it's null and logs a NullPointerException
            {
                _foodProgressBar.highValue = _foodEconomy.MaxProteinAmount;
                _foodProgressBar.lowValue = _foodEconomy.MinProteinAmount;
                _foodProgressBar.value = _foodEconomy.CurrentProteinAmount;
                foodTotalLabel.text = $"{(int) _foodEconomy.CurrentProteinAmount}/{_foodEconomy.MaxProteinAmount}";

            }
            UpdateProductLabels();
            UpdateProgressBarTemplate(_qualleProgressBar, ResourceType.Qualle);
            UpdateProgressBarTemplate(_algeProgressBar, ResourceType.Alge);
            UpdateProgressBarTemplate(_grilleProgressBar, ResourceType.Grille);
            UpdateProgressBarTemplate(_salzPflanzeProgressBar, ResourceType.Salzpflanze);
        }

        private void UpdateProductLabels()
        {
            foreach (var pair in _productLabels)
            {
                var product = pair.Key;
                var label = pair.Value;

                if (_foodEconomy.totalProducts.TryGetValue(product, out int amount))
                {
                    label.text = (amount).ToString();
                }
                else
                {
                    label.text = "0"; // oder "?"
                }
            }
        }

        private void UpdateProgressBarTemplate(ProgressBar progressBar, ResourceType food)
        {
            if (progressBar is null)
                return;
            progressBar.highValue = 100;
            progressBar.lowValue = 0;
            progressBar.value = _foodEconomy.totalResources[food];

        }

        private void UpdateCreditDisplay(int currentCredits)
        {
            creditsLabel.text = $"{"€" + currentCredits}";
        }

        private void UpdateFoodPerSecond()
        {
            if (foodPerSecondLabel != null)
            {
                if (proteinsPerSecond >= 0)
                    foodPerSecondLabel.text = $"+{proteinsPerSecond}/s";
                else if (proteinsPerSecond < 0)
                    foodPerSecondLabel.text = $"{proteinsPerSecond}/s";
            }
            else
            {
                Debug.LogWarning("FoodPErSecond label not found in the UI document.");
            }
        }
        public void RangeEnabler()
        {
            buildingSelectionManager = FindFirstObjectByType<BuildingSelectionManager>();
            buildingSelectionManager.ToggleSuperMarketRange();
        }
    }
}