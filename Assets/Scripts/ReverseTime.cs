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
    
    public GameObject[] reversables; // all objects in scene able to be reversed

    private bool timeFoward; // whether time is normal or being reversed

    private ArrayList[] paths; // list of vector3s
    //parallel with reversables: reversables[2] described by paths[2]
    //paths[1][3] refers to the position of object 1 on frame 3

    // TODO: should be transforms to keep rotations

    private ArrayList[] startPoses; // starting positions of all reversables
    //parallel with reversables: startPoses[4] is the start pos of reversables[4]

    // Start is called before the first frame update
    void Start()
    {
        timeFoward = true;
        paths = new ArrayList[reversables.Length]; 
        startPoses = new ArrayList[reversables.Length];
        int i = 0;
        foreach (GameObject go in reversables)
        {
            paths[i] = new ArrayList();
            ArrayList instance = new ArrayList(2);
            instance.Add(go.transform.position);
            instance.Add(go.transform.rotation);
            paths[i].Add(instance);
            startPoses[i] = instance;
            i++;
        }
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
            int i = 0;
            foreach (GameObject go in reversables)
            {
                if (go != null)
                {
                    ArrayList instance = new ArrayList(2);
                    instance.Add(go.transform.position);
                    instance.Add(go.transform.rotation);
                    paths[i].Add(instance);
                }
                i++;
            }
        }
    }

    IEnumerator Reverse()
    {
        for (int pp = 0; pp < paths[0].Count; pp++)
        {
            ArrayList x = (ArrayList) paths[0][pp];
            Debug.Log(x[0] + ", " + x[1]);
        }

        GameObject.Find("Text").GetComponent<Text>().text = "REVERSING TIME";
        timeFoward = false;
        FreezePlayer(true);

        // Want end position of each object so we can just start rather
        //  than being stuck where the player has left us for a while
        Vector3[] endPoses;
        endPoses = new Vector3[reversables.Length];
        int l = 0;
        foreach (GameObject go in reversables)
        {
            if (!go.tag.Equals("Frozen"))
            {
                go.GetComponent<Rigidbody>().velocity = Vector3.zero;
            }
            // TODO: Move this somewhere more rational
            if (go != null)
            {
                endPoses[l] = go.transform.position;
            }
            l++;
        }

        // Now we are going to cycle through all our captured frames
        //  to go back through what happened in time
        // TODO: perhaps not all frames? might be diff length for different objects?
        int i = paths[0].Count - 1; // number of captured frames
        while (i >= 0) {
            bool diff = false; // whether this new frame is actually different from the end frame
            int j = 0; // gameobject (reversables) iterator
            foreach (GameObject go in reversables)
            {
                if (go != null && !go.tag.Equals("Frozen"))
                {
                    ArrayList datum = (ArrayList)paths[j][i]; // datum[0] will be pos, datum[1] will be rot
                    if ((Vector3) datum[0] != endPoses[j]) // check for diff
                    {
                        diff = true; // was different than end

                        //Vector3 objectsVelocity = go.GetComponent<Rigidbody>().velocity;
                        Vector3 objectsPos = go.transform.position;
                        Vector3 desiredPos = (Vector3) datum[0];

                        Vector3 calc = (desiredPos - objectsPos) / Time.deltaTime;
                        calc = calc * 0.3f;
                        go.GetComponent<Rigidbody>().velocity = calc;

                        go.transform.rotation = Quaternion.RotateTowards(go.transform.rotation,
                                                                         (Quaternion)datum[1],
                                                                         90.0f/*Time.deltaTime*/);


                        /* OLD CODE WITH BASICALLY TELEPORT
                        //go.GetComponent<Collider>().isTrigger = true; // allow to go through (TEMPORARY??)
                        go.GetComponent<Rigidbody>().useGravity = false;
                        //go.GetComponent<Rigidbody>().isKinematic = true; // toggle off physics kinda (TEMPORARY??)
                        Quaternion rot = (Quaternion) datum[1]; // remember starting rotation
                        go.transform.LookAt((Vector3) datum[0]); // face towards previous frame
                        float dist = Vector3.Distance(go.transform.position, (Vector3)datum[0]); // get distance to travel
                        go.transform.Translate(Vector3.forward * dist); // travel to previous frame
                        go.transform.rotation = rot;
                        go.transform.rotation = Quaternion.RotateTowards(go.transform.rotation,
                                                                         (Quaternion) datum[1],
                                                                         Time.deltaTime);

                        //go.transform.rotation = rot; // reset rotation (since facing towards botched it)
                                                     // Wanted to remove itmes as below, but instead clearing all after (same?)
                                                     //go.transform.position = (Vector3) paths[j][i];
                                                     //paths[j].Remove(paths[j][i]);
                                                     //paths[j].RemoveAt(i);
                        */
                    }
                }
                j++;
            }
            if (diff) // only if not redoing end for no reason
            {
                yield return new WaitForSeconds(1.0f / 30.0f); // wait 1/30th sec (same time as FixedUpdate IE our capture)
            }
            i--;
        }

        // Reset things for our next time reversal
        int k = 0;
        foreach (GameObject go in reversables)
        {
            if (go != null)
            {
                if (!go.tag.Equals("Frozen"))
                {
                    go.GetComponent<Rigidbody>().velocity = Vector3.zero;

                    paths[k].Clear(); // would rather have removed, see above
                                      //Debug.Log("This should be 0: " + paths[k].Count); // was failing earlier
                    paths[k].Add(startPoses[k]); // replace our staring pos in our list
                                                 //go.GetComponent<Collider>().enabled = true;
                }
                //go.GetComponent<Collider>().isTrigger = false; // turn back to collide-able (TEMPORARY??)
                go.GetComponent<Rigidbody>().useGravity = true;
                //go.GetComponent<Rigidbody>().isKinematic = false; // turn back to physics-able (TEMPORARY??)
            }
            k++;
        }

        GameObject.Find("Text").GetComponent<Text>().text = "Done!";

        yield return new WaitForSeconds(1.0f); // just an extra second of frozen since I think it's nice
        // Note: objects will do their physics from their original position

        GameObject.Find("Text").GetComponent<Text>().text = "";
        timeFoward = true;
        FreezePlayer(false);
    }

    void FreezePlayer(bool freeze)
    {
        // TODO: currently disabling controller altogether, maybe allow looking with mouse?

        Rigidbody rb = GetComponent<Rigidbody>();
        
        if (freeze)
        {
            
            rb.constraints = RigidbodyConstraints.FreezePosition;
            GetComponent<FirstPersonController>().m_RunSpeed = 0.0f;
            GetComponent<FirstPersonController>().m_WalkSpeed = 0.0f;
            GetComponent<FirstPersonController>().m_JumpSpeed = 0.0f;
            //GetComponent<FirstPersonController>().enabled = false;
        }
        else
        {
            rb.constraints = RigidbodyConstraints.None;
            GetComponent<FirstPersonController>().m_RunSpeed = 10.0f;
            GetComponent<FirstPersonController>().m_WalkSpeed = 5.0f;
            GetComponent<FirstPersonController>().m_JumpSpeed = 10.0f;
            //GetComponent<FirstPersonController>().enabled = true;
        }
    }
}
