using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    // Allows the player to pickup boxes
    // Should be attached to camera-containing sub-object of Player.prefab

    private bool holding; // whether player is holding something
    private GameObject heldObj; // what player is holding (null if nothing)
    private ReverseTime rt; // reference to see time direction
    private bool forward; // time direction

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
            if (Input.GetKeyDown(KeyCode.Mouse0) || TooFar())
            {
                if (!holding)
                {
                    GameObject obj = GlobalMethods.TestHit(transform, 4.0f, 0.25f);
                    if (GlobalMethods.ObjectInArray(obj, GlobalMethods.GetPickupables()))
                    {
                        Debug.Log("Got " + obj.name);
                        heldObj = obj;
                        heldObj.gameObject.GetComponent<Rigidbody>().useGravity = false;
                        heldObj.layer = 8;

                    }
                }

                else
                {
                    heldObj.gameObject.GetComponent<Rigidbody>().useGravity = true;
                    heldObj.gameObject.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
                    heldObj.layer = 0;
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

            Vector3 calc = (desiredPos - objectsPos) / Time.deltaTime;
            calc = calc * 0.3f;
            heldObj.GetComponent<Rigidbody>().velocity = calc;

            heldObj.transform.LookAt(transform);
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

    private bool TooFar()
    {
        // Used to drop object if it has gotten too far away from the player
        if (heldObj == null) { return false; }
        float dist = (heldObj.transform.position - transform.position).sqrMagnitude;
        return dist > Mathf.Pow(6.0f, 2f);
    }

    public bool IsHolding()
    {
        return holding;
    }

    public GameObject getHeldObj()
    {
        return heldObj;
    }
}
