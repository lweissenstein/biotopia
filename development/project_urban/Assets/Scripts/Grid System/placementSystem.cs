using NUnit.Framework;
using System;
using System.Linq;
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

    private RandomGridManipulation randomPlacer;

    private void Start()
    {
        StopPlacement();
        floorData = new();
        furnitureData = new();
        randomPlacer = new RandomGridManipulation(grid, objectPlacer, database);
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
        randomPlacer.RandomPlace(5, 20, 20, furnitureData, 0);
    }

    // uses RandomWeightedPlace to randomely place objects
    public void weightedPlaceRandom()
    {
        randomPlacer.RandomWheightedPlace(5, 20, 20, furnitureData, 0);
    }

    // uses RandomUpgrade to randomely upgrade objects
    public void upgradeRandom()
    {
        randomPlacer.RandomUpgrade(1, 20, 20, furnitureData, 1);
    }

    // uses RandomWeightedUpgrade to randomely upgrade objects
    public void weightedUpgradeRandom()
    {
        randomPlacer.RandomWheightedUpgrade(1, 20, 20, furnitureData, 1);
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
            randomPlacer.GenerateRiver(20, 20, furnitureData, 3);
            randomPlacer.RandomPlace(10, 20 / 3, 20 / 3, furnitureData, 0);
            randomPlacer.RandomUpgrade(3, 20, 20, furnitureData, 1);
            randomPlacer.RandomUpgrade(1, 20, 20, furnitureData, 2);
        }
        else
        {
            randomPlacer.RandomWheightedPlace(5, 20, 20, furnitureData, 0);
            randomPlacer.RandomWheightedUpgrade(2, 20, 20, furnitureData, 1);
            randomPlacer.RandomWheightedUpgrade(2, 20, 20, furnitureData, 2);
        }            
    }

    public void ClearGrid()
    {
        int numObjects = 0;

        foreach (var pos in furnitureData.GetAllOccupiedPositions()) numObjects++;

        for (int i = 0; i < numObjects; i++)
        {
            foreach (var pos in furnitureData.GetAllOccupiedPositions())
            {
                objectPlacer.RemoveObjectAt(furnitureData.GetRepresentationIndex(pos));
                furnitureData.RemoveObjectAt(pos);
                break;
            }
        }             
    }
}
