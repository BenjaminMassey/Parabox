using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    // Allows the player to pickup boxes
    // TODO: change this entire script to be pushing instead?

    // Attached to player

    private bool holding; // whether player is holding something
    private GameObject heldObj; // what player is holding (null if nothing)

    // Start is called before the first frame update
    void Start()
    {
        holding = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (!holding)
            {
                GameObject obj = TestHit(); // looks 3 units straight from where looking
                if (obj != null)
                {
                    //Debug.Log("Got " + obj.name);
                    if (obj.name == "Box") // TODO: implement actual tag feature of pickup-ables
                    {
                        heldObj = obj;
                        heldObj.gameObject.GetComponent<Rigidbody>().isKinematic = true;
                        // want object to be able to be moved around freely, so no physics stuff
                    }
                }
            }
            else
            {
                heldObj.gameObject.GetComponent<Rigidbody>().isKinematic = false;
                // object can have physics again, now that not in hand
                heldObj = null;
            }
        }
        HandleHolding();
    }

    GameObject TestHit()
    {
        // Sees if object is within 3 units of player's looking, returns if so (otherwise null)

        RaycastHit rchit;

        bool hit = Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out rchit, 3.0f);
        if (!hit)
        {
            return null;
        }
        else
        {
            return rchit.collider.gameObject;
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
            heldObj.transform.position = transform.position;
            heldObj.transform.rotation = transform.rotation;
            heldObj.transform.Translate(Vector3.forward * 3.0f);
        }
    }
}
