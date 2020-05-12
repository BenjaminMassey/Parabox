using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Highlighter : MonoBehaviour
{

    // Attached to player camera

    public Material highlightMat;
    //private Material origMat;

    private GameObject highlightedObj; // what object is being highlighted (null if nothing)
    // objects do not highlight if time is reversing or if the player is holding something
    
    private ReverseTime rt;
    private bool forward;
    private Pickup pu;
    private bool holding;
    private Color highlightColor;

    // Start is called before the first frame update
    void Start()
    {
        highlightedObj = null;
        //origMat = null;
        rt = GameObject.Find("Player").GetComponent<ReverseTime>();
        pu = GameObject.Find("FirstPersonCharacter").GetComponent<Pickup>();
        forward = true;
        holding = false;
        highlightColor = new Color(0.25f, 0.25f, 0.25f, 0.0f);
        InvokeRepeating("CheckHighlight", 0.0f, 1.0f / 15.0f);
    }

    void CheckHighlight()
    {
        forward = rt.GetTimeForward();
        if (forward)
        {
            if (pu != null)
            {
                holding = pu.IsHolding();
            }
            if (!holding)
            {
                //GameObject obj = TestHit(); // looks 3 units straight from where looking
                GameObject obj = GlobalMethods.TestHit(transform, 3.0f, 0.25f);

                // TODO: If an object flies in front of the camera while highlighting, probably will not unhighlight the original
                // highlighted box (impossible to occur atm i think)
                // can fix this by just storing what obje
                if (obj != highlightedObj && highlightedObj != null)
                {
                    Unhighlight();
                }
                else if (obj != null && highlightedObj == null)
                {
                    Debug.Log("hi");
                    if (GlobalMethods.ObjectInArray(obj, GlobalMethods.GetReversables())) // TODO: add functionality for nonpickupables, and for frozen objects
                                                                                          // (need another material for all possible cases)
                    {
                        Highlight(obj);
                    }
                }

            }
            // if holding an object and highlighting
            else if (highlightedObj != null)
            {
                Unhighlight();
            }
        }
    }

    void Highlight(GameObject obj)
    {
        Debug.Log("highlighted " + obj.name);
        //origMat = obj.GetComponent<Renderer>().material;
        //obj.GetComponent<Renderer>().material = highlightMat;
        Renderer r_obj = obj.GetComponent<Renderer>();
        r_obj.material.SetColor("_Color", r_obj.material.color + highlightColor);
        highlightedObj = obj;
        //highlightedObj.GetComponent<Renderer>().material.SetColor("__EmissionColor", Color.red);

    }

    void Unhighlight()
    {
        Debug.Log("unhighlighted " + highlightedObj.name);
        //highlightedObj.GetComponent<Renderer>().material = origMat;
        //origMat = null;
        Renderer r_obj = highlightedObj.GetComponent<Renderer>();
        r_obj.material.SetColor("_Color", r_obj.material.color - highlightColor);
        highlightedObj = null;
    }

}
