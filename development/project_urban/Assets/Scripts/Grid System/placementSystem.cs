using UnityEngine;
using Timer = Util.Timer;

public class PlacementSystem : MonoBehaviour
{
    //[SerializeField] private MouseInputManager inputManager;
    [SerializeField] Grid grid;

    [SerializeField] private ObjectsDatabaseSO database;

    [SerializeField] private GameObject gridVisualzation;

    //[SerializeField] private PreviewSystem preview;

    private GridData floorData, furnitureData;

    private Vector3Int lastDetectedPosition = Vector3Int.zero;

    [SerializeField] private ObjectPlacer objectPlacer;

    //IBuildingState buildingState;dsad

    //[SerializeField] private SoundFeedback soundFeedback;

    private RandomGridManipulation randomPlacer;

    private int smallPlaceTimer = 1;
    private int smallToMediumTimer = 3;
    private int mediumToLargeTimer = 5;

    private Timer _timer = new();

    [SerializeField] private GameObject gridObject;

    int gridSizeX;
    int gridSizeZ;
    int freeTiles;

    private void Awake()
    {
        if (gridObject != null)
        {
            gridSizeX = Mathf.RoundToInt(gridObject.transform.localScale.x*10);
            gridSizeZ = Mathf.RoundToInt(gridObject.transform.localScale.z*10);
            freeTiles = gridSizeX * gridSizeZ;
        }
        Debug.Log($"Grid size: {gridSizeX}x{gridSizeZ} cells");
    }

    private void Start()
    {
        //StopPlacement();
        floorData = new();
        furnitureData = new();
        randomPlacer = new RandomGridManipulation(grid, objectPlacer, database);

        randomPlacer.GenerateRiver(gridSizeX, gridSizeZ, furnitureData, 3);
        randomPlacer.RandomPlaceOnBorder(gridSizeX, gridSizeZ, furnitureData, 5, 0.8);
        randomPlacer.RandomPlace(1, gridSizeX / 4, gridSizeZ / 4, furnitureData, 6);
        randomPlacer.RandomPlace(15, gridSizeX / 3, gridSizeZ / 3, furnitureData, 0);
        randomPlacer.RandomUpgrade(3, gridSizeX, gridSizeZ, furnitureData, 1);
    }

    //public void StartPlacement(int ID)
    //{
    //    StopPlacement();
    //    gridVisualzation.SetActive(true);
    //    buildingState = new PlacementState(ID,
    //                                       grid,
    //                                       preview,
    //                                       database,
    //                                       floorData,
    //                                       furnitureData,
    //                                       objectPlacer,
    //                                       soundFeedback);
    //    inputManager.OnClick += PlaceStructure;
    //    inputManager.OnExit += StopPlacement;
    //}

    //public void StartRemoving()
    //{
    //    StopPlacement();
    //    gridVisualzation.SetActive(true);
    //    buildingState = new RemovingState(grid,
    //                                      preview,
    //                                      floorData,
    //                                      furnitureData,
    //                                      objectPlacer);
    //    inputManager.OnClick += PlaceStructure;
    //    inputManager.OnExit += StopPlacement;
    //}

    //private void PlaceStructure()
    //{
    //    if (inputManager.IsPointerOverUI()) {return;}

    //    Vector3 mousePosition = inputManager.getSelectedMapPosition();
    //    Vector3Int gridPosition = grid.WorldToCell(mousePosition);

    //    buildingState.OnAction(gridPosition);
    //}

    ////private bool CheckPlacementValidity(Vector3Int gridPosition, int selectedObjectIndex)
    ////{
    ////    GridData selectedData = database.objectsData[selectedObjectIndex].ID == 4 ? floorData : furnitureData;

    ////    return selectedData.CanPlaceObjectAt(gridPosition, database.objectsData[selectedObjectIndex].Size);
    ////}

    //private void StopPlacement()
    //{
    //    if (buildingState == null)
    //        return;
    //    gridVisualzation.SetActive(false);
    //    buildingState.EndState();
    //    inputManager.OnClick -= PlaceStructure;
    //    inputManager.OnExit -= StopPlacement;
    //    lastDetectedPosition = Vector3Int.zero;
    //    buildingState = null;
    //}

    //private void Update()
    //{
    //    if (buildingState == null)
    //        return;
    //    Vector3 mousePosition = inputManager.getSelectedMapPosition();
    //    Vector3Int gridPosition = grid.WorldToCell(mousePosition);
    //    if(lastDetectedPosition != gridPosition)
    //    {
    //        buildingState.UpdateState(gridPosition);
    //        lastDetectedPosition = gridPosition;
    //    }
        
    //}

