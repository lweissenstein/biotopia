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

    public BuildingSelectionManager buildingSelectionManager;
    //IBuildingState buildingState;dsad

    //[SerializeField] private SoundFeedback soundFeedback;

    private RandomGridManipulation randomPlacer;

    private int smallPlaceTimer = 1;
    private int smallToMediumTimer = 3;
    private int mediumToLargeTimer = 5;
    private int parkTimer = 5;

    private Timer _timer = new();

    [SerializeField] private GameObject gridObject;

    int gridSizeX;
    int gridSizeZ;

    private void Awake()
    {
        if (gridObject != null)
        {
            gridSizeX = Mathf.RoundToInt(gridObject.transform.localScale.x*10);
            gridSizeZ = Mathf.RoundToInt(gridObject.transform.localScale.z*10);
        }
        Debug.Log($"Grid size: {gridSizeX}x{gridSizeZ} cells");
    }

    private void Start()
    {

        //StopPlacement();
        // no idea why we are using a Y axis btw
        var gridSize = new Vector3Int(gridSizeX, 1, gridSizeZ);
        floorData = GridData.New(gridSize);
        furnitureData = GridData.New(gridSize);
        randomPlacer = new RandomGridManipulation(grid, objectPlacer, database);
        if (GameState.isTutorial)
        {
            randomPlacer.PlaceTutorialBuildings(furnitureData);
            return;
        }
        randomPlacer.RandomPlaceOnBorder(gridSizeX, gridSizeZ, furnitureData, 5, 0.8);
        randomPlacer.RandomPlace(1, gridSizeX / 4, gridSizeZ / 4, furnitureData, 6);
        randomPlacer.GenerateRiver(gridSizeX, gridSizeZ, furnitureData, 3);
        randomPlacer.RandomPlace(15, gridSizeX / 3, gridSizeZ / 3, furnitureData, 0);
        randomPlacer.RandomUpgrade(3, gridSizeX, gridSizeZ, furnitureData, 1);
    }

  
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

    public void PingSuperMarket(Vector3 pos)
    {
        randomPlacer.PingSuperMarket(pos, furnitureData);
    }

    // generates a river and creates a base city using RandomPlace and RandomUpgrade in the center
    public void createBaseCity()
    {
        randomPlacer.GenerateRiver(gridSizeX, gridSizeZ, furnitureData, 3);
        randomPlacer.RandomWheightedPlace(15, gridSizeX / 3, gridSizeZ / 3, furnitureData, 0);
        randomPlacer.RandomUpgrade(3, gridSizeX, gridSizeZ, furnitureData, 1);
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
                randomPlacer.RandomWheightedPlace(1, gridSizeX, gridSizeZ, furnitureData, 0);
                smallPlaceTimer = 1;
                buildingSelectionManager = FindAnyObjectByType<BuildingSelectionManager>();
                Debug.Log("Building Selection Manager found: " + (buildingSelectionManager != null));
                randomPlacer.PingAll(buildingSelectionManager.superMarkets, furnitureData);

            }
            if (smallToMediumTimer == 0)
            {
                randomPlacer.RandomWheightedUpgrade(2, gridSizeX, gridSizeZ, furnitureData, 1);
                smallToMediumTimer = 8;
            }
            if (mediumToLargeTimer == 0)
            {
                randomPlacer.RandomWheightedUpgrade(1, gridSizeX, gridSizeZ, furnitureData, 2);
                mediumToLargeTimer = 15;
            }
            if (parkTimer == 0)
            {
                randomPlacer.RandomParkPlace(gridSizeX, gridSizeZ, furnitureData);
                parkTimer = 10;
            }
        }

        smallPlaceTimer--;
        smallToMediumTimer--;
        mediumToLargeTimer--;
        parkTimer--;      
    }

    void Update()
    {
        if (!GameState.isTutorial)
            _timer.OncePerSecond(perSecondUpdate);
        else if (GameState.isTutorial && GameState.allowBuildingSpawn)
        {
            _timer.OncePerIntervallMilliseconds(() =>
            {
                    perSecondUpdate();
            }, 20);
        }
    }

}
