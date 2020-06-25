using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    // Attached to canvas of pause menu

    public KeyCode PauseKey;

    private bool paused;

    private CharacterMover cm;
    private FPSCameraController cc;

    private float defaultMoveSpeed;
    private float defaultJumpForce;
    private float defaultMouseSens;

    // Start is called before the first frame update
    void Start()
    {
        paused = true; // want to toggle with the first click, so feels backwards
        cm = GameObject.Find("Player").GetComponent<CharacterMover>();
        cc = GameObject.Find("Camera").GetComponent<FPSCameraController>();

        defaultMoveSpeed = cm.moveSpeed;
        defaultJumpForce = cm.jumpForce;
        defaultMouseSens = cc.mouseSensitivity;

        SetUIComponents(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(PauseKey))
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        paused = !paused;
        if (paused)
        {
            Cursor.lockState = CursorLockMode.Locked;
            SetUIComponents(false);
            SetPlayerPlayerState(true);
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            SetUIComponents(true);
            SetPlayerPlayerState(false);
        }
        GameObject.Find("Canvas").GetComponent<Canvas>().enabled = paused;
    }

    void SetUIComponents(bool on)
    {
        int child_count = transform.childCount;
        for (int i = 0; i < child_count; i++)
        {
            GameObject c = transform.GetChild(i).gameObject;
            if (c.name.Contains("BG"))
            {
                c.GetComponent<Image>().enabled = on;
            }
            if (c.name.Contains("Text"))
            {
                c.GetComponent<Text>().enabled = on;
            }
        }
    }

    void SetPlayerPlayerState(bool enabled)
    {
        if (enabled)
        {
            cm.moveSpeed = defaultMoveSpeed;
            cm.jumpForce = defaultJumpForce;
            cc.mouseSensitivity = defaultMouseSens;
        }
        else
        {
            cm.moveSpeed = 0.0f;
            cm.jumpForce = 0.0f;
            cc.mouseSensitivity = 0.0f;
        }
    }

    // Here are the methods that buttons will call

    public void ResetLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void MainMenu()
    {
        Cursor.lockState = CursorLockMode.None;
        TimeKeep.first = true; // workaround for timer
        SceneManager.LoadScene("MainMenu");
    }
    
    public void CloseMenu()
    {
        GameObject.Find("PauseCanvas").GetComponent<PauseMenu>().TogglePause();
    }
    
}
