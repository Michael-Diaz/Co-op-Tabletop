using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_CharSheetElement : MonoBehaviour
{
    private bool isBeingHeld;

    // Start is called before the first frame update
    void Start()
    {
        isBeingHeld = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (isBeingHeld)
        {
            transform.position = Input.mousePosition;

            if (Input.GetMouseButtonUp(0))
                isBeingHeld = false;
        }
    }
}
