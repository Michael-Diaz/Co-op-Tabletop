using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_AttributeManager : MonoBehaviour
{
    // Each attribute slider reqires:
    // reference to the bone it affect + bone mirror if applicable
    // local storage of local scale applied to bone
    // local storage of offset to perform on bone during animation (LateUpdate)

    // Manager provides:
    // Single instance of static values for deformations (allow to be changed later on?)
    // application of deformations outside of character creator

    // Needs:
    // dictionary to hold references to bones & attributes (2nd order dictionary??)

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
