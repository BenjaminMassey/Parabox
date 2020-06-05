using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonDetection : MonoBehaviour
{
    // Gives buttons functionality, including a hook up to door

    // Attached to any button pokey bit (IE "Button" child of "Red Button")
    public AudioSource s_buttonDown;
    public AudioSource s_buttonUp;

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
    }

    IEnumerator ButtonChecker()
    {
        Collider[] cols;
        CapsuleCollider myCap = GetComponent<CapsuleCollider>();
        Vector3 pos = transform.position;
        bool press = false;
        while (true)
        {
            
            cols = Physics.OverlapCapsule(unpressedPos + new Vector3(0f, myCap.height / 2f) + myCap.center,
                                          unpressedPos - new Vector3(0f, myCap.height / 2f) + myCap.center,
                                          myCap.radius);
            press = false;
            foreach (Collider col in cols)
            {
                if (!col.gameObject.name.Equals("Button") && !col.gameObject.name.Contains("Floor") && !col.gameObject.name.Contains("Platform"))
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
        pressed = true;
        transform.position = pressedPos;
        GetComponent<Renderer>().material = pressedMat;
        s_buttonDown.Play();
        bool openDoor = false;
        foreach (GameObject door in doors)
        {
            door.GetComponent<DoorHandler>().AddButton();
            if (door.GetComponent<DoorHandler>().GetNumPressed() == 1)
            {
                openDoor = true; 
            }
        }

        if (openDoor)
        {
            StartCoroutine("OpenDoor");
        }

        
    }

    void Unpress()
    {
        pressed = false;
        transform.position = unpressedPos;
        GetComponent<Renderer>().material = unpressedMat;
        s_buttonUp.Play();
        bool closeDoor = false;
        foreach (GameObject door in doors)
        {
            door.GetComponent<DoorHandler>().SubtractButton();
            if (door.GetComponent<DoorHandler>().GetNumPressed() == 0)
            {
                closeDoor = true;
            }
        }
        if (closeDoor)
        {
            StartCoroutine("CloseDoor");
        }
    }

    IEnumerator OpenDoor()
    {
        foreach (GameObject door in doors)
        {
            door.GetComponent<AudioSource>().Play();
        }

        float seconds = 0.0005f;
        float length = 2.25f;
        float rate = 10.0f;
        for (int frame = 0; frame < rate; frame++)
        {
            MoveDoor(false, length / rate);
            yield return new WaitForSecondsRealtime(seconds / rate);
        }
    }

    IEnumerator CloseDoor()
    {
        foreach (GameObject door in doors)
        {
            door.GetComponent<AudioSource>().Play();
        }

        float seconds = 0.0005f;
        float length = 2.25f;
        float rate = 10.0f;
        for (int frame = 0; frame < rate; frame++)
        {
            MoveDoor(true, length / rate);
            yield return new WaitForSecondsRealtime(seconds / rate);
        }
    }

    void MoveDoor(bool close, float amount)
    {
        if (close)
        {
            amount *= -1.0f;
        }
        foreach (GameObject door in doors)
        {
            //Debug.Log(door.name);
            //Debug.Log(door.transform.childCount);
            for (int i = 0; i < door.transform.childCount; i++)
            {
                Transform child = door.transform.GetChild(i);
                if (child.gameObject.name == "In Left")
                {
                    child.Translate(Vector3.left * amount);
                }
                if (child.gameObject.name == "In Right")
                {
                    child.Translate(Vector3.right * amount);
                }
            }
        }
    }
}
