using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Freezer : MonoBehaviour
{
    // Allows the player to freeze an object in time with right-click
    // Should be attached to camera-containing sub-object of Player.prefab

    private bool hasFrozen; // some object has been frozen
    private GameObject frozenObj; // which object has been frozen (null if none)
    private ReverseTime rt; // reference in order to see time direction
    private bool forward; // time direction
    private Color frozenColor; // how to graphically change frozen object

    // Start is called before the first frame update
    void Start()
    {
        hasFrozen = false;
        frozenObj = null;
        rt = GameObject.Find("Player").GetComponent<ReverseTime>();
        forward = true;
        frozenColor = new Color(0.0f, 1.0f, 1.0f, 0.0f);
    }

    // Update is called once per frame
    void Update()
    {
        forward = rt.GetTimeForward();
        if (Input.GetKeyDown(KeyCode.Mouse1) && forward)
        {
            GameObject obj = GlobalMethods.TestHit(transform, 5.0f, 0.25f);
            if (obj != null && GlobalMethods.ObjectInArray(obj, GlobalMethods.GetReversables()))
            {
                GameObject unfrozenObj = null;

                if (hasFrozen)
                {
                    // unfreezes
                    Renderer r_obj = frozenObj.GetComponent<Renderer>();
                    r_obj.material.SetColor("_Color", r_obj.material.color - frozenColor);
                    frozenObj.tag = "Untagged"; // TODO: store old, don't assume none
                    hasFrozen = false;
                    unfrozenObj = frozenObj;
                    frozenObj = null;
                }

                if (!hasFrozen && unfrozenObj != obj)
                {
                    // freezes
                    Renderer r_obj = obj.GetComponent<Renderer>();
                    r_obj.material.SetColor("_Color", r_obj.material.color + frozenColor);
                    hasFrozen = true;
                    frozenObj = obj;
                    frozenObj.tag = "Frozen";
                }

            }
        }
    }
}