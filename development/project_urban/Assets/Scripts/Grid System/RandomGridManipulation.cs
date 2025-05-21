using System;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.PackageManager.UI;
using UnityEngine;
using Random = UnityEngine.Random;

public class RandomGridManipulation
{
	private Grid grid;
	private ObjectPlacer objectPlacer;
	[SerializeField] private ObjectsDatabaseSO dataBase;
	
	public RandomGridManipulation(Grid grid, ObjectPlacer objectPlacer, ObjectsDatabaseSO dataBase)
	{
		this.grid = grid;
		this.objectPlacer = objectPlacer;
		this.dataBase = dataBase;
	}

	public void RandomPlace(int toPlace, int maxX, int maxZ, GridData gridData, int objectID)
	{
		// create an empty list
		List<Vector3Int> validPositions = new();

		// add all valid positions
		for (int x = -maxX / 2; x < maxX / 2; x++)
		{
			for (int z = -maxZ / 2; z < maxZ / 2; z++)
			{
				Vector3Int testPos = new Vector3Int(x, 0, z);
				if (gridData.CanPlaceObjectAt(testPos, dataBase.objectsData[objectID].Size))
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
			int index = objectPlacer.PlaceObject(dataBase.objectsData[objectID].Prefab, dataBase.objectsData[objectID].UIWindow, grid.CellToWorld(pos));
			gridData.AddObjectAt(pos, dataBase.objectsData[objectID].Size, objectID, index);
		}
	}

    public void RandomWheightedPlace(int toPlace, int maxX, int maxZ, GridData gridData, int objectID)
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
                if (gridData.CanPlaceObjectAt(testPos, dataBase.objectsData[objectID].Size))
                {
					int numNeighbors = 0;

					if (!gridData.CanPlaceObjectAt(testPos + new Vector3Int(1, 0, 0), dataBase.objectsData[objectID].Size)) numNeighbors++;
					if (!gridData.CanPlaceObjectAt(testPos + new Vector3Int(-1, 0, 0), dataBase.objectsData[objectID].Size)) numNeighbors++;
                    if (!gridData.CanPlaceObjectAt(testPos + new Vector3Int(0, 0, 1), dataBase.objectsData[objectID].Size)) numNeighbors++;
                    if (!gridData.CanPlaceObjectAt(testPos + new Vector3Int(0, 0, -1), dataBase.objectsData[objectID].Size)) numNeighbors++;

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

			int index = objectPlacer.PlaceObject(dataBase.objectsData[objectID].Prefab, dataBase.objectsData[objectID].UIWindow, grid.CellToWorld(pos));
			gridData.AddObjectAt(pos, dataBase.objectsData[objectID].Size, objectID, index);

			placed++;
		}	
    }

    public void RandomUpgrade(int toUpgrade, int maxX, int maxZ, GridData gridData, int objectID)
	{
        // empty list for all valid positions
        List<Vector3Int> validUpgradeables = new();

		// add all valid upgradeable positions
		foreach (var pos in gridData.GetAllOccupiedPositions())
		{
			if (gridData.GetObjectIDAt(pos) == objectID - 1) validUpgradeables.Add(pos);
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

            int index = objectPlacer.PlaceObject(dataBase.objectsData[objectID].Prefab, dataBase.objectsData[objectID].UIWindow, grid.CellToWorld(pos));
            gridData.AddObjectAt(pos, dataBase.objectsData[objectID].Size, objectID, index);
        }
	}

