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
        private ProgressBar _foodProgressBar;

        private void Start()
        {
            _foodProgressBar = economyUI?.rootVisualElement?.Q<ProgressBar>("FoodProgressBar");  // no idea why this would be null
            if (isDebug)
            {
                Debug.Log("starting economy");
            }
        }

        private void FixedUpdate()
        {
            if (_foodProgressBar is not null) // no idea why, sometimes it's null and logs a NullPointerException
            {
                _foodProgressBar.title = $"food:{(int)Math.Round(_foodEconomy.CurrentProteinAmount)}/{(int)Math.Round(_foodEconomy.MaxProteinAmount)}";
                _foodProgressBar.highValue = _foodEconomy.MaxProteinAmount;
                _foodProgressBar.lowValue = _foodEconomy.MinProteinAmount;
                _foodProgressBar.value = _foodEconomy.CurrentProteinAmount;
            }

            if (isDebug)
            {
                _timer.OncePerSecondDebugLog(_foodEconomy.Report());
            }
        }
    }
}