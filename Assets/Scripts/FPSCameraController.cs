using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSCameraController : MonoBehaviour
{
    public Transform player;

    public float EditorMouseSensitivity = 1.0f;
    public float ReleaseMouseSensitivity = 0.35f;

    private float mouseSensitivity;

    private float mouseX;
    private float mouseY;
    private float xRotation;

    private bool active;

    private void Awake()
    {
        active = true;
        Cursor.lockState = CursorLockMode.Locked;

        if (Application.platform.Equals(RuntimePlatform.WindowsEditor))
        {
            mouseSensitivity = EditorMouseSensitivity;
        }
        else
        {
            mouseSensitivity = ReleaseMouseSensitivity;
        }
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
                Cursor.lockState = CursorLockMode.None;
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Mouse0) && IsMouseOverGameWindow)
            {
                active = true;
                Cursor.lockState = CursorLockMode.Locked;
            }
        }
    }

    // http://answers.unity.com/answers/1681937/view.html
    bool IsMouseOverGameWindow { get { return !(0 > Input.mousePosition.x || 0 > Input.mousePosition.y || Screen.width < Input.mousePosition.x || Screen.height < Input.mousePosition.y); } }

    public float GetMouseSens()
    {
        return mouseSensitivity;
    }

    public void SetMouseSens(float s)
    {
        mouseSensitivity = s;
    }
}
