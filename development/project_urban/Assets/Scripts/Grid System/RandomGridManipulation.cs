using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class RandomGridManipulation
{
    private Grid grid;
	private GridData gridData;
	private ObjectPlacer objectPlacer;
	private GameObject prefab;
	private GameObject uiWindow;
	private Vector2Int objectSize;
	private int objectID = 0;

	public RandomGridManipulation(Grid grid, GridData gridData, ObjectPlacer objectPlacer, GameObject prefab, GameObject uiWindow, Vector2Int objectSize)
	{
		this.grid = grid;
		this.gridData = gridData;
		this.objectPlacer = objectPlacer;
		this.prefab = prefab;
		this.uiWindow = uiWindow;
		this.objectSize = objectSize;
	}

	public void RandomPlace(int toPlace, int maxX, int maxZ)
	{
		// create an empty list
		List<Vector3Int> validPositions = new();

		// add all valid positions
		for (int x = -maxX / 2; x < maxX / 2; x++)
		{
			for (int z = -maxZ / 2; z < maxZ / 2; z++)
			{
				Vector3Int testPos = new Vector3Int(x, 0, z);
				if (gridData.CanPlaceObjectAt(testPos, objectSize))
				{
					validPositions.Add(testPos);
				}
			}
		}

		// shuffle the list
		Shuffle(validPositions);

		// take the first x positions according to toPlace if possible
		int validAmount = Mathf.Min(toPlace, validPositions.Count);
		for (int i = 0; i < validAmount; i++)
		{
			Vector3Int pos = validPositions[i];
			int index = objectPlacer.PlaceObject(prefab, uiWindow, grid.CellToWorld(pos));
			gridData.AddObjectAt(pos, objectSize, objectID, index);
		}
	}

    public void RandomWheightedPlace(int toPlace, int maxX, int maxZ)
    {

		// leere Listen für die freien Positionen
		var placeablesLists = new (int weight, List<Vector3Int> list)[]
		{
			(1, new List<Vector3Int>()), // 0 neighbors
			(20, new List<Vector3Int>()), // 1 neighbor
			(8, new List<Vector3Int>()), // 2 neighbors
			(30, new List<Vector3Int>()), // 3 neighbors
			(40, new List<Vector3Int>()), // 4 neighbors
		};

        // add all valid positions to the lists based on number of neighbors
        for (int x = -maxX / 2; x < maxX / 2; x++)
        {
            for (int z = -maxZ / 2; z < maxZ / 2; z++)
            {
                Vector3Int testPos = new Vector3Int(x, 0, z);
                if (gridData.CanPlaceObjectAt(testPos, objectSize))
                {
					int numNeighbors = 0;

					if (!gridData.CanPlaceObjectAt(testPos + new Vector3Int(1, 0, 0), objectSize)) numNeighbors++;
					if (!gridData.CanPlaceObjectAt(testPos + new Vector3Int(-1, 0, 0), objectSize)) numNeighbors++;
                    if (!gridData.CanPlaceObjectAt(testPos + new Vector3Int(0, 0, 1), objectSize)) numNeighbors++;
                    if (!gridData.CanPlaceObjectAt(testPos + new Vector3Int(0, 0, -1), objectSize)) numNeighbors++;

					placeablesLists[numNeighbors].list.Add(testPos);
                }
            }
        }

		// shuffle the lists
		foreach (var list in placeablesLists)
			Shuffle(list.list);

		// place objects in positions based on weighted probabilities
		int placed = 0;
		while (placed < toPlace)
		{
			int probabilityMax = 0;
			foreach (var item in placeablesLists)
				if (item.list.Count != 0)
					probabilityMax += item.weight;

			if (probabilityMax == 0) break;

			int rnd = Random.Range(0, probabilityMax);
			int cumulative = 0;
			int chosenIndex = -1;

			// randomely choose a list
			for (int i = 0; i < placeablesLists.Length; i++) 
			{
				if (placeablesLists[i].list.Count == 0) continue;

				cumulative += placeablesLists[i].weight;
				if (rnd < cumulative)
				{
					chosenIndex = i;
					break;
				}
			}

			// safety check
			if (chosenIndex == -1)
				continue;

			Vector3Int pos = placeablesLists[chosenIndex].list[0];
			placeablesLists[chosenIndex].list.RemoveAt(0);

			int index = objectPlacer.PlaceObject(prefab, uiWindow, grid.CellToWorld(pos));
			gridData.AddObjectAt(pos, objectSize, objectID, index);

			placed++;
		}	
    }

    public void RandomUpgrade(int toUpgrade, int maxX, int maxZ)
	{
        // empty list for all valid positions
        List<Vector3Int> validUpgradeables = new();

		// add all valid upgradeable positions
		foreach (var pos in gridData.GetAllOccupiedPositions())
		{
			if (gridData.GetObjectIDAt(pos) == 0) validUpgradeables.Add(pos);
		}

		// shuffle the list
		Shuffle(validUpgradeables);

		// replace the first n objects with objects of ID 2
		int validAmount = Mathf.Min(toUpgrade, validUpgradeables.Count);
		for (int i = 0; i < validAmount; i++)
		{
			Vector3Int pos = validUpgradeables[i];
			int representationIndex = gridData.GetRepresentationIndex(pos);
			
			objectPlacer.RemoveObjectAt(representationIndex);
			gridData.RemoveObjectAt(pos);

            int index = objectPlacer.PlaceObject(prefab, uiWindow, grid.CellToWorld(pos));
            gridData.AddObjectAt(pos, objectSize, 2, index);
        }
	}

    public void RandomWheightedUpgrade(int toUpgrade, int maxX, int maxZ)
    {

        // empty list for all valid positions and all fully encased positions
        List<Vector3Int> validUpgradeables = new();
        List<Vector3Int> encasedUpgradeables = new();

        foreach (var pos in gridData.GetAllOccupiedPositions())
        {
            if (gridData.GetObjectIDAt(pos) == 0) validUpgradeables.Add(pos);
        }

		foreach (var pos in validUpgradeables)
		{
            bool encased = true;

            for (int offsetX = -1; offsetX < 2; offsetX++)
            {
                for (int offsetZ = -1; offsetZ < 2; offsetZ++)
                {
                    if (gridData.CanPlaceObjectAt(pos + new Vector3Int(offsetX, 0, offsetZ), objectSize))
                    {
                        encased = false;
                        break;
                    }
                }

                if (!encased) break;
            }

            if (encased) encasedUpgradeables.Add(pos);
        }

        // shuffle the list
        Shuffle(encasedUpgradeables);

        // replace the first n objects with objects of ID 2
        int validAmount = Mathf.Min(toUpgrade, encasedUpgradeables.Count);
        for (int i = 0; i < validAmount; i++)
        {
            Vector3Int pos = encasedUpgradeables[i];
            int representationIndex = gridData.GetRepresentationIndex(pos);

            objectPlacer.RemoveObjectAt(representationIndex);
            gridData.RemoveObjectAt(pos);

            int index = objectPlacer.PlaceObject(prefab, uiWindow, grid.CellToWorld(pos));
            gridData.AddObjectAt(pos, objectSize, 2, index);
        }
    }

    // Fisher-Yates shuffle function
    private void Shuffle(List<Vector3Int> list) 
	{
		for (int i = 0; i < list.Count; i++) 
		{
			int rand = Random.Range(i, list.Count);
			(list[i], list[rand]) = (list[rand], list[i]);
		}
	}
}
