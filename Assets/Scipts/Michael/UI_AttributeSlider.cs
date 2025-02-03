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

    // The bone the slider is referencing
    [SerializeField] private Transform attribute_transform;
    private Vector3 attribute_transform_originalPosition;
    private Vector3 attribute_transform_originalRotation;
    private Vector3 attribute_transform_originalWorldScale;

    // The mirror of the reference bone (allows mirroring of attributes equally)
    [SerializeField] private bool attribute_isMirrored;
    [SerializeField] private Transform attribute_transformMirror;
    private Vector3 attribute_transformMirror_originalPosition;
    private Vector3 attribute_transformMirror_originalRotation;
    private Vector3 attribute_transformMirror_originalScale;

    private Dictionary<Transform, Dictionary<string, Vector3>> attribute_transformChildren_data;

    [Header("Hierarchy Script References")]

    [SerializeField] private UI_AttributeSlider attribute_transformParent_script;
    [SerializeField] private List<UI_AttributeSlider> attribute_transformChildren_scripts;

    private Slider attribute_slider;

    private Vector3 testing_posOffset;

    // Start is called before the first frame update
    void Start()
    {
        // Gets a reference to the slider component in the prefab
        attribute_slider = GetComponent<Slider>();

        // 
        attribute_transform_originalPosition = attribute_transform.position;
        attribute_transform_originalRotation = attribute_transform.eulerAngles;
        attribute_transform_originalWorldScale = attribute_transform.lossyScale;

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

    public void UpdateModel()
    {
        foreach (TargetAttribute a in attributes)
        {
            int attribute_intVal = (int)a;

            if (attribute_intVal <= 2)
                UpdateModelPosition(a);
            else if (attribute_intVal <= 5)
                UpdateModelRotation(a);
            else
                UpdateModelScale(a);
        }
    }

    public void LateUpdate()
    {
        attribute_transform.localPosition += testing_posOffset;
        if (attribute_isMirrored)
            attribute_transformMirror.localPosition += new Vector3(-testing_posOffset.x, testing_posOffset.y, testing_posOffset.z);
    }

    private void UpdateModelPosition(TargetAttribute a)
    {
        float pos_x = a == TargetAttribute.Position_X ? attribute_slider.value + attribute_transformMirror_originalPosition.x : attribute_transform.position.x;
        float pos_y = a == TargetAttribute.Position_Y ? attribute_slider.value + attribute_transformMirror_originalPosition.y : attribute_transform.position.y;
        float pos_z = a == TargetAttribute.Position_Z ? attribute_slider.value + attribute_transformMirror_originalPosition.z : attribute_transform.position.z;

        attribute_transform.position = new Vector3(pos_x, pos_y, pos_z);

        if (a == TargetAttribute.Position_X)
            testing_posOffset.x = attribute_slider.value;
        if (a == TargetAttribute.Position_Y)
            testing_posOffset.y = attribute_slider.value;
        if (a == TargetAttribute.Position_Z)
            testing_posOffset.z = attribute_slider.value;
    }

    private void UpdateModelRotation(TargetAttribute a)
    {
        float rot_x = a == TargetAttribute.Rotation_X ? attribute_slider.value : attribute_transform.eulerAngles.x;
        float rot_y = a == TargetAttribute.Rotation_Y ? attribute_slider.value : attribute_transform.eulerAngles.y;
        float rot_z = a == TargetAttribute.Rotation_Z ? attribute_slider.value : attribute_transform.eulerAngles.z;

        attribute_transform.rotation = Quaternion.Euler(rot_x, rot_y, rot_z);


    }

    private void UpdateModelScale(TargetAttribute a)
    {
        Vector3 attribute_transformParent_worldScale;
        if (attribute_transformParent_script != null)
            attribute_transformParent_worldScale = attribute_transformParent_script.GetOriginalBoneTransform()["original_scale"];
        else
            attribute_transformParent_worldScale = new Vector3(100.0f, 100.0f, 100.0f);

        float scale_x = a == TargetAttribute.Scale_X ? ((attribute_slider.value * attribute_transformParent_worldScale.x) + attribute_transform_originalWorldScale.x) / attribute_transform.parent.lossyScale.x
                                                     : attribute_transform.localScale.x;
        float scale_y = a == TargetAttribute.Scale_Y ? ((attribute_slider.value * attribute_transformParent_worldScale.y) + attribute_transform_originalWorldScale.y) / attribute_transform.parent.lossyScale.y
                                                     : attribute_transform.localScale.y;
        float scale_z = a == TargetAttribute.Scale_Z ? ((attribute_slider.value * attribute_transformParent_worldScale.z) + attribute_transform_originalWorldScale.z) / attribute_transform.parent.lossyScale.z
                                                     : attribute_transform.localScale.z;

        Vector3 newScale = new Vector3(scale_x, scale_y, scale_z);

        ApplyModelScaling(attribute_transform, newScale);
        if (attribute_isMirrored)
            ApplyModelScaling(attribute_transformMirror, newScale);

    }

    private void ApplyModelScaling(Transform bone, Vector3 newScale)
    {   
        for (int i = 0; i < bone.childCount; i++)
        {
            Transform attribute_transformChild = bone.GetChild(i);

            bool isBoneScripted = false;
            foreach (UI_AttributeSlider script in attribute_transformChildren_scripts)
            {
                if (attribute_transformChild == script.GetBoneReference() || attribute_transformChild == script.GetMirrorBoneReference())
                    isBoneScripted = true;
            }

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

        bone.localScale = newScale;

        foreach (UI_AttributeSlider childScript in attribute_transformChildren_scripts)
        {
            childScript.UpdateModel();
        }
    }
}
