using System.Collections;
using System.Collections.Generic;
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

    [SerializeField] private List<TargetAttribute> attribute;

    [SerializeField] private Transform attribute_transform;
    private Vector3 attribute_transform_originalPosition;
    private Vector3 attribute_transform_originalRotation;
    private Vector3 attribute_transform_originalScale;

    [SerializeField] private bool attribute_isMirrored;
    [SerializeField] private Transform attribute_transformMirror;
    private Vector3 attribute_transformMirror_originalPosition;
    private Vector3 attribute_transformMirror_originalRotation;
    private Vector3 attribute_transformMirror_originalScale;

    private Dictionary<Transform, Dictionary<string, Vector3>> attribute_transformChildren_data;

    private Slider attribute_slider;

    // Start is called before the first frame update
    void Start()
    {
        attribute_slider = GetComponent<Slider>();

        attribute_transform_originalPosition = attribute_transform.position;
        attribute_transform_originalRotation = attribute_transform.eulerAngles;
        attribute_transform_originalScale = attribute_transform.localScale;

        if (attribute_isMirrored)
        {
            attribute_transformMirror_originalPosition = attribute_transformMirror.position;
            attribute_transformMirror_originalRotation = attribute_transformMirror.eulerAngles;
            attribute_transformMirror_originalScale = attribute_transformMirror.localScale;
        }

        attribute_transformChildren_data = new Dictionary<Transform, Dictionary<string, Vector3>>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateModel()
    {
        foreach (TargetAttribute a in attribute)
        {
            int attribute_intVal = (int) a;

            if (attribute_intVal <= 2)
                UpdateModelPosition(a);
            else if (attribute_intVal <= 5)
                UpdateModelRotation(a);
            else
                UpdateModelScale(a);
        }
    }

    private void UpdateModelPosition(TargetAttribute a)
    {
        float pos_x = a == TargetAttribute.Position_X ? attribute_slider.value + attribute_transformMirror_originalPosition.x : attribute_transform.eulerAngles.x;
        float pos_y = a == TargetAttribute.Position_Y ? attribute_slider.value + attribute_transformMirror_originalPosition.y : attribute_transform.eulerAngles.y;
        float pos_z = a == TargetAttribute.Position_Z ? attribute_slider.value + attribute_transformMirror_originalPosition.z : attribute_transform.eulerAngles.z;

        attribute_transform.position = new Vector3(pos_x, pos_y, pos_z);
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
        float scale_x = a == TargetAttribute.Scale_X ? attribute_slider.value + attribute_transform_originalScale.x : attribute_transform.localScale.x;
        float scale_y = a == TargetAttribute.Scale_Y ? attribute_slider.value + attribute_transform_originalScale.y : attribute_transform.localScale.y;
        float scale_z = a == TargetAttribute.Scale_Z ? attribute_slider.value + attribute_transform_originalScale.z : attribute_transform.localScale.z;

        Vector3 newScale = new Vector3(scale_x, scale_y, scale_z);

        ApplyModelScaling(attribute_transform, newScale);
        if (attribute_isMirrored)
            ApplyModelScaling(attribute_transformMirror, newScale);

    }

    private void ApplyModelScaling(Transform bone, Vector3 newScale)
    {
        bone.localScale = new Vector3(newScale.x, newScale.y, newScale.z);

        for (int i = 0; i < bone.childCount; i++)
        {
            Transform attribute_transformChild = bone.GetChild(i);

            if (!attribute_transformChildren_data.ContainsKey(attribute_transformChild))
            {
                Dictionary<string, Vector3> bone_transformComponents = new Dictionary<string, Vector3>();
                bone_transformComponents.Add("original_pos", attribute_transformChild.position);
                bone_transformComponents.Add("original_rot", attribute_transformChild.eulerAngles);
                bone_transformComponents.Add("original_scale", attribute_transformChild.localScale);

                attribute_transformChildren_data.Add(attribute_transformChild, bone_transformComponents);
            }

            Vector3 attribute_transformChild_originalScale = attribute_transformChildren_data[attribute_transformChild]["original_scale"];

            attribute_transformChild.localScale = new Vector3(attribute_transformChild_originalScale.x * (1.0f / newScale.x),
                                                              attribute_transformChild_originalScale.y * (1.0f / newScale.y),
                                                              attribute_transformChild_originalScale.z * (1.0f / newScale.z));
        }

        if (attribute_transformChildren_data.ContainsKey(bone))
        {
            attribute_transformChildren_data[bone]["original_pos"] = bone.position;
            attribute_transformChildren_data[bone]["original_rot"] = bone.eulerAngles;
            attribute_transformChildren_data[bone]["original_scale"] = bone.localScale;
        }
    }
}