    // uses randomPlace to randomely place objects
    public void placeRandom()
    {
        randomPlacer.RandomPlace(5, gridSizeX, gridSizeZ, furnitureData, 0);
    }

    public void placeSingleRandom()
    {
        randomPlacer.RandomPlace(1, gridSizeX, gridSizeZ, furnitureData, 0);
    }

    // uses RandomWeightedPlace to randomely place objects
    public void weightedPlaceRandom()
    {
        randomPlacer.RandomWheightedPlace(5, gridSizeX, gridSizeZ, furnitureData, 0);
    }

    // uses RandomUpgrade to randomely upgrade objects
    public void upgradeRandom()
    {
        randomPlacer.RandomUpgrade(2, gridSizeX, gridSizeZ, furnitureData, 1);
        randomPlacer.RandomUpgrade(1, gridSizeX, gridSizeZ, furnitureData, 2);
    }

    public void upgradeSingleRandom()
    {
        int level = UnityEngine.Random.Range(1, 3); // Randomly choose a level between 1 and 2
        Debug.Log($"Upgrading to level {level}");
        randomPlacer.RandomUpgrade(1, gridSizeX, gridSizeZ, furnitureData, level);
    }

    // uses RandomWeightedUpgrade to randomely upgrade objects
    public void weightedUpgradeRandom()
    {
        int level = UnityEngine.Random.Range(1, 3);
        randomPlacer.RandomWheightedUpgrade(1, gridSizeX, gridSizeZ, furnitureData, level);
    }

    // grows the City using RandomWeightedPlace and RandomWeightedUpgrade
    public void SimulateGrowth()  //vorschlag: create random numbers for random amount of objects 
    {
        randomPlacer.RandomWheightedPlace(5, gridSizeX, gridSizeZ, furnitureData, 0);
        randomPlacer.RandomWheightedUpgrade(2, gridSizeX, gridSizeZ, furnitureData, 1);
        randomPlacer.RandomWheightedUpgrade(2, gridSizeX, gridSizeZ, furnitureData, 2);
    }

    // generates a river and creates a base city using RandomPlace and RandomUpgrade in the center
    public void createBaseCity()
    {
        randomPlacer.GenerateRiver(gridSizeX, gridSizeZ, furnitureData, 3);
        randomPlacer.RandomWheightedPlace(15, gridSizeX / 3, gridSizeZ / 3, furnitureData, 0);
        randomPlacer.RandomUpgrade(3, gridSizeX, gridSizeZ, furnitureData, 1);
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

    int getNumParks()
    {
        int numParks = 0;

        foreach (var pos in furnitureData.GetAllOccupiedPositions())
        {
            if (furnitureData.GetRepresentationIndex(pos) == 4) numParks++;
        }

        return numParks;
    }

    int getNumBuildings()
    {
        int numBuildings = 0;

        foreach (var pos in furnitureData.GetAllOccupiedPositions())
        {
            if (furnitureData.GetRepresentationIndex(pos) != 3 || furnitureData.GetRepresentationIndex(pos) != 4) numBuildings++;
        }

        return numBuildings;
    }

    public int GetFreeTiles()
    {
        int occupiedTiles = 0;

        foreach (Vector3Int pos in furnitureData.GetAllOccupiedPositions())
            occupiedTiles++;

        return gridSizeX * gridSizeZ - occupiedTiles;
    }

    public void perSecondUpdate()
    {
        bool empty = true;

        foreach (var pos in furnitureData.GetAllOccupiedPositions())
        {
            empty = false;
            break;
        }

        if (empty)
        {
            createBaseCity();
        }
        else
        {
            if (smallPlaceTimer == 0)
            {
                randomPlacer.RandomWheightedPlace(50, gridSizeX, gridSizeZ, furnitureData, 0);
                smallPlaceTimer = 1;
            }
            if (smallToMediumTimer == 0)
            {
                randomPlacer.RandomWheightedUpgrade(2, gridSizeX, gridSizeZ, furnitureData, 1);
                smallToMediumTimer = 1;
            }
            if (mediumToLargeTimer == 0)
            {
                randomPlacer.RandomWheightedUpgrade(2, gridSizeX, gridSizeZ, furnitureData, 2);
                mediumToLargeTimer = 1;
            }
        }

        smallPlaceTimer--;
        smallToMediumTimer--;
        mediumToLargeTimer--;

        if (getNumParks() < getNumBuildings() / 10 && GetFreeTiles() != 0) randomPlacer.RandomParkPlace(gridSizeX, gridSizeZ, furnitureData);
    }

    void Update()
    {
        _timer.OncePerSecond(perSecondUpdate);
    }

}
