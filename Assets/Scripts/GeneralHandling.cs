using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GeneralHandling : MonoBehaviour
{

    Text t;
    int defaultFont;
    // Start is called before the first frame update
    void Start()
    {
        t = GameObject.Find("Text").GetComponent<Text>();
        defaultFont = t.fontSize;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            StartCoroutine("EscMessage");
        }
    }

    IEnumerator EscMessage()
    {
        t.text = "MOUSE IS FREE";
        t.fontSize = defaultFont + 6;
        yield return new WaitForSeconds(10.0f);
        if (t.text.Equals("MOUSE IS FREE"))
        {
            t.text = "";
        }
        t.fontSize = defaultFont - 6;
    }
}
