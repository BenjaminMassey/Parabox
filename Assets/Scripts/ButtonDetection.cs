using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonDetection : MonoBehaviour
{

    public GameObject[] pressables;
    public Material pressedMat;
    public GameObject door;

    private bool pressed;
    private Vector3 unpressedPos;
    private Vector3 pressedPos;
    private Material unpressedMat;

    // Start is called before the first frame update
    void Start()
    {
        pressed = false;
        unpressedPos = transform.position;
        pressedPos = new Vector3(unpressedPos.x, unpressedPos.y - 0.2f, unpressedPos.z);
        unpressedMat = GetComponent<Renderer>().material;
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

    void Press()
    {
        pressed = true;
        transform.position = pressedPos;
        GetComponent<Renderer>().material = pressedMat;
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

    void Unpress()
    {
        pressed = false;
        transform.position = unpressedPos;
        GetComponent<Renderer>().material = unpressedMat;
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
