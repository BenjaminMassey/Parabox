using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GeneralHandling : MonoBehaviour
{
    // Some random handling: generally unique input conditions

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        /* Replaced with PauseMenu.cs
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            FancyTextHandler.Message("Mouse Is Free", 10, true);
        }
        */
        if (Input.GetKeyDown(KeyCode.G))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        if (Input.GetKeyDown(KeyCode.M))
        {
            Cursor.lockState = CursorLockMode.None;
            TimeKeep.first = true; // workaround for timer
            SceneManager.LoadScene("MainMenu");
        }
    }
}
