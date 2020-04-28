using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button2 : MonoBehaviour
{

    private Vector3 start;

    // Start is called before the first frame update
    void Start()
    {
        start = transform.position;
        StartCoroutine("BeDo");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator BeDo()
    {
        Quaternion trash = new Quaternion(0, 0, 0, 0);
        while (true)
        {
            //Debug.Log("Woo");
            if (!transform.position.Equals(start))
            {
                //Debug.Log("Wee");
                GlobalMethods.VelocityMove(gameObject, start, trash);
            }
            yield return new WaitForSeconds(1.0f / 6.0f);
        }
    }
}
