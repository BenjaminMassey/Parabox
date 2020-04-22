using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;

public class ReverseTime : MonoBehaviour
{
    public GameObject[] reversables;


    private bool timeFoward;
    private ArrayList[] paths; // list of vector3s
    
    

    // Start is called before the first frame update
    void Start()
    {
        timeFoward = true;
        paths = new ArrayList[reversables.Length];
        int i = 0;
        foreach (GameObject go in reversables)
        {
            paths[i] = new ArrayList();
            paths[i].Add(go.transform.position);
            i++;
        }
    }

    // 30 times a second
    void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            StartCoroutine("Reverse");
        }
        if (timeFoward)
        {
            int i = 0;
            foreach (GameObject go in reversables)
            {
                Vector3 goPos = go.transform.position;
                if (goPos != (Vector3) paths[i][0])
                {
                    paths[i].Add(goPos);
                }
                i++;
            }
        }
    }

    IEnumerator Reverse()
    {
        GameObject.Find("Text").GetComponent<Text>().text = "REVERSING TIME";
        timeFoward = false;
        FreezePlayer(true);

        Vector3[] endPoses;
        endPoses = new Vector3[reversables.Length];
        int l = 0;
        foreach (GameObject go in reversables)
        {
            endPoses[l] = go.transform.position;
            l++;
        }

        int i = paths[0].Count - 1; // number of captured frames
        while (i >= 0) {
            bool diff = false;
            int j = 0;
            foreach (GameObject go in reversables)
            {
                if ((Vector3)paths[j][i] != endPoses[j])
                {
                    //go.GetComponent<Collider>().enabled = false;
                    diff = true;
                    go.GetComponent<Collider>().isTrigger = true;
                    go.GetComponent<Rigidbody>().isKinematic = true;
                    Quaternion rot = go.transform.rotation;
                    go.transform.LookAt((Vector3)paths[j][i]);
                    float dist = Vector3.Distance(go.transform.position, (Vector3)paths[j][i]);
                    go.transform.Translate(Vector3.forward * dist);
                    go.transform.rotation = rot;
                    //go.transform.position = (Vector3) paths[j][i];
                    paths[j].RemoveAt(i);
                    j++;
                }
            }
            if (diff)
            {
                yield return new WaitForSeconds(1.0f / 30.0f);
            }
            i--;
        }
        int k = 0;
        foreach (GameObject go in reversables)
        {
            paths[k].Add(go.transform.position);
            //go.GetComponent<Collider>().enabled = true;
            go.GetComponent<Collider>().isTrigger = false;
            go.GetComponent<Rigidbody>().isKinematic = false;
            k++;
        }
        yield return new WaitForSeconds(1.0f);
        GameObject.Find("Text").GetComponent<Text>().text = "";
        timeFoward = true;
        FreezePlayer(false);
    }

    void FreezePlayer(bool freeze)
    {
        Rigidbody rb = GameObject.Find("Player").GetComponent<Rigidbody>();
        
        if (freeze)
        {
            rb.constraints = RigidbodyConstraints.FreezePosition;
            GameObject.Find("Player").GetComponent<FirstPersonController>().enabled = false;
        }
        else
        {
            rb.constraints = RigidbodyConstraints.None;
            GameObject.Find("Player").GetComponent<FirstPersonController>().enabled = true;
        }
    }
}
