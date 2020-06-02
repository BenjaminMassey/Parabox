using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TimerDisplay : MonoBehaviour
{
    public GameObject text;
    public GameObject box;
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
