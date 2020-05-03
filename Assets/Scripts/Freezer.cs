using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Freezer : MonoBehaviour
{

    public Material frozenMat;

    private bool hasFrozen;
    private GameObject frozenObj;
    private Material origMat;
    private ObjectLists objLists;
    private ReverseTime rt;
    private bool forward;

    // Start is called before the first frame update
    void Start()
    {
        hasFrozen = false;
        frozenObj = null;
        origMat = null;
        objLists = GameObject.Find("GlobalLists").GetComponent<ObjectLists>();
        rt = GameObject.Find("Player").GetComponent<ReverseTime>();
        forward = true;
    }

    // Update is called once per frame
    void Update()
    {
        forward = rt.GetTimeForward();
        if (Input.GetKeyDown(KeyCode.Mouse1) && forward)
        {
            if (hasFrozen)
            {
                List <GameObject> oldRev = objLists.Reversables;
                List <GameObject> newRev = new List<GameObject>(oldRev.Count);
                int i = 0;
                foreach (GameObject oldRevEntry in oldRev)
                {
                    if (!oldRevEntry.tag.Equals("Frozen"))
                    {
                        newRev[i] = oldRevEntry;
                    }
                    else
                    {
                        frozenObj.tag = "Untagged"; // TODO: store old, don't assume none
                        newRev[i] = frozenObj;
                    }
                    i++;
                }
                objLists.Reversables = newRev;
                frozenObj.GetComponent<Renderer>().material = origMat;
                origMat = null;
                hasFrozen = false;
                frozenObj = null;
            }
            else
            {
                //GameObject obj = TestHit();
                GameObject obj = GlobalMethods.TestHit(transform, 10.0f, 0.25f);
                if (obj != null) { Debug.Log("AHHH " + obj.name); }
                
                if (obj != null && objLists.Reversables.Contains(obj))
                {
                    origMat = obj.GetComponent<Renderer>().material;
                    obj.GetComponent<Renderer>().material = frozenMat;
                    hasFrozen = true;
                    frozenObj = obj;
                    List<GameObject> oldRev = objLists.Reversables;
                    List<GameObject> newRev = new List<GameObject>(oldRev.Count);
                    int i = 0;
                    foreach (GameObject oldRevEntry in oldRev)
                    {
                        if (oldRevEntry != obj)
                        {
                            newRev[i] = oldRevEntry;
                        }
                        else
                        {
                            oldRevEntry.tag = "Frozen";
                            newRev[i] = oldRevEntry;
                        }
                        i++;
                    }
                    objLists.Reversables = newRev;
                }
            }
        }
    }
}
