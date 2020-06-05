using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectLists : MonoBehaviour
{
    // Container to dump related objects into
    // Should be attached to an empty GameObject
        // (generally called "GlobalLists")
    // All of this will be functionally used by other scripts
        // in GlobalMethods.cs
    
    public GameObject[] Reversables;
    public GameObject[] Pickupables;

    // Start is called before the first frame update
    void Start()
    {
        GlobalMethods.OL = gameObject.GetComponent<ObjectLists>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
