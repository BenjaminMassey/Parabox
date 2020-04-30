using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;
using UnityEngine.SceneManagement;

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
    // parallel with reversables: reversables[2] described by paths[2]
    // paths[1][3][0] refers to the position of object 1 on frame 3
    // paths[1][3][1] refers to the rotation of object 1 on frame 3

    // TODO: should be transforms to keep rotations

    private ArrayList[] startPoses; // starting positions of all reversables
    // see paths for structure

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
        if (Input.GetKeyDown(KeyCode.G))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    // 30 times a second
    void FixedUpdate()
    {
        
        if (timeFoward)
        {
            bool anyDiff = false;
            int i = 0;
            foreach (GameObject go in reversables)
            {
                if (!go.transform.position.ToString().Equals(((Vector3) startPoses[i][0]).ToString()) ||
                    !go.transform.rotation.ToString().Equals(((Quaternion)startPoses[i][1]).ToString()))
                {
                    anyDiff = true;
                    break;
                }
                i++;
            }
            if (anyDiff)
            {
                //Debug.Log("AHHHH");
                //String textStr = "WEEEE" + " : " + reversables[0].transform.position + " vs " + (Vector3) startPoses[0][0] + " : " + reversables[0].transform.rotation + " vs " + (Quaternion) startPoses[0][1];
                //GameObject.Find("Text").GetComponent<Text>().text = textStr;
                int j = 0;
                foreach (GameObject go in reversables)
                {
                    if (go != null)
                    {
                        ArrayList instance = new ArrayList(2);
                        instance.Add(go.transform.position);
                        instance.Add(go.transform.rotation);
                        paths[j].Add(instance);
                    }
                    j++;
                }
            }
            else
            {
                //GameObject.Find("Text").GetComponent<Text>().text = "WOOOO";
            }
        }
    }

    IEnumerator Reverse()
    {

        GameObject.Find("Text").GetComponent<Text>().text = "REVERSING TIME";
        timeFoward = false;
        FreezePlayer(true);
        GameObject.Find("FirstPersonCharacter").GetComponent<Pickup>().StopHolding();

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
        ArrayList datum = new ArrayList() { Vector3.zero, null };
        ArrayList currInfo;
        ArrayList prevInfo;
        int i = paths[0].Count - 1; // number of captured frames
        while (i >= 0) {
            // START TAKE OUT OF UNNECESSARY FRAMES
            bool anyDiff = false;
            int iter = 0;
            // currInfo and prevInfo are ArrayLists (see before while)
            foreach (GameObject go in reversables)
            {
                currInfo = (ArrayList)paths[iter][i];
                if (i < paths[iter].Count - 1) { prevInfo = (ArrayList)paths[iter][i + 1]; }
                else { continue; }
                if (!((Vector3)currInfo[0]).ToString().Equals(((Vector3)prevInfo[0]).ToString()) ||
                    !((Quaternion)currInfo[1]).Equals(((Quaternion)prevInfo[1])))
                {
                    anyDiff = true;
                    break;
                }
                iter++;
            }
            if (!anyDiff)
            {
                i--;
                continue;
            }
            // END TAKE OUT OF UNNECESSARY FRAMES

            bool ediff = false; // whether this new frame is different from the end frame
            bool sdiff = false;
            int j = 0; // gameobject (reversables) iterator
            foreach (GameObject go in reversables)
            {
                if (go != null && !go.tag.Equals("Frozen"))
                {
                    datum = (ArrayList)paths[j][i]; // datum[0] will be pos, datum[1] will be rot
                    if ((Vector3) datum[0] != endPoses[j]) // check for diff
                    {
                        ediff = true; // was different than end
                        GlobalMethods.VelocityMove(go, (Vector3)datum[0], (Quaternion)datum[1]);
                    }
                }
                j++;
            }
            if (ediff) // only if not redoing end for no reason
            {
                if (!sdiff)
                {
                    if (Input.GetKey(KeyCode.F))
                    {
                        i -= 2; // super speed (questionable?)
                    }
                    //yield return new WaitForSeconds(1.0f / 30.0f); // wait 1/30th sec (same time as FixedUpdate IE our capture)
                    yield return new WaitForFixedUpdate(); // ooooo
                }
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
                    // teleport to position: LAZY
                    go.transform.position = (Vector3) startPoses[k][0];
                    go.transform.rotation = (Quaternion) startPoses[k][1];

                    paths[k].Add(startPoses[k]); // replace our staring pos in our list
                }

                go.GetComponent<Rigidbody>().useGravity = true;
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
            GetComponent<CapsuleCollider>().enabled = false;
            GetComponent<CharacterController>().enabled = false;
            //GetComponent<FirstPersonController>().enabled = false;
        }
        else
        {
            rb.constraints = RigidbodyConstraints.None;
            GetComponent<FirstPersonController>().m_RunSpeed = 10.0f;
            GetComponent<FirstPersonController>().m_WalkSpeed = 5.0f;
            GetComponent<FirstPersonController>().m_JumpSpeed = 10.0f;
            GetComponent<CapsuleCollider>().enabled = true;
            GetComponent<CharacterController>().enabled = true;
            //GetComponent<FirstPersonController>().enabled = true;
        }
    }

    public bool GetTimeForward()
    {
        return timeFoward;
    }
}