    public void RandomWheightedUpgrade(int toUpgrade, int maxX, int maxZ, GridData gridData, int objectID)
    {

        // empty list for all valid positions and all fully encased positions
		List<Vector3Int> upgradeables = new();
        List<Vector3Int> validUpgradeables = new();
        List<Vector3Int> encasedUpgradeables = new();

        var upgradeablesLists = new (int weight, List<Vector3Int> list)[]
        {
            (1, new List<Vector3Int>()), // 4 valid neighbors
			(20, new List<Vector3Int>()), // 5 valid neighbor
			(8, new List<Vector3Int>()), // 6 valid neighbors
			(30, new List<Vector3Int>()), // 7 valid neighbors
			(40, new List<Vector3Int>()), // 8 valid neighbors
		};

        foreach (var pos in gridData.GetAllOccupiedPositions())
        {
            if (gridData.GetObjectIDAt(pos) == objectID - 1) upgradeables.Add(pos);
        }

		foreach (var pos in upgradeables)
		{
			int numDirectNeighbors = 0;

			if (!gridData.CanPlaceObjectAt(pos + new Vector3Int(1, 0, 0), dataBase.objectsData[objectID].Size) && 
				gridData.GetObjectIDAt(pos + new Vector3Int(1, 0, 0)) >= objectID - 1) numDirectNeighbors++;
			if (!gridData.CanPlaceObjectAt(pos + new Vector3Int(-1, 0, 0), dataBase.objectsData[objectID].Size) &&
                gridData.GetObjectIDAt(pos + new Vector3Int(-1, 0, 0)) >= objectID - 1) numDirectNeighbors++;
			if (!gridData.CanPlaceObjectAt(pos + new Vector3Int(0, 0, 1), dataBase.objectsData[objectID].Size) &&
                gridData.GetObjectIDAt(pos + new Vector3Int(0, 0, 1)) >= objectID - 1) numDirectNeighbors++;
			if (!gridData.CanPlaceObjectAt(pos + new Vector3Int(0, 0, -1), dataBase.objectsData[objectID].Size) &&
                gridData.GetObjectIDAt(pos + new Vector3Int(0, 0, -1)) >= objectID - 1) numDirectNeighbors++;

			if (numDirectNeighbors == 4)
			{
				validUpgradeables.Add(pos);
			}
		}    

        foreach (var pos in validUpgradeables)
		{
            int numNeighbors = 0;

            if (!gridData.CanPlaceObjectAt(pos + new Vector3Int(1, 0, 1), dataBase.objectsData[objectID].Size) &&
                gridData.GetObjectIDAt(pos + new Vector3Int(1, 0, 1)) >= objectID - 1) numNeighbors++;
            if (!gridData.CanPlaceObjectAt(pos + new Vector3Int(-1, 0, 1), dataBase.objectsData[objectID].Size) &&
                gridData.GetObjectIDAt(pos + new Vector3Int(-1, 0, 1)) >= objectID - 1) numNeighbors++;
            if (!gridData.CanPlaceObjectAt(pos + new Vector3Int(-1, 0, 1), dataBase.objectsData[objectID].Size) &&
                gridData.GetObjectIDAt(pos + new Vector3Int(-1, 0, 1)) >= objectID - 1) numNeighbors++;
            if (!gridData.CanPlaceObjectAt(pos + new Vector3Int(-1, 0, -1), dataBase.objectsData[objectID].Size) &&
                gridData.GetObjectIDAt(pos + new Vector3Int(-1, 0, -1)) >= objectID - 1) numNeighbors++;

            upgradeablesLists[numNeighbors].list.Add(pos);
        }

        // shuffle the list
        foreach (var list in  upgradeablesLists)
			Shuffle(list.list);

        // upgrade objects in positions based on weighted probabilities
        int upgraded = 0;
        while (upgraded < toUpgrade)
        {
            int probabilityMax = 0;
            foreach (var item in upgradeablesLists)
                if (item.list.Count != 0)
                    probabilityMax += item.weight;

            if (probabilityMax == 0) break;

            int rnd = Random.Range(0, probabilityMax);
            int cumulative = 0;
            int chosenIndex = -1;

            // randomely choose a list
            for (int i = 0; i < upgradeablesLists.Length; i++)
            {
                if (upgradeablesLists[i].list.Count == 0) continue;

                cumulative += upgradeablesLists[i].weight;
                if (rnd < cumulative)
                {
                    chosenIndex = i;
                    break;
                }
            }

            // safety check
            if (chosenIndex == -1)
                continue;

            Vector3Int pos = upgradeablesLists[chosenIndex].list[0];
            int representationIndex = gridData.GetRepresentationIndex(pos);
            upgradeablesLists[chosenIndex].list.RemoveAt(0);

            objectPlacer.RemoveObjectAt(representationIndex);
            gridData.RemoveObjectAt(pos);

            int index = objectPlacer.PlaceObject(dataBase.objectsData[objectID].Prefab, dataBase.objectsData[objectID].UIWindow, grid.CellToWorld(pos));
            gridData.AddObjectAt(pos, dataBase.objectsData[objectID].Size, objectID, index);

            upgraded++;
        }
    }

