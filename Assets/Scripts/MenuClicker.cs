using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuClicker : MonoBehaviour
{
    // https://docs.unity3d.com/Manual/CameraRays.html

    public Camera camera;

    void Start()
    {
        
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            RaycastHit hit;
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                Transform objectHit = hit.transform;
                if (objectHit.name.Contains("Box"))
                {
                    objectHit.gameObject.GetComponent<Renderer>().material.color = new Color(1.0f, 0.0f, 0.0f);
                }
                if (objectHit.name.Contains("New Game"))
                {
                    SceneManager.LoadScene("Tutorial");
                }
                if (objectHit.name.Contains("Level Select"))
                {
                    SceneManager.LoadScene("LevelSelect");
                }
            }
        }
    }
}
