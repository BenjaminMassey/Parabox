using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;

public class ReverseTime : MonoBehaviour
{
    // Handles the main time travel mechanic
    // Will only be keeping position-over-time info of objects placed into reversables
    // Will get position of every reversable 50 times each second (FixedUpdate)
    // Reversing time handled in coroutine, see Reverse()

    // VISUAL FX
    public GameObject globalPostVolume; // holds our post processing

    // SOUND FX
    public AudioSource s_reverseTime;
    public AudioSource s_reverseTimeB;

    // Attached to player
    
    private GameObject[] reversables; // list of everything we will try to reverse

    private Shader[] origShaders; // original shaders to revert back to from AlwaysVisibleShader

    private bool timeFoward; // whether time is normal or being reversed

    private List<(Vector3 pos, Quaternion rot)>[] paths; // list of vector3s
    // parallel with reversables: reversables[2] described by paths[2]
    // paths[1][3].pos refers to the position of object 1 on frame 3
    // paths[1][3].rot refers to the rotation of object 1 on frame 3

    private List<(Vector3 pos, Quaternion rot)> starts; // starting positions of all reversables
    // see paths for structure

    // Used as "null" spots in our data
    private (Vector3 pos, Quaternion rot) dummy = (new Vector3(99.0f, 99.0f, 99.0f), new Quaternion(99.0f, 99.0f, 99.0f, 99.0f));

    private bool keep_time;