	public void GenerateRiver(int maxX, int maxZ, GridData gridData, int objectID) 
	{
		int borderLength = maxX * 2 + maxZ * 2 - 4;
		int innerBorderLength = maxX + maxZ - 4;
		int steps;
		int index;

        steps = Random.Range(0, borderLength);
        Vector3Int pos0 = iterateOverEdge(steps, maxX - 1, 0, 0) + new Vector3Int(- maxX / 2, 0,- maxZ / 2);

		steps = Random.Range((int)(0 + borderLength / 3) , (int)(borderLength / 3 * 2)) + steps;
        Vector3Int pos3 = iterateOverEdge(steps, maxX - 1, 0, 0) + new Vector3Int(-maxX / 2, 0, -maxZ / 2);

        steps = Random.Range(0, innerBorderLength);
		Vector3Int pos2 = iterateOverEdge(steps, maxX / 2 - 1, 0, 0) + new Vector3Int(- maxX / 2 + maxX / 4, 0,- maxZ / 2 + maxZ / 4);

        steps = Random.Range(steps + maxX / 4, innerBorderLength - maxX / 2);
        Vector3Int pos1 = iterateOverEdge(steps, maxX / 2 - 1, 0, 0) + new Vector3Int(- maxX / 2 + maxX / 4, 0, -maxZ / 2 + maxZ / 4);

        double distanceTo1 = Math.Sqrt((pos0.x - pos1.x) * (pos0.x - pos1.x) + (pos0.z - pos1.z) * (pos0.z - pos1.z));
		double distanceTo2 = Math.Sqrt((pos0.x - pos2.x) * (pos0.x - pos2.x) + (pos0.z - pos2.z) * (pos0.z - pos2.z));

		Vector3Int[] riverPoints = new Vector3Int[3];
		riverPoints[2] = pos3;

        if (distanceTo1 > distanceTo2)
		{
			riverPoints[0] = pos2;
			riverPoints[1] = pos1;
		} 
		else
		{
			riverPoints[0] = pos1;
			riverPoints[1] = pos2;
		}

		Vector3Int riverFlow = pos0;

		for (int i = 0; i < 3; i++)
		{
			while (riverFlow != riverPoints[i])
			{
				if (gridData.CanPlaceObjectAt(riverFlow, dataBase.objectsData[objectID].Size))
                {
                    index = objectPlacer.PlaceObject(dataBase.objectsData[objectID].Prefab, dataBase.objectsData[objectID].UIWindow, grid.CellToWorld(riverFlow));
                    gridData.AddObjectAt(riverFlow, dataBase.objectsData[objectID].Size, objectID, index);
                }

                int dx = Math.Abs(riverFlow.x - riverPoints[i].x);
                int dz = Math.Abs(riverFlow.z - riverPoints[i].z);

                bool moveX = dx > dz || (dx == dz && Random.value > 0.5f);

                if (moveX)
                {
                    riverFlow.x += (riverFlow.x < riverPoints[i].x) ? 1 : -1;
                }
                else
                {
                    riverFlow.z += (riverFlow.z < riverPoints[i].z) ? 1 : -1;
                }
            }
		}
		if (gridData.CanPlaceObjectAt(riverFlow, dataBase.objectsData[objectID].Size))
		{
			index = objectPlacer.PlaceObject(dataBase.objectsData[objectID].Prefab, dataBase.objectsData[objectID].UIWindow, grid.CellToWorld(riverFlow));
			gridData.AddObjectAt(riverFlow, dataBase.objectsData[objectID].Size, objectID, index);
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

    private Vector3Int iterateOverEdge(int steps, int edgeLength, int startX, int startY)
    {
        int perimeter = edgeLength * 4;
        steps %= perimeter;

        int x = startX;
        int z = startY;

        for (int i = 0; i < steps; i++)
        {
            if (z == 0 && x < edgeLength)
            {
                x++;
            }
            else if (x == edgeLength && z < edgeLength)
            {
                z++;
            }
            else if (z == edgeLength && x > 0)
            {
                x--;
            }
            else if (x == 0 && z > 0)
            {
                z--;
            }
        }

        return new Vector3Int(x, 0, z);
    }
}
