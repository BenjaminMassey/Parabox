using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxMover : MonoBehaviour
{
    // Allows the player to pickup boxes
    // Should be attached to camera-containing sub-object of Player.prefab

    // SOUND FX
    public AudioSource s_grab;

    public float pickupPower = 0.3f;
    public float pusherPower = 0.1f;

    private bool grabbed; // whether player has grabbed something
    private GameObject movingObj; // what player is grabbed (null if nothing)
    private bool movingObjPush;
    private ReverseTime rt; // reference to see time direction
    private CharacterMover cm; // reference to change speed
    private float defaultMoveSpeed;
    private float defaultJumpForce;
    private bool forward; // time direction

    // Start is called before the first frame update
    void Start()
    {
        grabbed = false;
        movingObj = null;
        movingObjPush = false;
        rt = GameObject.Find("Player").GetComponent<ReverseTime>();
        cm = GameObject.Find("Player").GetComponent<CharacterMover>();
        defaultMoveSpeed = cm.moveSpeed;
        defaultJumpForce = cm.jumpForce;
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
                if (!grabbed)
                {
                    GameObject obj = GlobalMethods.TestHit(transform, 4.0f, 0.25f);
                    if (GlobalMethods.ObjectInArray(obj, GlobalMethods.GetReversables()))
                    {
                        Debug.Log("Got " + obj.name);
                        movingObj = obj;
                        movingObjPush = obj.name.Contains("Push");
                        if (movingObjPush)
                        {
                            cm.moveSpeed = defaultJumpForce / 2.0f;
                            cm.jumpForce = 0.0f;
                        }
                        else
                        {
                            movingObj.gameObject.GetComponent<Rigidbody>().useGravity = false;
                        }
                        movingObj.layer = 8;
                        grabbed = true;
                        s_grab.Play();
                    }
                }

                else
                {
                    if (movingObjPush)
                    {
                        cm.moveSpeed = defaultMoveSpeed;
                        cm.jumpForce = defaultJumpForce;
                    }
                    else
                    {
                        movingObj.gameObject.GetComponent<Rigidbody>().useGravity = true;
                    }
                    movingObj.gameObject.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
                    movingObj.layer = 0;
                    movingObj = null;
                    grabbed = false;
                    s_grab.Play();
                }
            }
            if (grabbed)
            {
                if (movingObjPush)
                {
                    HandlePushing();
                }
                else
                {
                    HandleHolding();
                }
            }
        }
    }

    void HandleHolding()
    {
        // Move held object in front of player so they can move it
        Vector3 spot = GlobalMethods.GetVectorInFront(transform, 3.0f);
        GlobalMethods.VelocityMove(movingObj, spot, new Quaternion(0, 0, 0, 0), pickupPower);
        movingObj.transform.LookAt(transform);
    }

    void HandlePushing()
    {
        // Move held object in front of player so they can move it
        Vector3 spot = GlobalMethods.GetVectorInFront(transform, 3.0f);
        spot = new Vector3(spot.x, movingObj.transform.position.y, spot.z); // preserve y coord
        GlobalMethods.VelocityMove(movingObj, spot, new Quaternion(0, 0, 0, 0), pusherPower);
    }

    public void StopGrabbing()
    {
        if (grabbed)
        {
            grabbed = false;
            if (!movingObjPush)
            {
                movingObj.gameObject.GetComponent<Rigidbody>().useGravity = true;
            }
            movingObj = null;
        }
    }

    private bool TooFar()
    {
        // Used to drop object if it has gotten too far away from the player
        if (movingObj == null) { return false; }
        float dist = (movingObj.transform.position - transform.position).sqrMagnitude;
        return dist > Mathf.Pow(6.0f, 2f);
    }

    public bool HasGrabbed()
    {
        return grabbed;
    }

    public GameObject GetMovingObj()
    {
        return movingObj;
    }
}
