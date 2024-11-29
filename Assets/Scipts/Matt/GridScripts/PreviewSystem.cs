using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviewSystem : MonoBehaviour
{
    [SerializeField]
    private float previewYOffset = 0.06f;

    [SerializeField]
    private GameObject cellIndicator;
    private GameObject previewObject;

    [SerializeField]
    private Material previewMaterialPrefab;
    private Material previewMaterialInstance;

    private Renderer[] cellIndicatorRenderer;

    private void Start()
    {
        previewMaterialInstance = new Material(previewMaterialPrefab);
        cellIndicator.SetActive(false);

        cellIndicatorRenderer = cellIndicator.GetComponentsInChildren<Renderer>();
    }

    /* Starts the preview by instantiating the object that is going to be placed and calling the PreparePreview and PrepareCursor functions */
    public void StartShowingPlacementPreview(GameObject prefab, Vector2Int size)
    {
        previewObject = Instantiate(prefab);
        PreparePreview(previewObject);
        PrepareCursor(size);

        cellIndicator.SetActive(true);
    }

    /* This changed the cursor scale so that it fits to any object passed */
    private void PrepareCursor(Vector2Int size)
    {
        if(size.x > 0 || size.y > 0)
        {
            cellIndicator.transform.localScale = new Vector3(size.x, 1, size.y);
        }
    }

    /* This changes the color of the preview to either white or red depending on if its valid or invalid */
    private void PreparePreview(GameObject previewObject)
    {
        Renderer[] renderers = previewObject.GetComponentsInChildren<Renderer>();

        foreach(Renderer renderer in renderers)
        {
            Material[] materials = renderer.materials;

            for(int i = 0; i < materials.Length; i++)
            {
                materials[i] = previewMaterialInstance;
            }

            renderer.materials = materials;
        }
    }

    /* This disabled the cellIndicator and destroys the previewObject if it exists */
    public void StopShowingPreview()
    {
        cellIndicator.SetActive(false);
        if(previewObject != null)
        {
            Destroy(previewObject);
        }
    }

    /* This updates the position and color of the cursor and preview if the preview exists */
    public void UpdatePosition(Vector3 position, bool validity)
    {
        if(previewObject != null)
        {
            MovePreview(position);
            ApplyFeedbackToPreview(validity);
        }

        MoveCursor(position);
        ApplyFeedbackToCursor(validity);
    }

    /* This changes the color of the previewMaterialInstance depending on Validity */
    private void ApplyFeedbackToPreview(bool validity)
    {
        Color c = validity ? Color.white : Color.red;
        c.a = 0.5f;

        previewMaterialInstance.color = c;
    }

    /* This changes the color of the cellMaterialInstance depending on Validity */
    private void ApplyFeedbackToCursor(bool validity)
    {
        Color c = validity ? Color.white : Color.red;
        c.a = 0.5f;
        foreach (var renderer in cellIndicatorRenderer)
        {
            renderer.material.color = c;
        }
    }

    /* This moves the Cursor to the sent position */
    private void MoveCursor(Vector3 position)
    {
        cellIndicator.transform.position = position;
    }

    /* This moves the preview to the sent position */
    private void MovePreview(Vector3 position)
    {
        previewObject.transform.position = new Vector3(position.x, position.y + previewYOffset, position.z);
    }

    /* This moves the cursor to the base size and turns the cursor red */
    internal void StartShowingRemovePreview()
    {
        cellIndicator.SetActive(true);
        PrepareCursor(Vector2Int.one);
        ApplyFeedbackToCursor(false);
    }
}
