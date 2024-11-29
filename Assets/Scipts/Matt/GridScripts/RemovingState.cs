using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemovingState : IBuildingState
{
    private int gameObjectIndex = -1;
    Grid grid;
    PreviewSystem previewSystem;
    GridData floorData;
    GridData furnitureData;
    ObjectPlacer objectPlacer;

    public RemovingState(Grid grid, PreviewSystem previewSystem, GridData floorData, GridData furnitureData, ObjectPlacer objectPlacer)
    {
        this.grid = grid;
        this.previewSystem = previewSystem;
        this.floorData = floorData;
        this.furnitureData = furnitureData;
        this.objectPlacer = objectPlacer;

        previewSystem.StartShowingRemovePreview();
    }

    /* Calls the StopShowingPreview function from the previewSystem */
    public void EndState()
    {
        previewSystem.StopShowingPreview();
    }

    /* Sets the selectedData variable to either furnitureData or floorData and then uses the RemoveObjectAt functions to remove it from the Dictionary
     * and removes it from the scene. It then updates the position of the previewSystem*/
    public void OnAction(Vector3Int gridPosition)
    {
        GridData selectedData = null;

        if (!furnitureData.CanPlaceObjectAt(gridPosition, Vector2Int.one))
        {
            selectedData = furnitureData;
        }
        else
        {
            if(!floorData.CanPlaceObjectAt(gridPosition, Vector2Int.one))
            {
                selectedData = floorData;
            }
        }

        if(selectedData != null)
        {
            gameObjectIndex = selectedData.getRepresentationIndex(gridPosition);

            if(gameObjectIndex == -1)
            {
                return;
            }

            selectedData.RemoveObjectAt(gridPosition);
            objectPlacer.RemoveObjectAt(gameObjectIndex);
        }

        Vector3 cellPosition = grid.CellToWorld(gridPosition);
        previewSystem.UpdatePosition(cellPosition, CheckIfSelectionIsValid(gridPosition));
    }

    private bool CheckIfSelectionIsValid(Vector3Int gridPosition)
    {
        return !(furnitureData.CanPlaceObjectAt(gridPosition, Vector2Int.one) && floorData.CanPlaceObjectAt(gridPosition, Vector2Int.one));
    }

    /* Calls the UpdatePosition in the previewSystem to update the cursor position and color */
    public void UpdateState(Vector3Int gridPosition)
    {
        bool validity = CheckIfSelectionIsValid(gridPosition);

        previewSystem.UpdatePosition(grid.CellToWorld(gridPosition), validity);
    }
}
