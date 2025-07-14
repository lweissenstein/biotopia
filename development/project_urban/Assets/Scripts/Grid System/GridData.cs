#nullable enable
using System;
using System.Collections.Generic;
using UnityEngine;

public class GridData
{
    // negative and positive, and in 3 directions == 6 dimensions
    private readonly PlacementData?[][][][][][] _data;
    private bool _isCached;
    private List<Vector3Int> _cachedOccupiedPositions;

    public static GridData New(Vector3Int size)
    {
        var data = new PlacementData?[2][][][][][];

        for (var xd = 0; xd < data.Length; ++xd)
        {
            data[xd] = new PlacementData?[size.x][][][][];
            for (var x = 0; x < data[xd].Length; ++x)
            {
                data[xd][x] = new PlacementData?[2][][][];
                for (var yd = 0; yd < data[xd][x].Length; ++yd)
                {
                    data[xd][x][yd] = new PlacementData?[size.y][][];
                    for (var y = 0; y < data[xd][x][yd].Length; ++y)
                    {
                        data[xd][x][yd][y] = new PlacementData?[2][];
                        for (var zd = 0; zd < data[xd][x][yd][y].Length; ++zd)
                        {
                            data[xd][x][yd][y][zd] = new PlacementData?[size.z];
                            for (var z = 0; z < data[xd][x][yd][y][zd].Length; ++z)
                            {
                                data[xd][x][yd][y][zd][z] = null;
                            }
                        }
                    }
                }
            }
        }

        return new GridData(data);
    }

    public GridData(PlacementData?[][][][][][] data)
    {
        _data = data;
        _isCached = false;
        _cachedOccupiedPositions = new List<Vector3Int>();
    }

    private PlacementData? Get(Vector3Int v)
    {
        return _data[v.x < 0 ? 0 : 1]
            [v.x < 0 ? -v.x : v.x]
            [v.y < 0 ? 0 : 1]
            [v.y < 0 ? -v.y : v.y]
            [v.z < 0 ? 0 : 1]
            [v.z < 0 ? -v.z : v.z];
    }

    private void Put(Vector3Int v, PlacementData? item)
    {
        _isCached = false;
        _data[v.x < 0 ? 0 : 1]
            [v.x < 0 ? -v.x : v.x]
            [v.y < 0 ? 0 : 1]
            [v.y < 0 ? -v.y : v.y]
            [v.z < 0 ? 0 : 1]
            [v.z < 0 ? -v.z : v.z] = item;
    }


    public void AddObjectAt(
        Vector3Int gridPosition,
        Vector2Int objectSize,
        int ID,
        int placedObjectIndex
    )
    {
        List<Vector3Int> positionToOccupy = CalculatePositions(gridPosition, objectSize);
        foreach (var pos in positionToOccupy)
        {
            if (Get(pos) is not null) throw new Exception($"Dictionary already contains cell position {pos}");
            Put(pos, new PlacementData(positionToOccupy, ID, placedObjectIndex));
        }
    }

    private List<Vector3Int> CalculatePositions(Vector3Int gridPosition, Vector2Int objectSize)
    {
        List<Vector3Int> returnValue = new();
        for (int x = 0; x < objectSize.x; x++)
        {
            for (int y = 0; y < objectSize.y; y++)
            {
                returnValue.Add(gridPosition + new Vector3Int(x, 0, y));
            }
        }

        return returnValue;
    }

    public bool CanPlaceObjectAt(Vector3Int gridPosition, Vector2Int objectSize)
    {
        List<Vector3Int> positionToOccupy = CalculatePositions(gridPosition, objectSize);
        foreach (var pos in positionToOccupy)
        {
            if (Get(pos) is not null)
                return false;
        }

        return true;
    }


    internal int GetRepresentationIndex(Vector3Int pos)
    {
        if (Get(pos) is null) return -1;
        return Get(pos)!.PlacedObjectIndex;
    }

    internal void RemoveObjectAt(Vector3Int pos)
    {
        var obj = Get(pos);
        if (obj is null) return;
        foreach (var p in obj.occupiedPositions)
        {
            Put(p, null);
        }
    }

    public void UpdateObjectIDAt(Vector3Int pos, int newID)
    {
        var obj = Get(pos);
        if (obj is null) return;
        obj.ID = newID;
        Put(pos, obj);
    }

    // returns the ID of the prefab
    public int GetObjectIDAt(Vector3Int pos)
    {
        var obj = Get(pos);
        if (obj is null) return -1;
        return obj.ID;
    }

    // returns all occupied grid positions
    public IEnumerable<Vector3Int> GetAllOccupiedPositions()
    {
        if (!_isCached)
        {
            _cachedOccupiedPositions = CalculateAllOccupiedPositions();
            _isCached = true;
        }

        return _cachedOccupiedPositions;
    }

    private List<Vector3Int> CalculateAllOccupiedPositions()
    {
        var occupiedPostions = new List<Vector3Int>();

        for (var xd = 0; xd < _data.Length; ++xd)
        {
            for (var x = 0; x < _data[xd].Length; ++x)
            {
                for (var yd = 0; yd < _data[xd][x].Length; ++yd)
                {
                    for (var y = 0; y < _data[xd][x][yd].Length; ++y)
                    {
                        for (var zd = 0; zd < _data[xd][x][yd][y].Length; ++zd)
                        {
                            for (var z = 0; z < _data[xd][x][yd][y][zd].Length; ++z)
                            {
                                if (_data[xd][x][yd][y][zd][z] is not null)
                                {
                                    occupiedPostions.Add(
                                        new Vector3Int(
                                            x: xd == 0 ? -x : x,
                                            y: yd == 0 ? -y : y,
                                            z: zd == 0 ? -z : z
                                        )
                                    );
                                }
                            }
                        }
                    }
                }
            }
        }

        return occupiedPostions;
    }
}

public class PlacementData
{
    public List<Vector3Int> occupiedPositions;
    public int ID { get; internal set; }
    public int PlacedObjectIndex { get; private set; }

    public PlacementData(List<Vector3Int> occupiedPositions, int iD, int placedObjectIndex)
    {
        this.occupiedPositions = occupiedPositions;
        ID = iD;
        PlacedObjectIndex = placedObjectIndex;
    }
}