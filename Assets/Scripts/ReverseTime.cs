using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;

public class ReverseTime : MonoBehaviour
{
    // Handles the main time travel mechanic
    // Will only be keeping position-over-time info of objects placed into reversables
    // Will get position of every reversable 30 seconds every frame
    // Reversing time handled in coroutine, see Reverse()

    // Attached to player

    public Shader AlwaysVisibleShader;
    
    private GameObject[] reversables;

    private Shader[] origShaders;

    private bool timeFoward; // whether time is normal or being reversed

    private List<(Vector3 pos, Quaternion rot)>[] paths; // list of vector3s
    // parallel with reversables: reversables[2] described by paths[2]
    // paths[1][3].pos refers to the position of object 1 on frame 3
    // paths[1][3].rot refers to the rotation of object 1 on frame 3

    private List<(Vector3 pos, Quaternion rot)> starts; // starting positions of all reversables
    // see paths for structure


    // Start is called before the first frame update
    void Start()
    {
        reversables = GlobalMethods.GetReversables();

        timeFoward = true;
        paths = new List<(Vector3 pos, Quaternion rot)>[reversables.Length];
        starts = new List<(Vector3 pos, Quaternion rot)>(reversables.Length);

        (Vector3 pos, Quaternion rot) instance;
        int i = 0;
        foreach (GameObject go in reversables)
        {
            paths[i] = new List<(Vector3 pos, Quaternion rot)>();
            instance = (go.transform.position, go.transform.rotation);
            paths[i].Add(instance);
            starts.Add(instance);
            i++;
        }

        stopAlwaysVisible();
    }

