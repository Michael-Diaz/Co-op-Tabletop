using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_AttributeSlider : MonoBehaviour
{
    private enum TargetAttribute
    {
        Position_X,
        Position_Y,
        Position_Z,
        Rotation_X,
        Rotation_Y,
        Rotation_Z,
        Scale_X,
        Scale_Y,
        Scale_Z
    };

    [SerializeField] private List<TargetAttribute> attributes;

    [Header("Bone Reference(s)")]

    // The bone on the model the slider is referencing, along with its original transform state
    [SerializeField] private Transform attribute_transform;
    private Vector3 attribute_transform_originalPosition;
    private Vector3 attribute_transform_originalRotation;
    private Vector3 attribute_transform_originalWorldScale;

    // The mirror of the reference bone (allows mirroring of attributes equally for things like scale and position offsets)
    [SerializeField] private bool attribute_isMirrored;
    [SerializeField] private Transform attribute_transformMirror;
    private Vector3 attribute_transformMirror_originalPosition;
    private Vector3 attribute_transformMirror_originalRotation;
    private Vector3 attribute_transformMirror_originalScale;

    // A dictionary of the children bones of the reference bone
    // the secondary dictionary contains information on the original state of the transform ("pos", "rot", "scale") and its corresponding numbers as a Vec3
    private Dictionary<Transform, Dictionary<string, Vector3>> attribute_transformChildren_data;

    [Header("Hierarchy Script References")]

    // References to the attribute slider scripts on the parent and children bones
    [SerializeField] private UI_AttributeSlider attribute_transformParent_script;
    [SerializeField] private List<UI_AttributeSlider> attribute_transformChildren_scripts;

    // Reference to the actual slider on the screen to pull its values
    private Slider attribute_slider;

    // For testing with the 
    private Vector3 testing_posOffset;

    // Start is called before the first frame update
    void Start()
    {
        // Caches the reference to the slider component in the prefab
        attribute_slider = GetComponent<Slider>();

        // Caches the original state of the bone
        attribute_transform_originalPosition = attribute_transform.position;
        attribute_transform_originalRotation = attribute_transform.eulerAngles;
        attribute_transform_originalWorldScale = attribute_transform.lossyScale;

        // If the bone has a mirror, then it does so as well for the mirror
        if (attribute_isMirrored)
        {
            attribute_transformMirror_originalPosition = attribute_transformMirror.position;
            attribute_transformMirror_originalRotation = attribute_transformMirror.eulerAngles;
            attribute_transformMirror_originalScale = attribute_transformMirror.lossyScale;
        }

        attribute_transformChildren_data = new Dictionary<Transform, Dictionary<string, Vector3>>();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public Transform GetBoneReference()
    {
        return attribute_transform;
    }

    public Transform GetMirrorBoneReference()
    {
        return attribute_transformMirror;
    }

    public Dictionary<string, Vector3> GetOriginalBoneTransform()
    {
        Dictionary<string, Vector3> retVal = new Dictionary<string, Vector3>();
        retVal.Add("original_pos", attribute_transform_originalPosition);
        retVal.Add("original_rot", attribute_transform_originalRotation);
        retVal.Add("original_scale", attribute_transform_originalWorldScale);

        return retVal;
    }

    // Is run every time the canvas catches an event update when the slider's value changes
    public void UpdateModel()
    {
        // Since each slider can affect multiple attributes at the same time, we're looping through its attributes and applying the respective operation
        // These are set in the inspector for ease of use
        foreach (TargetAttribute a in attributes)
        {
            // The enumerations for each attribute are casted to integers as they're grouped by type (position, rotation, and scale)
            int attribute_intVal = (int)a;

            if (attribute_intVal <= 2) // Values 2 or less are for position
                UpdateModelPosition(a);
            else if (attribute_intVal <= 5) // Values between 3 and 5 are for rotation
                UpdateModelRotation(a);
            else
                UpdateModelScale(a); // Everything else is for scale
        }
    }

    // Will need to find a way to move this code into the attribute manager later on but it works for now lol
    // The secret sauce is that LateUpdate runs after the bones have been set by the animation curve's keyframes, BUT before everything renders to the screen
    public void LateUpdate()
    {
        // Apply the positional offset from the slider onto the bone, and do the opposite for the mirror
        attribute_transform.localPosition += testing_posOffset;
        if (attribute_isMirrored)
            attribute_transformMirror.localPosition += new Vector3(-testing_posOffset.x, testing_posOffset.y, testing_posOffset.z);
    }

    // Will apply a local positional offset to a bone in relation to its parent orientation based off of the slider value
    private void UpdateModelPosition(TargetAttribute a)
    {
        // Ignore this block of code as it was an attempt at a first pass lol
        /*
        float pos_x = a == TargetAttribute.Position_X ? attribute_slider.value + attribute_transformMirror_originalPosition.x : attribute_transform.position.x;
        float pos_y = a == TargetAttribute.Position_Y ? attribute_slider.value + attribute_transformMirror_originalPosition.y : attribute_transform.position.y;
        float pos_z = a == TargetAttribute.Position_Z ? attribute_slider.value + attribute_transformMirror_originalPosition.z : attribute_transform.position.z;
        

        attribute_transform.position = new Vector3(pos_x, pos_y, pos_z);
        */

        // Icky placeholder code for now :(
        if (a == TargetAttribute.Position_X)
            testing_posOffset.x = attribute_slider.value;
        if (a == TargetAttribute.Position_Y)
            testing_posOffset.y = attribute_slider.value;
        if (a == TargetAttribute.Position_Z)
            testing_posOffset.z = attribute_slider.value;
    }

    // Will apply a local roational offset to a bone in relation to its parent orientation based off of the slider value
    private void UpdateModelRotation(TargetAttribute a)
    {
        // Same thing as the UpdateModelPosition function lol
        // NOTE: Keeping this uncommented for the body rotation slider as it isn't influence by animation keyframes, but this WILL need to be updated
        float rot_x = a == TargetAttribute.Rotation_X ? attribute_slider.value : attribute_transform.eulerAngles.x;
        float rot_y = a == TargetAttribute.Rotation_Y ? attribute_slider.value : attribute_transform.eulerAngles.y;
        float rot_z = a == TargetAttribute.Rotation_Z ? attribute_slider.value : attribute_transform.eulerAngles.z;

        attribute_transform.rotation = Quaternion.Euler(rot_x, rot_y, rot_z);

    }

    // Updates the local scale of the selected bone in relation to its original global scale of 1
    // Then alters the local scale of its children to preserve their global scales
    private void UpdateModelScale(TargetAttribute a)
    {
        // Will attempt to get the world scale of the parent via the cached script reference
        Vector3 attribute_transformParent_worldScale;
        if (attribute_transformParent_script != null)
            attribute_transformParent_worldScale = attribute_transformParent_script.GetOriginalBoneTransform()["original_scale"];
        else
            attribute_transformParent_worldScale = new Vector3(100.0f, 100.0f, 100.0f); // For some reason the model is incredibly scaled up so that the default scale is 100

        // Each time the script loops through the attributes it will scale each bone locally bit by bit
        // If the respective attribute is the one being looped (and passed to the function), it will get the slider value, otherwise it uses the existing world scale of the bone in that axis
        float scale_x = a == TargetAttribute.Scale_X ? ((attribute_slider.value * attribute_transformParent_worldScale.x) + attribute_transform_originalWorldScale.x) / attribute_transform.parent.lossyScale.x
                                                     : attribute_transform.localScale.x;
        float scale_y = a == TargetAttribute.Scale_Y ? ((attribute_slider.value * attribute_transformParent_worldScale.y) + attribute_transform_originalWorldScale.y) / attribute_transform.parent.lossyScale.y
                                                     : attribute_transform.localScale.y;
        float scale_z = a == TargetAttribute.Scale_Z ? ((attribute_slider.value * attribute_transformParent_worldScale.z) + attribute_transform_originalWorldScale.z) / attribute_transform.parent.lossyScale.z
                                                     : attribute_transform.localScale.z;

        Vector3 newScale = new Vector3(scale_x, scale_y, scale_z);

        // Apply the scale to the bone and its mirror if applicable
        ApplyModelScaling(attribute_transform, newScale);
        if (attribute_isMirrored)
            ApplyModelScaling(attribute_transformMirror, newScale);

    }

    // Does the actual math when rescaling the bone to figure out how to adjust the scale of the children bones
    private void ApplyModelScaling(Transform bone, Vector3 newScale)
    {   
        // Loop through the children of the current bone
        for (int i = 0; i < bone.childCount; i++)
        {
            Transform attribute_transformChild = bone.GetChild(i);

            // Check to see if the child bone has a UI_AttributeSlider component attached; moreso if that script is one listed in the child references
            // Essentially, "Is this child bone registered as my child?"
            bool isBoneScripted = false;
            foreach (UI_AttributeSlider script in attribute_transformChildren_scripts)
            {
                if (attribute_transformChild == script.GetBoneReference() || attribute_transformChild == script.GetMirrorBoneReference())
                    isBoneScripted = true;
            }

            // If the bone is not a registered child, and it has not been added to the dictionary of the original state of the child transform, add it
            // Then do a calculation to maintain it's world scale in relation to the parent bone, and set its local scale to that
            if (!isBoneScripted)
            {
                if (!attribute_transformChildren_data.ContainsKey(attribute_transformChild))
                {
                    Dictionary<string, Vector3> bone_transformComponents = new Dictionary<string, Vector3>();
                    bone_transformComponents.Add("original_pos", attribute_transformChild.position);
                    bone_transformComponents.Add("original_rot", attribute_transformChild.eulerAngles);
                    bone_transformComponents.Add("original_scale", attribute_transformChild.lossyScale);

                    attribute_transformChildren_data.Add(attribute_transformChild, bone_transformComponents);
                }

                Vector3 attribute_transformChild_originalWorldScale = attribute_transformChildren_data[attribute_transformChild]["original_scale"];

                attribute_transformChild.localScale = new Vector3(attribute_transformChild_originalWorldScale.x / bone.lossyScale.x,
                                                                    attribute_transformChild_originalWorldScale.y / bone.lossyScale.y,
                                                                    attribute_transformChild_originalWorldScale.z / bone.lossyScale.z);
            }
        }

        // We set the original bone's scale now instead of before to prevent overwriting the original global transforms of the children
        bone.localScale = newScale;

        // Recursively call the function on all the children to propogate the calculations down the bone hierarchy
        foreach (UI_AttributeSlider childScript in attribute_transformChildren_scripts)
        {
            childScript.UpdateModel();
        }
    }
}
