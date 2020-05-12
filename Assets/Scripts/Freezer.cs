using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Freezer : MonoBehaviour
{

    //public Material frozenMat;

    private bool hasFrozen;
    private GameObject frozenObj;
    //private Material origMat;
    private ReverseTime rt;
    private bool forward;
    private Color frozenColor;

    // Start is called before the first frame update
    void Start()
    {
        hasFrozen = false;
        frozenObj = null;
        //origMat = null;
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
            if (hasFrozen)
            {
                //frozenObj.GetComponent<Renderer>().material = origMat;
                Renderer r_obj = frozenObj.GetComponent<Renderer>();
                r_obj.material.SetColor("_Color", r_obj.material.color - frozenColor);
                frozenObj.tag = "Untagged"; // TODO: store old, don't assume none
                //origMat = null;
                hasFrozen = false;
                frozenObj = null;
            }
            else
            {
                //GameObject obj = TestHit();
                GameObject obj = GlobalMethods.TestHit(transform, 10.0f, 0.25f);
                if (obj != null) { Debug.Log("AHHH " + obj.name); }

                if (obj != null && obj.name.Contains("Box")) // TODO: better than just == name check
                {
                    //origMat = obj.GetComponent<Renderer>().material;
                    Renderer r_obj = obj.GetComponent<Renderer>();
                    r_obj.material.SetColor("_Color", r_obj.material.color + frozenColor);
                    //obj.GetComponent<Renderer>().material = frozenMat;
                    hasFrozen = true;
                    frozenObj = obj;
                    frozenObj.tag = "Frozen";
                }
            }
        }
    }
}