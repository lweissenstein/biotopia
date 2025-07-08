using System.Collections.Generic;
using UnityEngine;

public class BuildingUpdateManager : MonoBehaviour
{
    public static BuildingUpdateManager Instance;

    private readonly List<BuildingInstance> buildings = new();
    private int currentIndex = 0;

    public int updatesPerSecond = 20;
    private float timer = 0f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this.gameObject); // Optional, falls persistierend über Szenen hinweg
    }

    public void RegisterBuilding(BuildingInstance building)
    {
        buildings.Add(building);
    }

    private void FixedUpdate()
    {
        float updatesPerFrame = updatesPerSecond * Time.fixedDeltaTime;

        timer += updatesPerFrame;
        int updatesToDo = Mathf.FloorToInt(timer);
        timer -= updatesToDo;

        for (int i = 0; i < updatesToDo; i++)
        {
            if (buildings.Count == 0) return;
            currentIndex = (currentIndex + 1) % buildings.Count;
            buildings[currentIndex].TryConsumeProduct();
        }
    }
}