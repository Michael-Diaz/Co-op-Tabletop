using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridPlacementSystem : MonoBehaviour
{
    [SerializeField]
    private GridInputManager inputManager;
    [SerializeField]
    private Grid grid;

    [SerializeField]
    private ObjectsDatabaseSO database;

    [SerializeField]
    private GameObject gridVisualization;

    private GridData floorData, furnitureData;

    [SerializeField]
    private PreviewSystem preview;

    private Vector3Int lastDetectedPosition = Vector3Int.zero;

    [SerializeField]
    private ObjectPlacer objectPlacer;

    IBuildingState buildingState;

    private void Start()
    {
        StopPlacement();
        floorData = new();
        furnitureData = new();
    }

    /* Initializes the Building State variable to the PlacementState and 
     * adds the PlaceStructure function to the inputManager.OnClicked event and the StopPlacement to the inputManager.OnExt event */
    public void StartPlacement(int ID)
    {
        StopPlacement();

        gridVisualization.SetActive(true);

        buildingState = new PlacementState(ID, grid, preview,database,floorData,furnitureData,objectPlacer);

        inputManager.OnClicked += PlaceStructure;
        inputManager.OnExit += StopPlacement;
    }

    /* Initializes the Building State variable to the RemovingState and 
     * adds the PlaceStructure function to the inputManager.OnClicked event and the StopPlacement to the inputManager.OnExt event */
    public void StartRemoving()
    {
        StopPlacement();
        gridVisualization.SetActive(true);
        buildingState = new RemovingState(grid, preview, floorData, furnitureData, objectPlacer);

        inputManager.OnClicked += PlaceStructure;
        inputManager.OnExit += StopPlacement;
    }

    /* Uses the BuildingState interface function OnAction to either place or remove the object depending on initialization */
    private void PlaceStructure()
    {
        if (inputManager.IsPointerOverUI())
        {
            return;
        }

        Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);

        buildingState.OnAction(gridPosition);
    }

    /* Disables the grid visualization, uses the EndState interface function for the initialized state, takes the PlaceStructure and StopPlacement functions
     * from the inputManager event and sets the buildingState to null */
    public void StopPlacement()
    {
        if(buildingState == null)
        {
            return;
        }

        gridVisualization.SetActive(false);

        buildingState.EndState();

        inputManager.OnClicked -= PlaceStructure;
        inputManager.OnExit -= StopPlacement;

        lastDetectedPosition = Vector3Int.zero;

        buildingState = null;
    }

    /* If the BuildingState is initialized and the mouse has moved, use the UpdateState interface function to update the mouse cursor */
    private void Update()
    {
        if(buildingState == null)
        {
            return ;
        }
        Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);

        if(lastDetectedPosition != gridPosition)
        {
            buildingState.UpdateState(gridPosition);
            lastDetectedPosition = gridPosition;
        }
       
    }
}
