using EconomySystem;
using System.Collections.Generic;
using UnityEngine;
using Util;


public class BuildingUpdateManager : MonoBehaviour
{
    public static BuildingUpdateManager Instance;
    [SerializeField] public DiversityTracker diversityTracker; // Referenz auf den DiversityTracker

    private readonly List<BuildingInstance> buildings = new();
    private int currentIndex = 0;
    private FoodEconomy foodEconomy;
    public CreditSystem creditSystem;
    public int updatesPerSecond = 20;
    private float timer = 0f;

    private Timer timerClass = new();

    private int indexProduction = 0;
    private int indexConsume = 0;
    private int indexSaturation = 0;

    private float diversityTimer = 0f;
    private const float diversityInterval = 60f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
        foodEconomy = FoodEconomy.Instance;

    }

    public void RegisterBuilding(BuildingInstance building)
    {
        buildings.Add(building);
    }

    private void FixedUpdate()
    {
        if (!GameState.allowUpdates) return;
        if (buildings.Count == 0) return;

        float updatesPerFrame = updatesPerSecond * Time.fixedDeltaTime;
        timer += updatesPerFrame;
        int updatesToDo = Mathf.FloorToInt(timer);
        timer -= updatesToDo;
        int consumePerFrame = Mathf.CeilToInt(buildings.Count * 0.25f / updatesPerSecond);
        
        if(GameState.isTutorial)
        {
            if(GameState.allowConsumption)
            {
                Debug.Log("bin drinne");
                creditSystem.AddCredits(1);
                foodEconomy.OnProduceFood(0.5f);
            }

            if (GameState.allowSaturation)
            {
                foodEconomy.OnConsumeFood(0.2f);
            }
        }


        for (int j = 0; j < consumePerFrame; j++)
        {
            buildings[indexConsume].TryConsumeProduct();
            indexConsume = (indexConsume + 1) % buildings.Count;
        } 
 
        for (int i = 0; i < updatesToDo; i++)
        {
            buildings[indexProduction].GetProduction();
            indexProduction = (indexProduction + 1) % buildings.Count;

        } 
 
        int saturationPerFrame = Mathf.CeilToInt(buildings.Count * 1f / updatesPerSecond);
        for (int j = 0; j < saturationPerFrame; j++)
        {
            buildings[indexSaturation].ConsumeTotalSaturation();
            indexSaturation = (indexSaturation + 1) % buildings.Count;
        }


        

        timerClass.OncePerIntervallSeconds(() =>
        {
            if (diversityTracker != null)
            {
                diversityTracker.Reset();
                Debug.Log("DiversityTracker wurde zur�ckgesetzt (jede Minute).");
            }
            else
            {
                Debug.LogWarning("not worikng");
            }
        }, 60);
    }

}