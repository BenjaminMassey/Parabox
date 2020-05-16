using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSCameraController : MonoBehaviour
{
    public Transform player;

    public float mouseSensitivity = 10.0f;

    private float mouseX;
    private float mouseY;
    private float xRotation;

    private bool active;

    private void Awake()
    {
        active = true;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        if (active)
        {
            mouseX = Input.GetAxisRaw("Mouse X") * mouseSensitivity;
            mouseY = Input.GetAxisRaw("Mouse Y") * mouseSensitivity;

            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -90.0f, 90.0f);

            transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
            player.transform.rotation *= Quaternion.Euler(Vector3.up * mouseX);
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                active = false;
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Mouse0) && IsMouseOverGameWindow)
            {
                active = true;
            }
        }
    }

    // http://answers.unity.com/answers/1681937/view.html
    bool IsMouseOverGameWindow { get { return !(0 > Input.mousePosition.x || 0 > Input.mousePosition.y || Screen.width < Input.mousePosition.x || Screen.height < Input.mousePosition.y); } }
}
