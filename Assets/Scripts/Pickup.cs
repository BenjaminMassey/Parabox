using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{

    private bool holding;
    private GameObject heldObj;

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
                GameObject obj = TestHit();
                if (obj != null)
                {
                    Debug.Log("Got " + obj.name);
                    if (obj.name == "Box")
                    {
                        heldObj = obj;
                        heldObj.gameObject.GetComponent<Rigidbody>().isKinematic = true;
                    }
                }
            }
            else
            {
                heldObj.gameObject.GetComponent<Rigidbody>().isKinematic = false;
                heldObj = null;
            }
        }
        HandleHolding();
    }

    GameObject TestHit()
    {
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
        if (heldObj == null)
        {
            holding = false;
        }
        else
        {
            holding = true;
        }

        if (holding)
        {
            heldObj.transform.position = transform.position;
            heldObj.transform.rotation = transform.rotation;
            heldObj.transform.Translate(Vector3.forward * 3.0f);
        }
    }
}