    // Start is called before the first frame update
    void Start()
    {
        reversables = GlobalMethods.GetReversables();

        foreach (GameObject go in reversables)
        {
            go.layer = 0;
        }

        timeFoward = true;
        paths = new List<(Vector3 pos, Quaternion rot)>[reversables.Length];
        starts = new List<(Vector3 pos, Quaternion rot)>(reversables.Length);

        // Set up our starting positions
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

        keep_time = false;
        StartCoroutine("KeepTimeDelay");
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
        if (!keep_time)
        {
            return;
        }
        // While time is forward, will keep track of all movement
        if (timeFoward)
        {
            // START IGNORE OF UNNECESSARY FRAMES
            bool anyDiff = false;

            (Vector3 pos, Quaternion rot) lastStored; // only used in taking out unnecessary frames
            (Vector3 pos, Quaternion rot) currentSpot; // only used in taking out unnecessary frames

            int go_iter = 0;
            foreach (GameObject go in reversables)
            {
                if (go != null)
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
            // END IGNORE UNNECESSARY FRAMES
            // Now we can actually store our frame data, if proven necessary
            if (anyDiff)
            {
                (Vector3 pos, Quaternion rot) instance;
                int j = 0;
                foreach (GameObject go in reversables)
                {
                    if (go != null)
                    {
                        instance = (go.transform.position, go.transform.rotation);
                        paths[j].Add(instance);
                        Debug.Log("storing for \"" + go.name + "\" (count " + paths[j].Count + ")");
                    }
                    j++;
                }
            }
        }
        // While time is backward, will only keep track of frozen objects
            // (perhaps being moved by others: would have to remember)
        else
        {
            // START IGNORE OF UNNECESSARY FRAMES
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
            // END IGNORE UNNECESSARY FRAMES
            // Now we can store this special frozen info, if necessary
            if (diff)
            {
                //Debug.Log("FREEZE STORING " + paths[go_iter].Count + " TO " + reversables[go_iter].tag);
                paths[go_iter].Add(currentSpot);
            }
        }
    }

    IEnumerator Reverse()
    {
        // This is where we do the main thing: reversing time
        // We will see how many frames have been collected
            // (max count of frames from objects) start at 
            // that number, and work our way down to zero

        // Setup
        //GameObject.Find("Text").GetComponent<Text>().text = "REVERSING TIME";
        timeFoward = false;
        s_reverseTime.Play();
        FreezePlayer(true);
        
        StartCoroutine("EnablePostProcessing");

        // Drop object if one is being held
        BoxMover p = GameObject.Find("Camera").GetComponent<BoxMover>();
        if (p != null) { p.StopGrabbing(); }

        // Now we are going to cycle through all our captured frames
        //  to go back through what happened in time
        (Vector3 pos, Quaternion rot) datum; // main path data used throughout
        
        // Figure out how many frames to use
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
        
        // Main loop :)
        int go_iter; // gameobject (reversables) iterator (reused frequently)
        while (frame_iter >= 2) {
            // See if any (non-frozen) objects have actually changed
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

            // Actually move our objects
            go_iter = 0;
            foreach (GameObject go in reversables)
            {

                if (go != null && !go.tag.Equals("Frozen") && paths[go_iter].Count - 1 >= frame_iter)
                {
                    if (paths[go_iter][frame_iter] != dummy)
                    {
                        datum = paths[go_iter][frame_iter]; // datum.pos will be pos, datum.rot will be rot
                        GlobalMethods.VelocityMove(go, datum.pos, datum.rot);
                        //Debug.Log("move " + frame_iter);
                    }
                }
                go_iter++;
            }

            // Wait the same time we recorded, if there was a change we care about
            if (nonFrozenChanged)
            {
                yield return new WaitForFixedUpdate();
            }
            
            frame_iter--;
        }

        // Clear paths: needs own loop (also grab max while at it)
        int max_paths_size = 0;
        go_iter = 0;
        foreach (GameObject go in reversables)
        {
            if (go != null && !go.tag.Equals("Frozen"))
            {
                Debug.Log("clear");
                paths[go_iter].Clear();
            }
            max_paths_size = Mathf.Max(max_paths_size, paths[go_iter].Count);
            go_iter++;
        }
        Debug.Log("Max: " + max_paths_size);

        // Reset things for our next time reversal
        go_iter = 0;
        foreach (GameObject go in reversables)
        {
            if (go != null && !go.tag.Equals("Frozen"))
            {
                go.GetComponent<Rigidbody>().velocity = Vector3.zero;
                go.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;

                //paths[go_iter].Clear(); MOVED TO BEFORE

                // If close enough to previous start, then teleport there
                float distToStart = Vector3.Distance(go.transform.position, starts[go_iter].pos);
                if (distToStart < 0.5f)
                {
                    go.transform.position = starts[go_iter].pos;
                    go.transform.rotation = starts[go_iter].rot;
                    //Debug.Log("object" + go + "returned to location" + starts[go_iter].pos);
                }

                // Store our new start (should be the same if above happened)
                (Vector3 pos, Quaternion rot) currSpot = (go.transform.position, go.transform.rotation);

                starts[go_iter] = currSpot;
                //Debug.Log("object" + go + "now starts at location" + starts[go_iter].pos);
                
                paths[go_iter].Add(starts[go_iter]);
                for (int i = 0; i < max_paths_size; i++)
                {
                    paths[go_iter].Add(dummy);
                }

                go.GetComponent<Rigidbody>().useGravity = true;
            }
            go_iter++;
        }
        
        StartCoroutine("DisablePostProcessing");

        //GameObject.Find("Text").GetComponent<Text>().text = "Done!";

        s_reverseTimeB.Play();

        yield return new WaitForSeconds(1.0f); // just an extra second of frozen since I think it's nice
        // Note: objects will do their physics from their original position

        //GameObject.Find("Text").GetComponent<Text>().text = "";
        timeFoward = true;
        FreezePlayer(false);

        go_iter = 0;
        foreach (GameObject go in reversables)
        {
            Debug.Log("Size [" + go_iter + "]: " + paths[go_iter].Count);
            go_iter++;
        }

    }

    void FreezePlayer(bool freeze)
    {
        // Makes sure the player cannot move during time freeze
            // (but can look around)
        
        Rigidbody rb = GetComponent<Rigidbody>();
        
        if (freeze)
        {
            // Player can no longer interact with objects
            foreach (GameObject go in reversables)
            {
                go.layer = 11;
            }
            rb.constraints = RigidbodyConstraints.FreezePosition;
            rb.isKinematic = true;
            //GetComponent<CapsuleCollider>().enabled = false;
        }
        else
        {
            // Player can now interact with objects again
            foreach (GameObject go in reversables)
            {
                go.layer = 0;
            }
            rb.constraints = RigidbodyConstraints.None;
            rb.constraints = RigidbodyConstraints.FreezeRotation;
            rb.isKinematic = false;
            //GetComponent<CapsuleCollider>().enabled = true;
        }
    }

    IEnumerator EnablePostProcessing()
    {
        PostProcessVolume ppv = globalPostVolume.GetComponent<PostProcessVolume>();
        ppv.weight = 0;
        for (int i = 0; i < 10; i++)
        {
            ppv.weight += (1f / 10f);
            yield return new WaitForFixedUpdate();
        }
        ppv.weight = 1;
    }

    IEnumerator DisablePostProcessing()
    {
        PostProcessVolume ppv = globalPostVolume.GetComponent<PostProcessVolume>();
        ppv.weight = 1;
        for (int i = 0; i < 10; i++)
        {
            ppv.weight -= (1f / 10f);
            yield return new WaitForFixedUpdate();
        }
        ppv.weight = 0;
    }

    // TODO: this shouldn't be needed with correct logic,
        // but for some reason there are around ~20 dummy
        // frames upon game start (which are ignored, but
        // they are scary so don't want them at all)
    IEnumerator KeepTimeDelay()
    {
        for (int _ = 0; _ < 25; _++)
        {
            yield return new WaitForFixedUpdate();
        }
        keep_time = true;
        Debug.Log("Keeping time as of now!");
    }

    public bool GetTimeForward()
    {
        return timeFoward;
    }
}
