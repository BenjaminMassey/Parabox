using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialMessage : MonoBehaviour
{
    // NOTE: use "<br>" for newlines as entering via inspector is weird

    public string text;

    private void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.name == "Player")
        {
            text = text.Replace("<br>", "\n");
            FancyTextHandler.Message(text, 3f, true);
            Destroy(gameObject);
        }
    }
}
