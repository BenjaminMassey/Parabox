using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Highlighter : MonoBehaviour
{
    // Adds a highlight effect to boxes being pointed at by player
    // Should be attached to camera-containing sub-object of Player.prefab
    // Note: objects do not highlight if time is reversing or if the player is holding something

    private GameObject highlightedObj; // what object is being highlighted (null if nothing)
    private ReverseTime rt; // reference to see time direction
    private bool forward; // time direction
    private Pickup pu; // reference to see if holding
    private bool holding; // whether player is holding an object
    private Color highlightColor; // how to graphically change highlighted object

    // Start is called before the first frame update
    void Start()
    {
        highlightedObj = null;
        rt = GameObject.Find("Player").GetComponent<ReverseTime>();
        pu = GameObject.Find("Camera").GetComponent<Pickup>();
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
            if (holding)
            {
                if (highlightedObj == null)
                {
                    Highlight(pu.getHeldObj());
                }
            }
            else if (!holding)
            {
                GameObject obj = GlobalMethods.TestHit(transform, 4.0f, 0.25f);

                // TODO: If an object flies in front of the camera while highlighting, probably will not unhighlight the original
                // highlighted box (impossible to occur atm i think)
                // can fix this by just storing what object
            
                if (obj != highlightedObj && highlightedObj != null)
                {
                    Unhighlight();
                }
                else if (obj != null && highlightedObj == null)
                {
                    if (GlobalMethods.ObjectInArray(obj, GlobalMethods.GetReversables()))
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
        // Boxes are fairly complicated, with many planes (for their outline effect)
            // so make sure to look at Box.prefab to see why this makes sense
        int childCount = obj.GetComponent<Transform>().childCount;
        for (int side_iter = 0; side_iter < childCount; side_iter++)
        {
            GameObject side = obj.GetComponent<Transform>().GetChild(side_iter).gameObject;
            int subChildCount = side.GetComponent<Transform>().childCount;
            for (int sidePart_iter = 0; sidePart_iter < subChildCount; sidePart_iter++)
            {
                GameObject sidePart = side.GetComponent<Transform>().GetChild(sidePart_iter).gameObject;
                Renderer r_sp = sidePart.GetComponent<Renderer>();
                r_sp.material.SetColor("_Color", r_sp.material.color + highlightColor);
            }
        }
        highlightedObj = obj;

    }

    void Unhighlight()
    {
        // Boxes are fairly complicated, with many planes (for their outline effect)
        // so make sure to look at Box.prefab to see why this makes sense

        GameObject obj = highlightedObj;

        int childCount = obj.GetComponent<Transform>().childCount;
        for (int side_iter = 0; side_iter < childCount; side_iter++)
        {
            GameObject side = obj.GetComponent<Transform>().GetChild(side_iter).gameObject;
            int subChildCount = side.GetComponent<Transform>().childCount;
            for (int sidePart_iter = 0; sidePart_iter < subChildCount; sidePart_iter++)
            {
                GameObject sidePart = side.GetComponent<Transform>().GetChild(sidePart_iter).gameObject;
                Renderer r_sp = sidePart.GetComponent<Renderer>();
                r_sp.material.SetColor("_Color", r_sp.material.color - highlightColor);
            }
        }

        highlightedObj = null;
    }

}
