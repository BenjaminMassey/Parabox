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
    }

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
        //Debug.Log("Exit");
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
    }

    // Doors are of the following hiearchy from left to right:
    /* X Door {
     *      Out Left
     *      In Left
     *      In Right
     *      Out Right
     * }
    */
    // this is important when viewing Press() and Unpress()

    void Press()
    {
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
    }
}
