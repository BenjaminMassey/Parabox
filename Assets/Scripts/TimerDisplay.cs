using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TimerDisplay : MonoBehaviour
{
    // Allows the usage of timer display in MainMenu scene
    // Set up to only show up the not-first time you load MainMenu
        // IE you've beaten the game at least once (so have a time)
    
    // Attached to timer display made up like this:
    // Empty GameObject
    // >> Text object with TextMesh
    // >> Cube (acts as BG for text)
    // These sub-objects will start out disabled, and will be enabled here

    public GameObject text; // Text object with TextMesh
    public GameObject box; // Cube (acts as BG for text)

    // Start is called before the first frame update
    void Start()
    {
        if (SceneManager.GetActiveScene().name.Equals("MainMenu") && !TimeKeep.first)
        {
            text.GetComponent<MeshRenderer>().enabled = true;
            box.GetComponent<MeshRenderer>().enabled = true;
            text.GetComponent<TextMesh>().text = "Your time:\n" + TimeKeep.GetTime();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
