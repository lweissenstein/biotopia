using NUnit.Framework;
using System;
using UnityEngine;
using UnityEngine.LightTransport;

public class PlacementSystem : MonoBehaviour
{
    [SerializeField] private MouseInputManager inputManager;
    [SerializeField] Grid grid;

    [SerializeField] private ObjectsDatabaseSO database;

    [SerializeField] private GameObject gridVisualzation;

    [SerializeField] private PreviewSystem preview;

    private GridData floorData, furnitureData;

    private Vector3Int lastDetectedPosition = Vector3Int.zero;

    [SerializeField] private ObjectPlacer objectPlacer;

    IBuildingState buildingState;

    [SerializeField] private SoundFeedback soundFeedback;

    private void Start()
    {
        StopPlacement();
        floorData = new();
        furnitureData = new();
    }

    public void StartPlacement(int ID)
    {
        StopPlacement();
        gridVisualzation.SetActive(true);
        buildingState = new PlacementState(ID,
                                           grid,
                                           preview,
                                           database,
                                           floorData,
                                           furnitureData,
                                           objectPlacer,
                                           soundFeedback);
        inputManager.OnClick += PlaceStructure;
        inputManager.OnExit += StopPlacement;
    }

    public void StartRemoving()
    {
        StopPlacement();
        gridVisualzation.SetActive(true);
        buildingState = new RemovingState(grid,
                                          preview,
                                          floorData,
                                          furnitureData,
                                          objectPlacer);
        inputManager.OnClick += PlaceStructure;
        inputManager.OnExit += StopPlacement;
    }

    private void PlaceStructure()
    {
        if (inputManager.IsPointerOverUI()) {return;}

        Vector3 mousePosition = inputManager.getSelectedMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);

        buildingState.OnAction(gridPosition);
    }

    //private bool CheckPlacementValidity(Vector3Int gridPosition, int selectedObjectIndex)
    //{
    //    GridData selectedData = database.objectsData[selectedObjectIndex].ID == 4 ? floorData : furnitureData;

    //    return selectedData.CanPlaceObjectAt(gridPosition, database.objectsData[selectedObjectIndex].Size);
    //}

    private void StopPlacement()
    {
        if (buildingState == null)
            return;
        gridVisualzation.SetActive(false);
        buildingState.EndState();
        inputManager.OnClick -= PlaceStructure;
        inputManager.OnExit -= StopPlacement;
        lastDetectedPosition = Vector3Int.zero;
        buildingState = null;
    }

    private void Update()
    {
        if (buildingState == null)
            return;
        Vector3 mousePosition = inputManager.getSelectedMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);
        if(lastDetectedPosition != gridPosition)
        {
            buildingState.UpdateState(gridPosition);
            lastDetectedPosition = gridPosition;
        }
        
    }

    // uses randomPlace to randomely place objects
    public void placeRandom()
    {
        int objectID = 0;
        var data = database.objectsData[objectID];


        RandomGridManipulation placer = new RandomGridManipulation(grid, furnitureData, objectPlacer, data.Prefab, data.UIWindow, data.Size);

        placer.RandomPlace(5, 20, 20);
    }

    // uses RandomWeightedPlace to randomely place objects
    public void weightedPlaceRandom()
    {
        int objectID = 0;
        var data = database.objectsData[objectID];


        RandomGridManipulation placer = new RandomGridManipulation(grid, furnitureData, objectPlacer, data.Prefab, data.UIWindow, data.Size);

        placer.RandomWheightedPlace(5, 20, 20);
    }

    // uses RandomUpgrade to randomely upgrade objects
    public void upgradeRandom()
    {
        int objectID = 2;
        var data = database.objectsData[objectID];


        RandomGridManipulation placer = new RandomGridManipulation(grid, furnitureData, objectPlacer, data.Prefab, data.UIWindow, data.Size);

        placer.RandomUpgrade(1, 20, 20);
    }

    // uses RandomWeightedUpgrade to randomely upgrade objects
    public void weightedUpgradeRandom()
    {
        int objectID = 2;
        var data = database.objectsData[objectID];


        RandomGridManipulation placer = new RandomGridManipulation(grid, furnitureData, objectPlacer, data.Prefab, data.UIWindow, data.Size);

        placer.RandomWheightedUpgrade(1, 20, 20);
    }

    // either creates a base city using RandomPlace and RandomUpgrade in the center, or grows the City if there is already one using RandomWeightedPlace and RandomWeightedUpgrade
    public void SimulateGrowth()
    {
        bool empty = true;

        foreach (var pos in furnitureData.GetAllOccupiedPositions())
        {
            empty = false;
            break;
        }

        if (empty)
        {
            int objectID = 0;
            var data = database.objectsData[objectID];

            RandomGridManipulation placer = new RandomGridManipulation(grid, furnitureData, objectPlacer, data.Prefab, data.UIWindow, data.Size);
            placer.RandomPlace(10, 20 / 3, 20 / 3);

            int upgradeID = 2;
            var upgradeData = database.objectsData[upgradeID];

            RandomGridManipulation upgradePlacer = new RandomGridManipulation(grid, furnitureData, objectPlacer, upgradeData.Prefab, upgradeData.UIWindow, upgradeData.Size);
            upgradePlacer.RandomUpgrade(3, 20, 20);
        }
        else
        {
            int objectID = 0;
            var data = database.objectsData[objectID];

            RandomGridManipulation placer = new RandomGridManipulation(grid, furnitureData, objectPlacer, data.Prefab, data.UIWindow, data.Size);
            placer.RandomWheightedPlace(3, 20, 20);

            int upgradeID = 2;
            var upgradeData = database.objectsData[upgradeID];

            RandomGridManipulation upgradePlacer = new RandomGridManipulation(grid, furnitureData, objectPlacer, upgradeData.Prefab, upgradeData.UIWindow, upgradeData.Size);
            upgradePlacer.RandomWheightedUpgrade(1, 20, 20);
        }            
    }
}
