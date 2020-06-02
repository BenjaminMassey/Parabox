using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuClicker : MonoBehaviour
{
    // https://docs.unity3d.com/Manual/CameraRays.html

    public Camera cam;

    private string lvlname;
    private Transform obj;
    private bool verified;

    void Start()
    {

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            RaycastHit hit;
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                verified = false;
                Transform objectHit = hit.transform;
                if (objectHit != null)
                {
                    obj = objectHit;
                    if (objectHit.name.Contains("Box"))
                    {
                        objectHit.gameObject.GetComponent<Renderer>().material.color = new Color(1.0f, 0.0f, 0.0f);
                    }
                    if (objectHit.name.Contains("New Game"))
                    {
                        lvlname = "Tutorial";
                        verified = true;
                    }
                    else if (objectHit.name.Contains("Level"))
                    {
                        string x = objectHit.name.Replace(" ", "");
                        x = x.Replace("Collider", "");
                        lvlname = x;
                        verified = true;
                    }
                    if (objectHit.name.Contains("Go Back"))
                    {
                        lvlname = "MainMenu";
                        verified = true;
                    }
                    if (verified)
                    {
                        StartCoroutine("Press");
                    }
                }
            }
        }
    }

    IEnumerator Press()
    {
        Color orig_col = obj.GetComponent<Renderer>().material.color;
        Color new_col = Color.white;
        // Use obj to change to press color
        obj.GetComponent<Renderer>().material.color = new_col;
        yield return new WaitForSecondsRealtime(1.0f / 10.0f);
        // Use obj to change back
        obj.GetComponent<Renderer>().material.color = orig_col;
        yield return new WaitForSecondsRealtime(1.0f / 10.0f);
        if (lvlname.Equals("Tutorial"))
        {
            Debug.Log("Started Timer");
            TimeKeep.Start();
        }
        SceneManager.LoadScene(lvlname);
    }
}
