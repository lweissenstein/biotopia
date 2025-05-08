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
	private Vector2Int objectSize;
	private int objectID = 0;

	public RandomGridManipulation(Grid grid, GridData gridData, ObjectPlacer objectPlacer, GameObject prefab, Vector2Int objectSize)
	{
		this.grid = grid;
		this.gridData = gridData;
		this.objectPlacer = objectPlacer;
		this.prefab = prefab;
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
			int index = objectPlacer.PlaceObject(prefab, grid.CellToWorld(pos));
			gridData.AddObjectAt(pos, objectSize, objectID, index);
		}
	}

    public void RandomWheightedPlace(int toPlace, int maxX, int maxZ)
    {
        // create an empty list
        List<Vector3Int> noNeighbor = new();
		List<Vector3Int> hasNeighbor = new();

        // add all valid positions
        for (int x = -maxX / 2; x < maxX / 2; x++)
        {
            for (int z = -maxZ / 2; z < maxZ / 2; z++)
            {
                Vector3Int testPos = new Vector3Int(x, 0, z);
                if (gridData.CanPlaceObjectAt(testPos, objectSize))
                {
					if (!gridData.CanPlaceObjectAt(testPos + new Vector3Int(1, 0, 0), objectSize) ||
                        !gridData.CanPlaceObjectAt(testPos + new Vector3Int(-1, 0, 0), objectSize) ||
                        !gridData.CanPlaceObjectAt(testPos + new Vector3Int(0, 0, 1), objectSize) ||
                        !gridData.CanPlaceObjectAt(testPos + new Vector3Int(0, 0, -1), objectSize))
					{
                        hasNeighbor.Add(testPos);
                    } 
					else
					{
						noNeighbor.Add(testPos);
                    }
                }
            }
        }

        // shuffle the list
        Shuffle(noNeighbor);
        Shuffle(hasNeighbor);

		int rnd = 0;
		int noIndex = 0;
		int hasIndex = 0;

        // take the first x positions according to toPlace if possible
        int validAmount = Mathf.Min(toPlace, noNeighbor.Count + hasNeighbor.Count);
        for (int i = 0; i < validAmount; i++)
        {
			if (hasNeighbor.Count == 0)
			{
                Vector3Int pos = noNeighbor[noIndex];
				noIndex++;
                int index = objectPlacer.PlaceObject(prefab, grid.CellToWorld(pos));
                gridData.AddObjectAt(pos, objectSize, objectID, index);
			}
			else if (noNeighbor.Count == 0)
            {
                Vector3Int pos = hasNeighbor[hasIndex];
                hasIndex++;
                int index = objectPlacer.PlaceObject(prefab, grid.CellToWorld(pos));
                gridData.AddObjectAt(pos, objectSize, objectID, index);
            }
			else
			{
				rnd = Random.Range(0, 10);

				if (rnd > 1)
				{
					Vector3Int pos = hasNeighbor[hasIndex];
					hasIndex++;
					int index = objectPlacer.PlaceObject(prefab, grid.CellToWorld(pos));
					gridData.AddObjectAt(pos, objectSize, objectID, index);
				}
				else
				{
					Vector3Int pos = noNeighbor[noIndex];
					noIndex++;
					int index = objectPlacer.PlaceObject(prefab, grid.CellToWorld(pos));
					gridData.AddObjectAt(pos, objectSize, objectID, index);
				}
			}
        }
    }

    public void RandomUpgrade(int toUpgrade, int maxX, int maxZ)
	{
		// create an empty list
		List<Vector3Int> validUpgradeables = new();

		// add all valid upgradeable positions
		foreach (var pos in gridData.GetAllOccupiedPositions())
		{
			if (gridData.GetObjectIDAt(pos) == 0)
			{
				validUpgradeables.Add(pos);
			}		
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

            int index = objectPlacer.PlaceObject(prefab, grid.CellToWorld(pos));
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
