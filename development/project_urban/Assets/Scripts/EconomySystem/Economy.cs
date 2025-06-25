using System;
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


        private Label algeLabel, qualleLabel, salzpflanzeLabel, grilleLabel;

        private void Start()
        {
            // no idea why this would be null, sometimes they are null for a while. just skipping them in the update works good enough.
            _foodProgressBar = economyUI?.rootVisualElement?.Q<ProgressBar>("FoodProgressBar");
            _qualleProgressBar = economyUI?.rootVisualElement?.Q<ProgressBar>("QualleProgressBar");
            _algeProgressBar = economyUI?.rootVisualElement?.Q<ProgressBar>("AlgeProgressBar");
            _grilleProgressBar = economyUI?.rootVisualElement?.Q<ProgressBar>("GrilleProgressBar");
            _salzPflanzeProgressBar = economyUI?.rootVisualElement?.Q<ProgressBar>("SalzPflanzeProgressBar");

            if (isDebug)
            {
                Debug.Log("starting economy");
            }


            if (ressourceUI == null)
            {
                Debug.LogError("ressourceUI ist null!");
            }
            else
            {
                var root = ressourceUI.rootVisualElement;
                algeLabel = root.Q<Label>("algenLabel");
                qualleLabel = root.Q<Label>("quallenLabel");
                salzpflanzeLabel = root.Q<Label>("salzpflanzenLabel");
                grilleLabel = root.Q<Label>("grillenLabel");

                if (algeLabel == null)
                {
                    Debug.LogError("algeLabel wurde nicht gefunden! Name korrekt?");
                }
            }
        }

        private void FixedUpdate()
        {
            UpdateEconomyUI();

            _timer.OncePerSecondDebugLog("Algen: " + _foodEconomy.totalAlge +
                                         "  Quallen: " + _foodEconomy.totalQualle +
                                         "  Salzpflanzen: " + _foodEconomy.totalSalzpflanze +
                                         "  Grillen: " + _foodEconomy.totalGrille);

            algeLabel.text = "" + (int)_foodEconomy.totalAlge;
            qualleLabel.text = "" + (int)_foodEconomy.totalQualle;
            salzpflanzeLabel.text = "" + (int)_foodEconomy.totalSalzpflanze;
            grilleLabel.text = "" + (int)_foodEconomy.totalGrille;

            if (isDebug)
            {
                _timer.OncePerSecondDebugLog(_foodEconomy.Report());
            }
        }

        private void UpdateEconomyUI()
        {
            if (_foodProgressBar is not null) // no idea why, sometimes it's null and logs a NullPointerException
            {
                _foodProgressBar.title = $"food:{(int)Math.Round(_foodEconomy.CurrentProteinAmount)}/{(int)Math.Round(_foodEconomy.MaxProteinAmount)}";
                _foodProgressBar.highValue = _foodEconomy.MaxProteinAmount;
                _foodProgressBar.lowValue = _foodEconomy.MinProteinAmount;
                _foodProgressBar.value = _foodEconomy.CurrentProteinAmount;
            }

            UpdateProgressBarTemplate(_qualleProgressBar, $"qualle:<TOTAL>", _foodEconomy.Qualle);
            UpdateProgressBarTemplate(_algeProgressBar, $"alge:<TOTAL>", _foodEconomy.Alge);
            UpdateProgressBarTemplate(_grilleProgressBar, $"grille:<TOTAL>", _foodEconomy.Grille);
            UpdateProgressBarTemplate(_salzPflanzeProgressBar, $"salzPflanze:<TOTAL>", _foodEconomy.SalzPflanze);
        }

        private void UpdateProgressBarTemplate(ProgressBar progressBar, string title, FoodResource food)
        {
            if (progressBar is null) return;
            progressBar.title = title.Replace("<TOTAL>", ((int)Math.Round(food.Total)).ToString());
            progressBar.highValue = food.Max;
            progressBar.lowValue = food.Min;
            progressBar.value = food.Total;
        }
    }
}