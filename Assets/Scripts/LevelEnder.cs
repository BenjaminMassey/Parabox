using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelEnder : MonoBehaviour
{
    // Script to handle hitting the end of level and take to next
    // Attached to a portal (sub-object of some "End Gate.prefab")

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P)) // for debugging
        {
            NextLevel();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Player")
        {
            //Debug.Log(SceneManager.GetActiveScene().name);
            NextLevel();
        }
    }

    void NextLevel()
    {
        if (SceneManager.GetActiveScene().name == "Tutorial")
        {
            SceneManager.LoadScene("Level1");
        }
        else if (SceneManager.GetActiveScene().name == "Level1")
        {
            SceneManager.LoadScene("Level2");
        }
        else if (SceneManager.GetActiveScene().name == "Level2")
        {
            SceneManager.LoadScene("Level3");
        }
        else if (SceneManager.GetActiveScene().name == "Level3")
        {
            SceneManager.LoadScene("Level4");
        }
        else if (SceneManager.GetActiveScene().name == "Level4")
        {
            SceneManager.LoadScene("Level5");
        }
        else if (SceneManager.GetActiveScene().name == "Level5")
        {
            SceneManager.LoadScene("Level6");
        }
        else if (SceneManager.GetActiveScene().name == "Level6")
        {
            SceneManager.LoadScene("Level7");
        }
        else if (SceneManager.GetActiveScene().name == "Level7")
        {
            Debug.Log("Ended Timer");
            TimeKeep.End();
            Cursor.lockState = CursorLockMode.None;
            SceneManager.LoadScene("MainMenu");
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
