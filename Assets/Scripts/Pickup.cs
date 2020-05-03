using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    // Allows the player to pickup boxes
    // TODO: change this entire script to be pushing instead?

    // Attached to player

    public Material highlightMat;

    private Material origMat;
    private bool highlighting; // whether an object is highlighted
    private GameObject highlightedObj; // what object is being highlighted (null if nothing)
    // objects do not highlight if time is reversing or if the player is holding something

    private bool holding; // whether player is holding something
    private GameObject heldObj; // what player is holding (null if nothing)
    private ReverseTime rt;
    private ObjectLists objLists;
    private bool forward;

    // Start is called before the first frame update
    void Start()
    {
        highlighting = false;
        holding = false;
        origMat = null;
        rt = GameObject.Find("Player").GetComponent<ReverseTime>();
        objLists = GameObject.Find("GlobalLists").GetComponent<ObjectLists>();
        forward = true;
    }

    // Update is called once per frame
    void Update()
    {
        forward = rt.GetTimeForward();
        if (forward)
        {
            if (!holding)
            {
                //GameObject obj = TestHit(); // looks 3 units straight from where looking
                GameObject obj = GlobalMethods.TestHit(transform, 3.0f, 0.25f);

                // TODO: If an object flies in front of the camera while highlighting, probably will not unhighlight the original
                // highlighted box (impossible to occur atm i think)
                if (obj != null && !highlighting)
                {
                    if (objLists.Pickupables.Contains(obj)) // TODO: implement actual tag feature of pickup-ables
                    {
                        Highlight(obj);
                    }
                }
                else if (obj == null && highlighting)
                {
                    Unhighlight();
                }
            }
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {

                if (!holding)
                {
                    if (highlighting)
                    {
                        Debug.Log("Got " + highlightedObj.name);
                        heldObj = highlightedObj;
                        Unhighlight();
                        heldObj.gameObject.GetComponent<Rigidbody>().useGravity = false;
                        //heldObj.gameObject.GetComponent<Rigidbody>().isKinematic = true;
                        // want object to be able to be moved around freely, so no physics stuff

                    }
                }

                else
                {
                    heldObj.gameObject.GetComponent<Rigidbody>().useGravity = true;
                    //heldObj.gameObject.GetComponent<Rigidbody>().isKinematic = false;
                    // object can have physics again, now that not in hand
                    heldObj = null;
                }
            }
            HandleHolding();
        }
    }

    void Highlight(GameObject obj)
    {
        Debug.Log("highlighted " + obj.name);
        origMat = obj.GetComponent<Renderer>().material;
        obj.GetComponent<Renderer>().material = highlightMat;
        highlighting = true;
        highlightedObj = obj;
        //highlightedObj.GetComponent<Renderer>().material.SetColor("__EmissionColor", Color.red);

    }

    void Unhighlight()
    {
        Debug.Log("unhighlighted " + highlightedObj.name);
        highlightedObj.GetComponent<Renderer>().material = origMat;
        origMat = null;
        highlighting = false;
        highlightedObj = null;
    }

    void HandleHolding()
    {
        // Convert heldObj to boolean
        // TODO: is this boolean even useful?
        if (heldObj == null)
        {
            holding = false;
        }
        else
        {
            holding = true;
        }

        // Move held object in front of player so they can move it
        if (holding)
        {
            Vector3 objectsVelocity = heldObj.GetComponent<Rigidbody>().velocity;
            Vector3 objectsPos = heldObj.transform.position;

            Vector3 origPos = transform.position;
            Transform t = transform;
            t.Translate(Vector3.forward * 3.0f);
            Vector3 desiredPos = t.position;
            transform.position = origPos;

            Vector3 calc = /*Vector3.Normalize(*/(desiredPos - objectsPos/*)*/) / Time.deltaTime;
            calc = calc * 0.3f;
            heldObj.GetComponent<Rigidbody>().velocity = calc;

            heldObj.transform.LookAt(transform);
            /*
            heldObj.transform.position = transform.position;
            heldObj.transform.rotation = transform.rotation;
            heldObj.transform.Translate(Vector3.forward * 3.0f);
            */
        }
    }

    public void StopHolding()
    {
        if (holding)
        {
            holding = false;
            heldObj.gameObject.GetComponent<Rigidbody>().useGravity = true;
            heldObj = null;
        }
    }
}
