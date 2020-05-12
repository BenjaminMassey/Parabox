using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    // Allows the player to pickup boxes
    // TODO: change this entire script to be pushing instead?

    // Attached to player camera


    private bool holding; // whether player is holding something
    private GameObject heldObj; // what player is holding (null if nothing)
    private ReverseTime rt;
    private bool forward;

    // Start is called before the first frame update
    void Start()
    {
        holding = false;
        rt = GameObject.Find("Player").GetComponent<ReverseTime>();
        forward = true;
    }

    // Update is called once per frame
    void Update()
    {
        forward = rt.GetTimeForward();
        if (forward)
        {

            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                if (!holding)
                {
                    GameObject obj = GlobalMethods.TestHit(transform, 5.0f, 0.25f);
                    if (GlobalMethods.ObjectInArray(obj, GlobalMethods.GetPickupables()))
                    {
                        Debug.Log("Got " + obj.name);
                        heldObj = obj;
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

    public bool IsHolding()
    {
        return holding;
    }
}
