using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Freezer : MonoBehaviour
{

    public Material frozenMat;

    private bool hasFrozen;
    private GameObject frozenObj;
    private Material origMat;
    private ReverseTime rt;
    private bool forward;

    // Start is called before the first frame update
    void Start()
    {
        hasFrozen = false;
        frozenObj = null;
        origMat = null;
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
                GameObject[] oldRev = GameObject.Find("Player").GetComponent<ReverseTime>().reversables;
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
                GameObject.Find("Player").GetComponent<ReverseTime>().reversables = newRev;
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
                
                if (obj != null && obj.name.Contains("Box")) // TODO: better than just == name check
                {
                    origMat = obj.GetComponent<Renderer>().material;
                    obj.GetComponent<Renderer>().material = frozenMat;
                    hasFrozen = true;
                    frozenObj = obj;
                    GameObject[] oldRev = GameObject.Find("Player").GetComponent<ReverseTime>().reversables;
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
                    GameObject.Find("Player").GetComponent<ReverseTime>().reversables = newRev;
                }
            }
        }
    }
}
