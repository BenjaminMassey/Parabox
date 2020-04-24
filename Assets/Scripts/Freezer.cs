using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Freezer : MonoBehaviour
{

    public Material frozenMat;

    private bool hasFrozen;
    private GameObject frozenObj;
    private Material origMat;

    // Start is called before the first frame update
    void Start()
    {
        hasFrozen = false;
        frozenObj = null;
        origMat = null;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            if (hasFrozen)
            {
                GameObject[] oldRev = GetComponent<ReverseTime>().reversables;
                GameObject[] newRev = new GameObject[oldRev.Length];
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
                GetComponent<ReverseTime>().reversables = newRev;
                frozenObj.GetComponent<Renderer>().material = origMat;
                origMat = null;
                hasFrozen = false;
                frozenObj = null;
            }
            else
            {
                //GameObject obj = TestHit();
                GameObject obj = GlobalMethods.TestHit(transform, 10.0f, 0.25f);
                if (obj != null && obj.name == "Box") // TODO: better than just == name check
                {
                    origMat = obj.GetComponent<Renderer>().material;
                    obj.GetComponent<Renderer>().material = frozenMat;
                    hasFrozen = true;
                    frozenObj = obj;
                    GameObject[] oldRev = GetComponent<ReverseTime>().reversables;
                    GameObject[] newRev = new GameObject[oldRev.Length];
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
                    GetComponent<ReverseTime>().reversables = newRev;
                }
            }
        }
    }
}
