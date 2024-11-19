using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridData
{
    Dictionary<Vector3Int, PlacementData> placedObjects = new();

    /* Gets the data for the object using the Object ID and places the object in its corresponding cell in the Dictionary if there is nothing in that cell*/
    public void AddObjectAt(Vector3Int gridPosition, Vector2Int objectSize, int ID, int placedObjectIndex)
    {
        List<Vector3Int> positionsToOccupy = CalculatePositions(gridPosition, objectSize);
        PlacementData data = new PlacementData(positionsToOccupy, ID, placedObjectIndex);

        foreach(var pos in positionsToOccupy)
        {
            if (placedObjects.ContainsKey(pos))
            {
                throw new Exception($"Dictionary already containts this cell position{pos}");
            }
            placedObjects[pos] = data;
        }
    }

    /* This function gets the cells that would be occupied with the current object and returns a list with those positions*/
    private List<Vector3Int> CalculatePositions(Vector3Int gridPosition, Vector2Int objectSize)
    {
        List<Vector3Int> returnVal = new();
        for(int x = 0; x < objectSize.x; x++)
        {
            for(int y = 0; y < objectSize.y; y++)
            {
                returnVal.Add(gridPosition + new Vector3Int(x, 0, y));
            }
        }

        return returnVal;
    }

    /* Checks if the cells the object would be placed in already have an object in them.*/
    public bool CanPlaceObjectAt(Vector3Int gridPosition, Vector2Int objectSize)
    {
        List<Vector3Int> positionToOccupy = CalculatePositions(gridPosition, objectSize);
        
        foreach( var pos in positionToOccupy)
        {
            if (placedObjects.ContainsKey(pos))
            {
                return false;
            }
        }

        return true;
    }

    /* Returns the PlacedObjectIndex for a grid position or -1 if it doesn't exist*/
    internal int getRepresentationIndex(Vector3Int gridPosition)
    {
        if (!placedObjects.ContainsKey(gridPosition))
        {
            return -1;
        }
        else
        {
            return placedObjects[gridPosition].PlacedObjectIndex;
        }
    }

    /* Removes the occupied positions in the Dictionary for the sent grid position*/
    internal void RemoveObjectAt(Vector3Int gridPosition)
    {
        foreach(var pos in placedObjects[gridPosition].occupidePositions)
        {
            placedObjects.Remove(pos);
        }
    }
}

public class PlacementData
{
    public List<Vector3Int> occupidePositions;
    public int ID { get; private set; }
    public int PlacedObjectIndex { get; private set; }

    public PlacementData(List<Vector3Int> occupidePositions, int iD, int placedObjectIndex)
    {
        this.occupidePositions = occupidePositions;
        ID = iD;
        PlacedObjectIndex = placedObjectIndex;
    }

}