using System.Collections.Generic;
using UnityEngine;

public class RandomPlacing
{
    private Grid grid;
	private GridData gridData;
	private ObjectPlacer objectPlacer;
	private GameObject prefab;
	private Vector2Int objectSize;
	private int objectID = 0;

	public RandomPlacing(Grid grid, GridData gridData, ObjectPlacer objectPlacer, GameObject prefab, Vector2Int objectSize)
	{
		this.grid = grid;
		this.gridData = gridData;
		this.objectPlacer = objectPlacer;
		this.prefab = prefab;
		this.objectSize = objectSize;
	}

	public void RandomPlace(int toPlace, int maxX, int maxZ)
	{
		// create and empty list
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
