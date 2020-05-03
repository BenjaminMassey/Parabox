using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectLists : MonoBehaviour
{
    // Changed these to lists so that I could use Contains method
    // List of interactable objects with frozen object(s) excluded
    public List<GameObject> Reversables;
    public GameObject FrozenObject;
    public List<GameObject> Pickupables;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
