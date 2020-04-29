using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonDetection : MonoBehaviour
{
    // Gives buttons functionality, including a hook up to door

    // Attached to any button pokey bit (IE "Button" child of "Red Button")

    public GameObject[] pressables; // all objects that are allowed to press this button IE Player, Box...
    public Material pressedMat; // for different color when pressed (optional)
    public GameObject[] doors; // doors to be hooked onto this button

    private bool pressed; // whether the button is currently pressed
    private Vector3 unpressedPos; // where the button should be when not pressed
    private Vector3 pressedPos; // where the button should be when pressed
    private Material unpressedMat; // default color (taken at start)

    //private int pressing; // num objs pressing

    // Start is called before the first frame update
    void Start()
    {
        pressed = false;
        unpressedPos = transform.position;
        pressedPos = new Vector3(unpressedPos.x, unpressedPos.y - 0.2f, unpressedPos.z);
        // pressed is simply 0.2f units down
        unpressedMat = GetComponent<Renderer>().material;
        if (pressedMat == null)
        {
            pressedMat = unpressedMat;
        }
        StartCoroutine("ButtonChecker");
        //pressing = 0;
    }

    /*
    private void OnTriggerEnter(Collider col)
    {
        //Debug.Log("Enter");
        foreach (GameObject pressable in pressables) 
        {
            if (pressable == col.gameObject) {
                if (!pressed)
                {
                    Press();
                }
            }
        }
    }

    private void OnTriggerExit(Collider col)
    {
        /*
        //Debug.Log("Exit");
        
        CapsuleCollider myCap = GetComponent<CapsuleCollider>();
        Vector3 pos = transform.position;
        Collider[] colsA = Physics.OverlapCapsule(new Vector3(pos.x, pos.y - myCap.height, pos.z),
                                                 new Vector3(pos.x, pos.y + myCap.height, pos.z),
                                                 myCap.radius);
        
        ArrayList bads = new ArrayList();
        ArrayList colsAL = new ArrayList(colsA);
        
        foreach (object colAL in colsAL)
        {
            string name = ((Collider)colAL).gameObject.name;
            if (!name.Equals("Push Box") || !name.Equals("Box"))
            {
                bads.Add(colAL);
            }
        }

        foreach (object colAL in colsAL)
        {
            string name = ((Collider)colAL).gameObject.name;
            Debug.Log("FIEW " + name);
        }

        foreach (object bad in bads)
        {
            colsAL.Remove(bad);
        }

        Debug.Log("colsAL count: " + colsAL.Count);
        pressing = colsAL.Count;
        if (colsAL.Count <= 0)
        {
            Unpress();
        }
        *//*
        foreach (GameObject pressable in pressables)
        {
            if (pressable == col.gameObject)
            {
                if (pressed)
                {
                    Unpress();
                }
            }
        }
        
    }*/

    // Doors are of the following hiearchy from left to right:
    /* X Door {
     *      Out Left
     *      In Left
     *      In Right
     *      Out Right
     * }
    */
    // this is important when viewing Press() and Unpress()

    IEnumerator ButtonChecker()
    {
        Collider[] cols;
        CapsuleCollider myCap = GetComponent<CapsuleCollider>();
        Vector3 pos = transform.position;
        bool press = false;
        while (true)
        {
            
            cols = Physics.OverlapCapsule(myCap.bounds.min,
                                          myCap.bounds.max,
                                          myCap.radius);
            press = false;
            foreach (Collider col in cols)
            {
                if (!col.gameObject.name.Equals("Button") && !col.gameObject.name.Equals("Floor"))
                {
                    press = true;
                    break;
                }
            }
            if (press && !pressed)
            {
                Press();
            }
            if (!press && pressed)
            {
                Unpress();
            }
            yield return new WaitForSeconds(1.0f / 10.0f);
        }
    }

    void Press()
    {
        /*
        Debug.Log("Press() " + pressing);
        pressing++;
        */
        pressed = true;
        transform.position = pressedPos;
        GetComponent<Renderer>().material = pressedMat;
        foreach (GameObject door in doors)
        {
            Debug.Log(door.name);
            Debug.Log(door.transform.childCount);
            for (int i = 0; i < door.transform.childCount; i++)
            {
                Transform child = door.transform.GetChild(i);
                if (child.gameObject.name == "In Left")
                {
                    child.Translate(Vector3.left * 2.25f);
                }
                if (child.gameObject.name == "In Right")
                {
                    child.Translate(Vector3.right * 2.25f);
                }
            }
        }
    }

    void Unpress()
    {
        /*
        Debug.Log("Unpress() " + pressing);
        pressing--;
        if (pressing <= 0)
        {
            if (pressing < 0) { pressing = 0; }
            Debug.Log("Actual unpress");
            */
            pressed = false;
            transform.position = unpressedPos;
            GetComponent<Renderer>().material = unpressedMat;
            foreach (GameObject door in doors)
            {
                for (int i = 0; i < door.transform.childCount; i++)
                {
                    Transform child = door.transform.GetChild(i);
                    if (child.gameObject.name == "In Left")
                    {
                        child.Translate(Vector3.right * 2.25f);
                    }
                    if (child.gameObject.name == "In Right")
                    {
                        child.Translate(Vector3.left * 2.25f);
                    }
                }
            }
        //}
    }
}
