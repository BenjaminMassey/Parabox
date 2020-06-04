using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    // Allows the player to pickup boxes
    // Should be attached to camera-containing sub-object of Player.prefab

    // SOUND FX
    public AudioSource s_pickup;

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
                        s_pickup.Play();
                    }
                }

                else
                {
                    heldObj.gameObject.GetComponent<Rigidbody>().useGravity = true;
                    heldObj.gameObject.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
                    heldObj.layer = 0;
                    heldObj = null;
                    s_pickup.Play();
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
            Vector3 spot = GlobalMethods.GetVectorInFront(transform, 3.0f);
            GlobalMethods.VelocityMove(heldObj, spot, new Quaternion(0, 0, 0, 0), 0.1f);
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
