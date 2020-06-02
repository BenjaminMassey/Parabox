using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class FancyText : MonoBehaviour
{
    // To be placed on Canvas named "Canvas"
    // To be used by other methods via FancyTextHandler (below)

    public Image bg;
    public Text text;

    private string message;
    private float hangtime; // in seconds
    private bool inUse;

    private void Start()
    {
        //bg.enabled = false;
        //text.enabled = false;
    }

    public bool Message(string s, float time, bool force = false) {
        Debug.Log("Got here");
        if (force)
        {
            StopAllCoroutines();
            inUse = false;
        }
        if (!inUse)
        {
            message = s;
            hangtime = time;
            StartCoroutine("PostMessage");
            return true;
        }
        else
        {
            return false;
        }
    }

    private IEnumerator PostMessage()
    {
        inUse = true;

        Color bgc = bg.color;
        bgc.a = 0f;
        bg.color = bgc;

        Color tc = text.color;
        tc.a = 0f;
        text.color = tc;

        bg.enabled = true;
        text.enabled = true;

        text.text = message;

        string[] lines = message.Split('\n');
        int numLines = lines.Length;
        int maxLength = 0;
        foreach (string line in lines)
        {
            maxLength = Mathf.Max(maxLength, line.Length);
        }
        RectTransform rt = bg.GetComponent<RectTransform>();
        Vector2 size = new Vector3(30.0f, 110.0f); // 30px per char, 110px per line
        size.x *= maxLength;
        size.y *= numLines;
        rt.sizeDelta = size;

        int framesToFade = 45;
        float val;
        for (int i = 1; i <= framesToFade; i++) // fade in
        {
            val = (i * (255f / framesToFade)) / 255f;
            //Debug.Log("ALPHA VALUE: " + val);
            bgc.a = val;
            bg.color = bgc;
            tc.a = val;
            text.color = tc;
            yield return new WaitForFixedUpdate();
        }

        yield return new WaitForSeconds(hangtime);

        for (int i = framesToFade; i > 0; i--) // fade out
        {
            val = (i * (255f / framesToFade)) / 255f;
            //Debug.Log("ALPHA VALUE: " + val);
            bgc.a = val;
            bg.color = bgc;
            tc.a = val;
            text.color = tc;
            yield return new WaitForFixedUpdate();
        }

        text.text = "";

        bg.enabled = false;
        text.enabled = false;

        inUse = false;
    }
}

public static class FancyTextHandler {
    public static bool Message(string msg, float time, bool force = false)
    {
        Debug.Log("Sending FancyText message: \"" + msg + "\"");
        return GameObject.Find("Canvas").GetComponent<FancyText>().Message(msg, time, force);
    }
}