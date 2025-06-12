using System;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.PackageManager.UI;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// Diese Klasse bietet Methoden, um Objekte zufällig oder gewichtet auf einem Grid zu platzieren, zu upgraden oder spezielle Strukturen wie Flüsse zu generieren.
/// Sie arbeitet eng mit GridData (Grid-Logik), ObjectPlacer (GameObject-Instanziierung) und ObjectsDatabaseSO (Objektdatenbank) zusammen.
/// </summary>
public class RandomGridManipulation
{
    // Referenz auf das Unity Grid-Objekt (für Umrechnung Grid <-> Welt)
    private Grid grid;
    // Verwaltet das Platzieren und Entfernen von GameObjects in der Szene
    private ObjectPlacer objectPlacer;
    // Datenbank mit allen platzierbaren Objekten (Prefabs, Größen, etc.)
    [SerializeField] private ObjectsDatabaseSO dataBase;

    /// <summary>
    /// Konstruktor: Initialisiert die Klasse mit Grid, ObjectPlacer und Datenbank.
    /// </summary>
    public RandomGridManipulation(Grid grid, ObjectPlacer objectPlacer, ObjectsDatabaseSO dataBase)
    {
        this.grid = grid;
        this.objectPlacer = objectPlacer;
        this.dataBase = dataBase;
    }

    /// <summary>
    /// Platziert eine bestimmte Anzahl von Objekten zufällig auf dem Grid.
    /// Nutzt GridData zur Prüfung, ob ein Platz frei ist, und ObjectPlacer zum Instanziieren.
    /// </summary>
    public void RandomPlace(int toPlace, int maxX, int maxZ, GridData gridData, int objectID)
    {
        // Liste für alle gültigen Positionen, an denen platziert werden kann
        List<Vector3Int> validPositions = new();

        // Alle möglichen Positionen im Grid durchgehen
        for (int x = -maxX / 2; x < maxX / 2; x++)
        {
            for (int z = -maxZ / 2; z < maxZ / 2; z++)
            {
                Vector3Int testPos = new Vector3Int(x, 0, z);
                // Prüfen, ob das Objekt an dieser Position platziert werden kann (GridData)
                if (gridData.CanPlaceObjectAt(testPos, dataBase.objectsData[objectID].Size))
                {
                    validPositions.Add(testPos);
                }
            }
        }

        // Liste zufällig mischen (Fisher-Yates)
        Shuffle(validPositions);

        // Die ersten n Positionen aus der gemischten Liste nehmen und platzieren
        int validAmount = Mathf.Min(toPlace, validPositions.Count);
        for (int i = 0; i < validAmount; i++)
        {
            Vector3Int pos = validPositions[i];
            // Objekt in der Szene platzieren (ObjectPlacer) und Index merken
            int index = objectPlacer.PlaceObject(dataBase.objectsData[objectID].Prefab, grid.CellToWorld(pos));
            // Platzierung im GridData speichern
            gridData.AddObjectAt(pos, dataBase.objectsData[objectID].Size, objectID, index);
        }
    }

