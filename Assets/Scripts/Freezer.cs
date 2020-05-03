using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Freezer : MonoBehaviour
{

    public Material frozenMat;

    private GameObject frozenObj;
    private Material origMat;
    private List<GameObject> reversables;
    private ReverseTime rt;
    private bool forward;

    // Start is called before the first frame update
    void Start()
    {
        origMat = null;
        reversables = GameObject.Find("GlobalLists").GetComponent<ObjectLists>().Reversables;
        frozenObj = GameObject.Find("GlobalLists").GetComponent<ObjectLists>().FrozenObject;
        rt = GameObject.Find("Player").GetComponent<ReverseTime>();
        forward = true;
    }

    // Update is called once per frame
    void Update()
    {
        forward = rt.GetTimeForward();
        if (Input.GetKeyDown(KeyCode.Mouse1) && forward)
        {
            // Wasn't sure if you wanted it to unfreeze while not targeting the object
            // Makes sense either way, seems unintuitive for it to be able to but
            // more puzzles are possible if you can.
            GameObject obj = GlobalMethods.TestHit(transform, 10.0f, 0.25f);
            if (frozenObj != null)
            {

                if (obj == frozenObj)
                {
                    reversables.Add(frozenObj);
                    frozenObj.GetComponent<Renderer>().material = origMat;
                    origMat = null;
                    frozenObj = null;
                    
                }
            

            }
            else
            {
                if (obj != null) { Debug.Log("AHHH " + obj.name); }
                
                if (obj != null && reversables.Contains(obj))
                {
                    origMat = obj.GetComponent<Renderer>().material;
                    obj.GetComponent<Renderer>().material = frozenMat;
                    frozenObj = obj;
                    reversables.Remove(obj);
                }
            }
        }
    }
}
