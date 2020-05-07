using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectLists : MonoBehaviour
{
    // Changed these to lists so that I could use Contains method
    // List of interactable objects with frozen object(s) excluded
    public GameObject[] Reversables; // set in ReverseTime
    //public GameObject FrozenObject; // Don't think this makes much sense as non Freezer.cs handling should be by tag
    public GameObject[] Pickupables;
    // Start is called before the first frame update
    void Start()
    {
        GlobalMethods.OL = gameObject.GetComponent<ObjectLists>();//this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
