
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


        private Label algeLabel, qualleLabel, salzpflanzeLabel, grilleLabel, creditsLabel, mainMenu, timeLabel, foodPerSecondLabel, foodTotalLabel;
        private VisualElement resourcePanel, showResources;

        private void Start()
        {
            // no idea why this would be null, sometimes they are null for a while. just skipping them in the update works good enough.
            _foodProgressBar = economyUI?.rootVisualElement?.Q<ProgressBar>("FoodProgressBar");
            _qualleProgressBar = economyUI?.rootVisualElement?.Q<ProgressBar>("QualleProgressBar");
            _algeProgressBar = economyUI?.rootVisualElement?.Q<ProgressBar>("AlgeProgressBar");
            _grilleProgressBar = economyUI?.rootVisualElement?.Q<ProgressBar>("GrilleProgressBar");
            _salzPflanzeProgressBar = economyUI?.rootVisualElement?.Q<ProgressBar>("SalzPflanzeProgressBar");

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

            showResources.RegisterCallback<ClickEvent>(Object => ToggleResourcePanel());

            CreditSystem.Instance.OnCreditsChanged += UpdateCreditDisplay;
            UpdateCreditDisplay(CreditSystem.Instance.currentCredits);
            HideResourcePanel();

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

            int totalSeconds = Mathf.FloorToInt(timeElapsedFood);
            if (totalSeconds != lastDisplayedSeconds)
            {
                lastDisplayedSeconds = totalSeconds;
                int minutes = totalSeconds / 60;
                int seconds = totalSeconds % 60;

                timeLabel.text = string.Format("{0:00}:{1:00}", minutes, seconds);
            }

            if(timeElapsedFood >= 1f)
            {
                float delta = _foodEconomy.CurrentProteinAmount - lastProteinAmount;
                proteinsPerSecond = Mathf.RoundToInt(delta / timeElapsed);
                lastProteinAmount = _foodEconomy.CurrentProteinAmount;
                timeElapsedFood = 0f;
                UpdateFoodPerSecond();
            }


            UpdateEconomyUI();
            

            //Debug.Log("------------------------------------------");

            //Debug.Log("Algen: " + _foodEconomy.totalResources[ResourceType.Alge] +
            //          "  Quallen: " + _foodEconomy.totalResources[ResourceType.Qualle] +
            //          "  Salzpflanzen: " + _foodEconomy.totalResources[ResourceType.Salzpflanze] +
            //          "  Grillen: " + _foodEconomy.totalResources[ResourceType.Grille]);

            //Debug.Log("AlgePowder: " + _foodEconomy.totalProducts[ProductType.AlgePowder] +
            //          "  AlgeNoodle: " + _foodEconomy.totalProducts[ProductType.AlgeNoodle] +
            //          "  AlgeJelly: " + _foodEconomy.totalProducts[ProductType.AlgeJelly] +
            //          "  AlgePatty: " + _foodEconomy.totalProducts[ProductType.AlgePatty]);

            //Debug.Log("QualleNoodle: " + _foodEconomy.totalProducts[ProductType.QualleNoodle] +
            //            "  QualleMayo: " + _foodEconomy.totalProducts[ProductType.QualleMayo] +
            //            "  QualleTofu: " + _foodEconomy.totalProducts[ProductType.QualleTofu] +
            //            "  QualleBites: " + _foodEconomy.totalProducts[ProductType.QualleBites]);

            //Debug.Log("SalzpflanzeSalt: " + _foodEconomy.totalProducts[ProductType.SalzpflanzeSalt] +
            //            "  SalzpflanzePickles: " + _foodEconomy.totalProducts[ProductType.SalzpflanzePickles] +
            //            "  SalzpflanzeSpread: " + _foodEconomy.totalProducts[ProductType.SalzpflanzeSpread] +
            //            "  SalzpflanzeChips: " + _foodEconomy.totalProducts[ProductType.SalzpflanzeChips]);

            //Debug.Log("GrilleFlour: " + _foodEconomy.totalProducts[ProductType.GrilleFlour] +
            //            "  GrilleLoaf: " + _foodEconomy.totalProducts[ProductType.GrilleLoaf] +
            //            "  GrilleChips: " + _foodEconomy.totalProducts[ProductType.GrilleChips] +
            //            "  GrilleBar: " + _foodEconomy.totalProducts[ProductType.GrilleBar]);

            algeLabel.text = "" + (int)_foodEconomy.totalResources[ResourceType.Alge];
            qualleLabel.text = "" + (int)_foodEconomy.totalResources[ResourceType.Qualle];
            salzpflanzeLabel.text = "" + (int)_foodEconomy.totalResources[ResourceType.Salzpflanze];
            grilleLabel.text = "" + (int)_foodEconomy.totalResources[ResourceType.Grille];

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

            UpdateProgressBarTemplate(_qualleProgressBar, ResourceType.Qualle);
            UpdateProgressBarTemplate(_algeProgressBar, ResourceType.Alge);
            UpdateProgressBarTemplate(_grilleProgressBar, ResourceType.Grille);
            UpdateProgressBarTemplate(_salzPflanzeProgressBar, ResourceType.Salzpflanze);
        }

        private void UpdateProgressBarTemplate(ProgressBar progressBar, ResourceType food)
        {
            if (progressBar is null) return;
            progressBar.highValue = 100;
            progressBar.lowValue = 0;
            progressBar.value = _foodEconomy.totalResources[food];

        }

        private void UpdateCreditDisplay(int currentCredits)
        {
            creditsLabel.text = $"{"€" +  currentCredits}/s";
            foodPerSecondLabel.text = $"{(proteinsPerSecond >= 0 ? "+" : "")}{proteinsPerSecond}/s";
        }

        private void UpdateFoodPerSecond()
        {
            if (foodPerSecondLabel != null)
            {
                if(proteinsPerSecond >= 0)
                    foodPerSecondLabel.text = $"+{proteinsPerSecond}/s";
                else if (proteinsPerSecond < 0)
                    foodPerSecondLabel.text = $"-{proteinsPerSecond}/s";
            }
            else
            {
                Debug.LogWarning("FoodPErSecond label not found in the UI document.");
            }
        }
    }
}