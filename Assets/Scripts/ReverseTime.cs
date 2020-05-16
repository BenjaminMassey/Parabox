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
                if (!go.transform.position.ToString().Equals(starts[i].pos.ToString()) ||
                    !go.transform.rotation.ToString().Equals(starts[i].rot.ToString()))
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
                (Vector3 pos, Quaternion rot) instance;
                int j = 0;
                foreach (GameObject go in reversables)
                {
                    if (go != null)
                    {
                        instance = (go.transform.position, go.transform.rotation);
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
        Pickup p = GameObject.Find("FirstPersonCharacter").GetComponent<Pickup>();
        if (p != null) { p.StopHolding(); }
        

        // Now we are going to cycle through all our captured frames
        //  to go back through what happened in time
        // TODO: perhaps not all frames? might be diff length for different objects?
        (Vector3 pos, Quaternion rot) datum; // main path data used throughout
        (Vector3 pos, Quaternion rot) currInfo; // only used in taking out unnecessary frames
        (Vector3 pos, Quaternion rot) prevInfo; // only used in taking out unnecessary frames
        int go_iter; // gameobject (reversables) iterator
        int frame_iter = paths[0].Count - 1; // number of captured frames

        startAlwaysVisible();

        while (frame_iter >= 0) {
            // START TAKE OUT OF UNNECESSARY FRAMES
            bool anyDiff = false;
            go_iter = 0;
            foreach (GameObject go in reversables)
            {
                if (go != null)
                {
                    currInfo = paths[go_iter][frame_iter];
                    if (frame_iter < paths[go_iter].Count - 1) { prevInfo = paths[go_iter][frame_iter + 1]; }
                    else { continue; }
                    if (!currInfo.pos.ToString().Equals(prevInfo.pos.ToString()) ||
                        !currInfo.rot.Equals(prevInfo.rot))
                    {
                        if (!go.tag.Equals("Frozen"))
                        {
                            anyDiff = true;
                            break;
                        }
                    }
                    go_iter++;
                }
            }
            if (!anyDiff)
            {
                frame_iter--;
                continue;
            }
            // END TAKE OUT OF UNNECESSARY FRAMES
            
            go_iter = 0;
            foreach (GameObject go in reversables)
            {
                if (go != null && !go.tag.Equals("Frozen"))
                {
                    datum = paths[go_iter][frame_iter]; // datum.pos will be pos, datum.rot will be rot
                    GlobalMethods.VelocityMove(go, datum.pos, datum.rot);
                }
                go_iter++;
            }
            
            if (Input.GetKey(KeyCode.F))
            {
                frame_iter -= 2; // super speed (questionable?)
            }
            //yield return new WaitForSeconds(1.0f / 30.0f); // wait 1/30th sec (same time as FixedUpdate IE our capture)
            yield return new WaitForFixedUpdate(); // ooooo
            
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
                go.transform.position = starts[go_iter].pos;
                go.transform.rotation = starts[go_iter].rot;

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
