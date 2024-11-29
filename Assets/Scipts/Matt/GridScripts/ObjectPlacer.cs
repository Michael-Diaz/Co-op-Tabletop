using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPlacer : MonoBehaviour
{
    private List<GameObject> placedGameObjects = new();

    /* Instantiates the sent prefab and puts it in the given position. 
     * It then adds the object to the placedGameObjects list and returns the index of where it was added*/
    public int PlaceObject(GameObject prefab, Vector3 position)
    {
        GameObject newObject = Instantiate(prefab);
        newObject.transform.position = position;
        placedGameObjects.Add(newObject);

        return placedGameObjects.Count - 1;
    }

    /* Checks if the index exists and the count of the list is more than the index.
     * It then destroys the object and then sets that index to null*/
    internal void RemoveObjectAt(int gameObjectIndex)
    {
        if(placedGameObjects.Count <= gameObjectIndex || placedGameObjects[gameObjectIndex] == null)
        {
            return;
        }

        Destroy(placedGameObjects[gameObjectIndex]);
        placedGameObjects[gameObjectIndex] = null;
    }
}