    // Every frame
    void Update()
    {
        if (timeFoward)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                StartCoroutine("Reverse");
            }
        }
    }

    // 30 times a second
    void FixedUpdate()
    {

        if (timeFoward)
        {
            // START TAKE OUT OF UNNECESSARY FRAMES
            bool anyDiff = false;

            (Vector3 pos, Quaternion rot) lastStored; // only used in taking out unnecessary frames
            (Vector3 pos, Quaternion rot) currentSpot; // only used in taking out unnecessary frames

            int go_iter = 0;
            foreach (GameObject go in reversables)
            {
                if (go != null/* && paths[go_iter].Count < frame_iter*/)
                {
                    lastStored = paths[go_iter][paths[go_iter].Count - 1];
                    currentSpot = (go.transform.position, go.transform.rotation);
                    if (!lastStored.pos.ToString().Equals(currentSpot.pos.ToString()) ||
                        !lastStored.rot.Equals(currentSpot.rot))
                    {
                        anyDiff = true;
                        break;
                    }
                    go_iter++;
                }
            }
            // END TAKE OUT UNNECESSARY FRAMES
            if (anyDiff)
            {
                //Debug.Log("AHHHH");
                //String textStr = "WEEEE" + " : " + reversables[0].transform.position + " vs " + (Vector3) startPoses[0][0] + " : " + reversables[0].transform.rotation + " vs " + (Quaternion) startPoses[0][1];
                //GameObject.Find("Text").GetComponent<Text>().text = textStr;
                (Vector3 pos, Quaternion rot) instance;
                int j = 0;
                foreach (GameObject go in reversables)
                {
                    if (go != null)
                    {
                        instance = (go.transform.position, go.transform.rotation);
                        paths[j].Add(instance);
                        //Debug.Log("storing " + paths[j].Count);
                    }
                    j++;
                }
            }
            else
            {
                //GameObject.Find("Text").GetComponent<Text>().text = "WOOOO";
            }
        }
        else
        {
            // START FROZEN CHECK FOR ADDING FRAMES

            bool diff = false;

            (Vector3 pos, Quaternion rot) lastStored; // only used in taking out unnecessary frames
            (Vector3 pos, Quaternion rot) currentSpot; // only used in taking out unnecessary frames
            currentSpot = (Vector3.zero, new Quaternion(0, 0, 0, 0));

            int go_iter = 0;
            foreach (GameObject go in reversables)
            {
                if (go != null && go.tag.Equals("Frozen"))
                {
                    lastStored = paths[go_iter][paths[go_iter].Count - 1];
                    currentSpot = (go.transform.position, go.transform.rotation);
                    if (!lastStored.pos.ToString().Equals(currentSpot.pos.ToString()) ||
                        !lastStored.rot.Equals(currentSpot.rot))
                    {
                        diff = true;
                        break;
                    }
                }
                go_iter++;
            }
            if (diff)
            {
                //Debug.Log("FREEZE STORING " + paths[go_iter].Count + " TO " + reversables[go_iter].tag);
                paths[go_iter].Add(currentSpot);
            }

            // END FROZEN CHECK FOR ADDING FRAMES
        }
    }

    IEnumerator Reverse()
    {

        GameObject.Find("Text").GetComponent<Text>().text = "REVERSING TIME";
        timeFoward = false;
        FreezePlayer(true);
        startAlwaysVisible();

        Pickup p = GameObject.Find("FirstPersonCharacter").GetComponent<Pickup>();
        if (p != null) { p.StopHolding(); }

        // Now we are going to cycle through all our captured frames
        //  to go back through what happened in time
        // TODO: perhaps not all frames? might be diff length for different objects?
        (Vector3 pos, Quaternion rot) datum; // main path data used throughout
        //int frame_iter = paths[0].Count - 1; // number of captured frames
        
        int frame_iter = 0;
        for(int i = 0; i < reversables.Length; i++)
        {
            //Debug.Log("Possible frames: " + paths[i].Count + "(" + i + ") [" + reversables[i].tag + "]");
            if (!reversables[i].tag.Equals("Frozen"))
            {
                frame_iter = Mathf.Max(frame_iter, paths[i].Count - 1);
            }
        }

        Debug.Log("Started off with " + frame_iter + " frames");
        
        int go_iter; // gameobject (reversables) iterator
        while (frame_iter >= 2) {

            bool nonFrozenChanged = false;
            for (int i = 0; i < reversables.Length; i++)
            {
                if (frame_iter + 1 >= paths[i].Count) { // array out-of-bounds: just do it
                    nonFrozenChanged = true;
                    break;
                }
                if (!paths[i][frame_iter].pos.ToString().Equals(paths[i][frame_iter + 1].pos.ToString()))
                {
                    if (!reversables[i].tag.Equals("Frozen"))
                    {
                        nonFrozenChanged = true;
                        break;
                    }
                }
            }

            go_iter = 0;
            foreach (GameObject go in reversables)
            {
                if (go != null && !go.tag.Equals("Frozen") && paths[go_iter].Count - 1 >= frame_iter)
                {
                    datum = paths[go_iter][frame_iter]; // datum.pos will be pos, datum.rot will be rot
                    GlobalMethods.VelocityMove(go, datum.pos, datum.rot);
                    //Debug.Log("move " + frame_iter);
                }
                go_iter++;
            }
            //if (Input.GetKey(KeyCode.F)) { frame_iter -= 2; } // super speed (questionable?)

            if (nonFrozenChanged)
            {
                yield return new WaitForFixedUpdate();
            }
            
            frame_iter--;
        }

        // Reset things for our next time reversal
        go_iter = 0;
        foreach (GameObject go in reversables)
        {
            if (go != null && !go.tag.Equals("Frozen"))
            {

                go.GetComponent<Rigidbody>().velocity = Vector3.zero;

                paths[go_iter].Clear(); // would rather have removed, see above
                //Debug.Log("This should be 0: " + paths[k].Count); // was failing earlier
                // teleport to position: LAZY

                

                float distToStart = Vector3.Distance(go.transform.position, starts[go_iter].pos);
                if (distToStart < 0.5f)
                {
                    go.transform.position = starts[go_iter].pos;
                    go.transform.rotation = starts[go_iter].rot;
                }

                (Vector3 pos, Quaternion rot) currSpot = (go.transform.position, go.transform.rotation);

                starts[go_iter] = currSpot;
                paths[go_iter].Add(starts[go_iter]);

                /* Attempts at non teleport end
                GlobalMethods.VelocityMove(go, starts[go_iter].pos, starts[go_iter].rot);

                go.GetComponent<Rigidbody>().velocity = Vector3.zero;

                (Vector3 pos, Quaternion rot) instance = (go.transform.position, go.transform.rotation);
                starts[go_iter] = instance;
                */

                go.GetComponent<Rigidbody>().useGravity = true;
            }
            go_iter++;
        }

        stopAlwaysVisible();

        GameObject.Find("Text").GetComponent<Text>().text = "Done!";

        yield return new WaitForSeconds(1.0f); // just an extra second of frozen since I think it's nice
        // Note: objects will do their physics from their original position

        GameObject.Find("Text").GetComponent<Text>().text = "";
        timeFoward = true;
        FreezePlayer(false);
    }

    void FreezePlayer(bool freeze)
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        
        if (freeze)
        {
            
            rb.constraints = RigidbodyConstraints.FreezePosition;
            rb.isKinematic = true;
            GetComponent<CapsuleCollider>().enabled = false;
        }
        else
        {
            rb.constraints = RigidbodyConstraints.None;
            rb.constraints = RigidbodyConstraints.FreezeRotation;
            rb.isKinematic = false;
            GetComponent<CapsuleCollider>().enabled = true;
        }
    }

    void startAlwaysVisible()
    {
        foreach (GameObject go in reversables)
        {
            go.GetComponent<Renderer>().sharedMaterial.shader = AlwaysVisibleShader;
        }
    }

    void stopAlwaysVisible()
    {

        foreach (GameObject go in reversables)
        {
            //go.GetComponent<Renderer>().sharedMaterial.EnableKeyword("_ALPHATEST_ON");
            //go.GetComponent<Renderer>().sharedMaterial.EnableKeyword("_ALPHABLEND_ON");

            go.GetComponent<Renderer>().sharedMaterial.shader = Shader.Find("Standard");
            go.GetComponent<Renderer>().sharedMaterial.EnableKeyword("_ALPHAPREMULTIPLY_ON");
            go.GetComponent<Renderer>().sharedMaterial.renderQueue = 3000;

        }
    }

    public bool GetTimeForward()
    {
        return timeFoward;
    }

    private void OnApplicationQuit()
    {
        stopAlwaysVisible();
    }
}