    /// <summary>
    /// Platziert Objekte zufällig, aber mit Gewichtung je nach Nachbarschaft.
    /// Positionen mit mehr Nachbarn haben eine höhere oder niedrigere Wahrscheinlichkeit, gewählt zu werden.
    /// </summary>
    public void RandomWheightedPlace(int toPlace, int maxX, int maxZ, GridData gridData, int objectID)
    {
        // Listen für Positionen mit 0-4 Nachbarn, jeweils mit Gewicht
        var placeablesLists = new (int weight, List<Vector3Int> list)[]
        {
            (1, new List<Vector3Int>()),   // 0 Nachbarn
            (20, new List<Vector3Int>()),  // 1 Nachbar
            (8, new List<Vector3Int>()),   // 2 Nachbarn
            (30, new List<Vector3Int>()),  // 3 Nachbarn
            (40, new List<Vector3Int>()),  // 4 Nachbarn
        };

        // Alle möglichen Positionen im Grid durchgehen
        for (int x = -maxX / 2; x < maxX / 2; x++)
        {
            for (int z = -maxZ / 2; z < maxZ / 2; z++)
            {
                Vector3Int testPos = new Vector3Int(x, 0, z);
                if (gridData.CanPlaceObjectAt(testPos, dataBase.objectsData[objectID].Size))
                {
                    // Nachbarn zählen (direkt angrenzende Felder)
                    int numNeighbors = 0;
                    if (!gridData.CanPlaceObjectAt(testPos + new Vector3Int(1, 0, 0), dataBase.objectsData[objectID].Size)) numNeighbors++;
                    if (!gridData.CanPlaceObjectAt(testPos + new Vector3Int(-1, 0, 0), dataBase.objectsData[objectID].Size)) numNeighbors++;
                    if (!gridData.CanPlaceObjectAt(testPos + new Vector3Int(0, 0, 1), dataBase.objectsData[objectID].Size)) numNeighbors++;
                    if (!gridData.CanPlaceObjectAt(testPos + new Vector3Int(0, 0, -1), dataBase.objectsData[objectID].Size)) numNeighbors++;

                    // Position in die entsprechende Liste einfügen
                    placeablesLists[numNeighbors].list.Add(testPos);
                }
            }
        }

        // Jede Liste mischen
        foreach (var list in placeablesLists)
            Shuffle(list.list);

        // Platzieren nach gewichteter Wahrscheinlichkeit
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

            // Liste nach Gewicht zufällig auswählen
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

            // Sicherheitsabfrage
            if (chosenIndex == -1)
                continue;

            Vector3Int pos = placeablesLists[chosenIndex].list[0];
            placeablesLists[chosenIndex].list.RemoveAt(0);

            int index = objectPlacer.PlaceObject(dataBase.objectsData[objectID].Prefab, grid.CellToWorld(pos));
            gridData.AddObjectAt(pos, dataBase.objectsData[objectID].Size, objectID, index);

            placed++;
        }
    }

    /// <summary>
    /// Upgradet eine bestimmte Anzahl von Objekten auf dem Grid auf eine neue Objekt-ID.
    /// Entfernt das alte Objekt und platziert das neue an derselben Stelle.
    /// </summary>
    public void RandomUpgrade(int toUpgrade, int maxX, int maxZ, GridData gridData, int objectID)
    {
        // Liste für alle upgradebaren Positionen
        List<Vector3Int> validUpgradeables = new();

        // Alle belegten Positionen durchgehen (GridData)
        foreach (var pos in gridData.GetAllOccupiedPositions())
        {
            // Nur Objekte mit der vorherigen ID sind upgradebar
            if (gridData.GetObjectIDAt(pos) == objectID - 1) validUpgradeables.Add(pos);
        }

        // Liste mischen
        Shuffle(validUpgradeables);

        // Die ersten n Positionen upgraden
        int validAmount = Mathf.Min(toUpgrade, validUpgradeables.Count);
        for (int i = 0; i < validAmount; i++)
        {
            Vector3Int pos = validUpgradeables[i];
            int representationIndex = gridData.GetRepresentationIndex(pos);

            GameObject go = objectPlacer.placedGameObject[representationIndex];
            if (go != null)
            {
                var upgradeable = go.GetComponent<UpgradeableObject>();
                if (upgradeable != null)
                    upgradeable.UpgradeTo(objectID);

                var building = go.GetComponent<BuildingInstance>();
                if (building != null)
                    building.Upgrade();
            }

            gridData.UpdateObjectIDAt(pos, objectID);
        }
    }

    /// <summary>
    /// Wie RandomUpgrade, aber mit Gewichtung nach Nachbarschaft.
    /// Nur Objekte mit bestimmten Nachbarbedingungen werden bevorzugt geupgradet.
    /// </summary>
    public void RandomWheightedUpgrade(int toUpgrade, int maxX, int maxZ, GridData gridData, int objectID)
    {
        // Listen für upgradebare Positionen
        List<Vector3Int> upgradeables = new();
        List<Vector3Int> validUpgradeables = new();

        // Listen für verschiedene Nachbarschaftsbedingungen mit Gewicht
        var upgradeablesLists = new (int weight, List<Vector3Int> list)[]
        {
            (1, new List<Vector3Int>()),   // 4 gültige Nachbarn
            (20, new List<Vector3Int>()),  // 5 gültige Nachbarn
            (8, new List<Vector3Int>()),   // 6 gültige Nachbarn
            (30, new List<Vector3Int>()),  // 7 gültige Nachbarn
            (40, new List<Vector3Int>()),  // 8 gültige Nachbarn
        };

        // Alle belegten Positionen mit passender ID sammeln
        foreach (var pos in gridData.GetAllOccupiedPositions())
        {
            if (gridData.GetObjectIDAt(pos) == objectID - 1) upgradeables.Add(pos);
        }

        // Nachbarn prüfen (direkte Nachbarn)
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

            // Nur Positionen mit 4 direkten Nachbarn werden weiter betrachtet
            if (numDirectNeighbors == 4)
            {
                validUpgradeables.Add(pos);
            }
        }

        // Weitere Nachbarn prüfen (diagonal)
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

            // Position in die entsprechende Liste einfügen
            upgradeablesLists[numNeighbors].list.Add(pos);
        }

        // Listen mischen
        foreach (var list in upgradeablesLists)
            Shuffle(list.list);

        // Upgrade nach gewichteter Wahrscheinlichkeit
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

            // Liste nach Gewicht zufällig auswählen
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

            // Sicherheitsabfrage
            if (chosenIndex == -1)
                continue;

            Vector3Int pos = upgradeablesLists[chosenIndex].list[0];
            int representationIndex = gridData.GetRepresentationIndex(pos);
            upgradeablesLists[chosenIndex].list.RemoveAt(0);

            GameObject go = objectPlacer.placedGameObject[representationIndex];
            if (go != null)
            {
                var upgradeable = go.GetComponent<UpgradeableObject>();
                if (upgradeable != null)
                    upgradeable.UpgradeTo(objectID);

                var building = go.GetComponent<BuildingInstance>();
                if (building != null)
                    building.Upgrade();
            }
            gridData.UpdateObjectIDAt(pos, objectID);
            upgraded++;
        }
    }

    /// <summary>
    /// Generiert einen Fluss, der sich in mehreren Segmenten über das Grid schlängelt.
    /// Nutzt GridData zur Platzierungsprüfung und ObjectPlacer zum Instanziieren.
    /// </summary>
    public void GenerateRiver(int maxX, int maxZ, GridData gridData, int objectID)
    {
        // Berechnung der Randlänge für Start- und Endpunkte
        int borderLength = maxX * 2 + maxZ * 2 - 4;
        int innerBorderLength = maxX + maxZ - 4;
        int steps;
        int index;

        // Zufällige Start- und Endpunkte am Rand und innerhalb des Grids bestimmen
        steps = Random.Range(0, borderLength);
        Vector3Int pos0 = iterateOverEdge(steps, maxX - 1, 0, 0) + new Vector3Int(-maxX / 2, 0, -maxZ / 2);

        steps = Random.Range((int)(0 + borderLength / 3), (int)(borderLength / 3 * 2)) + steps;
        Vector3Int pos3 = iterateOverEdge(steps, maxX - 1, 0, 0) + new Vector3Int(-maxX / 2, 0, -maxZ / 2);

        steps = Random.Range(0, innerBorderLength);
        Vector3Int pos2 = iterateOverEdge(steps, maxX / 2 - 1, 0, 0) + new Vector3Int(-maxX / 2 + maxX / 4, 0, -maxZ / 2 + maxZ / 4);

        steps = Random.Range(steps + maxX / 4, innerBorderLength - maxX / 2);
        Vector3Int pos1 = iterateOverEdge(steps, maxX / 2 - 1, 0, 0) + new Vector3Int(-maxX / 2 + maxX / 4, 0, -maxZ / 2 + maxZ / 4);

        // Bestimmen, welche Zwischenpunkte zuerst angelaufen werden
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

        // Fluss von Punkt zu Punkt "zeichnen"
        for (int i = 0; i < 3; i++)
        {
            while (riverFlow != riverPoints[i])
            {
                // Nur platzieren, wenn die Stelle frei ist
                if (gridData.CanPlaceObjectAt(riverFlow, dataBase.objectsData[objectID].Size))
                {
                    index = objectPlacer.PlaceObject(dataBase.objectsData[objectID].Prefab, grid.CellToWorld(riverFlow));
                    gridData.AddObjectAt(riverFlow, dataBase.objectsData[objectID].Size, objectID, index);
                }

                // Richtung bestimmen (x oder z)
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
        // Letzten Punkt setzen
        if (gridData.CanPlaceObjectAt(riverFlow, dataBase.objectsData[objectID].Size))
        {
            index = objectPlacer.PlaceObject(dataBase.objectsData[objectID].Prefab, grid.CellToWorld(riverFlow));
            gridData.AddObjectAt(riverFlow, dataBase.objectsData[objectID].Size, objectID, index);
        }
    }

    public void RandomParkPlace(int maxX, int maxZ, GridData gridData)
    {
        // Listen für Positionen mit 1-4 Nachbarn, jeweils mit Gewicht
        List<Vector3Int> one =   new(); // 1 Nachbar
        List<Vector3Int> two =   new(); // 2 Nachbarn
        List<Vector3Int> three = new(); // 3 Nachbarn
        List<Vector3Int> four =  new(); // 4 Nachbarn
        
        List<Vector3Int>[] placeablesLists = {one, two, three, four};

        // Alle möglichen Positionen im Grid durchgehen
        for (int x = -maxX / 2; x < maxX / 2; x++)
        {
            for (int z = -maxZ / 2; z < maxZ / 2; z++)
            {
                Vector3Int testPos = new Vector3Int(x, 0, z);
                if (gridData.CanPlaceObjectAt(testPos, dataBase.objectsData[4].Size))
                {
                    // Nachbarn zählen (direkt angrenzende Felder)
                    int numNeighbors = -1;
                    if (!gridData.CanPlaceObjectAt(testPos + new Vector3Int(1, 0, 0),  dataBase.objectsData[4].Size)) numNeighbors++;
                    if (!gridData.CanPlaceObjectAt(testPos + new Vector3Int(-1, 0, 0), dataBase.objectsData[4].Size)) numNeighbors++;
                    if (!gridData.CanPlaceObjectAt(testPos + new Vector3Int(0, 0, 1),  dataBase.objectsData[4].Size)) numNeighbors++;
                    if (!gridData.CanPlaceObjectAt(testPos + new Vector3Int(0, 0, -1), dataBase.objectsData[4].Size)) numNeighbors++;

                    // Position in die entsprechende Liste einfügen
                    if (numNeighbors > -1) placeablesLists[numNeighbors].Add(testPos);
                }
            }
        }

        // Jede Liste mischen
        foreach (var list in placeablesLists)
            Shuffle(list);

        List<Vector3Int> manyToFew = new();
        foreach (var viablePos in four)  manyToFew.Add(viablePos);
        foreach (var viablePos in three) manyToFew.Add(viablePos);
        foreach (var viablePos in two)   manyToFew.Add(viablePos);
        foreach (var viablePos in one)   manyToFew.Add(viablePos);

        // Platzieren mit Priorität auf Position mit mehr Nachbarn
        Vector3Int pos = manyToFew[0];

        int index = objectPlacer.PlaceObject(dataBase.objectsData[4].Prefab, grid.CellToWorld(pos));
        gridData.AddObjectAt(pos, dataBase.objectsData[4].Size, 4, index);
    }

    /// <summary>
    /// Mischt eine Liste mit dem Fisher-Yates-Algorithmus.
    /// Wird für zufällige Platzierung verwendet.
    /// </summary>
    private void Shuffle(List<Vector3Int> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int rand = Random.Range(i, list.Count);
            (list[i], list[rand]) = (list[rand], list[i]);
        }
    }

    /// <summary>
    /// Hilfsfunktion, um eine bestimmte Anzahl Schritte entlang des Rands eines Rechtecks zu gehen.
    /// Wird für die Flussgenerierung verwendet.
    /// </summary>
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
