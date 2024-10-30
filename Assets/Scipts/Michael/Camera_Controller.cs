using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_Controller : MonoBehaviour
{
    private Transform cam_transform;

    [SerializeField] private float mouse_xSensitivity;
    [SerializeField] private float mouse_ySensitivity;
    [SerializeField] private float cam_moveSpeed;

    // Start is called before the first frame update
    void Start()
    {
        cam_transform = transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse1))
            LockCursor();

        if (Input.GetKey(KeyCode.Mouse1))
        {
            float mouse_xDelta = Input.GetAxis("Mouse Y") * mouse_xSensitivity * Time.deltaTime;
            float mouse_yDelta = Input.GetAxis("Mouse X") * mouse_ySensitivity * Time.deltaTime;

            float cam_xRot = cam_transform.rotation.eulerAngles.x;
            float cam_yRot = cam_transform.rotation.eulerAngles.y;

            cam_transform.rotation = Quaternion.Euler(cam_xRot - mouse_xDelta, cam_yRot + mouse_yDelta, 0.0f);
        }

        Vector3 cam_xPosDelta = Input.GetAxis("Horizontal") * transform.TransformDirection(Vector3.right) * cam_moveSpeed * Time.deltaTime;
        Vector3 cam_zPosDelta = Input.GetAxis("Vertical") * transform.TransformDirection(Vector3.forward) * cam_moveSpeed * Time.deltaTime;

        transform.position += cam_xPosDelta + cam_zPosDelta;

        if (Input.GetKeyUp(KeyCode.Mouse1))
            UnlockCursor();
    }

    private void LockCursor()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void UnlockCursor()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
}
