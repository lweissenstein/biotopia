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

    public void placeRandom()
    {
        // welches Object wird platziert
        int objectID = 0;
        var data = database.objectsData[objectID];


        RandomGridManipulation placer = new RandomGridManipulation(grid, furnitureData, objectPlacer, data.Prefab, data.Size);

        placer.RandomPlace(5, 20, 20);
    }

    public void weightedPlaceRandom()
    {
        // welches Object wird platziert
        int objectID = 0;
        var data = database.objectsData[objectID];


        RandomGridManipulation placer = new RandomGridManipulation(grid, furnitureData, objectPlacer, data.Prefab, data.Size);

        placer.RandomWheightedPlace(5, 20, 20);
    }

    public void upgradeRandom()
    {
        // welches Object wird platziert
        int objectID = 2;
        var data = database.objectsData[objectID];


        RandomGridManipulation placer = new RandomGridManipulation(grid, furnitureData, objectPlacer, data.Prefab, data.Size);

        placer.RandomUpgrade(1, 20, 20);
    }
}
