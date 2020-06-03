using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialMessage : MonoBehaviour
{
    // Tutorial style pop-up messages
    // Uses class FancyTextHandler from FancyText.cs
    // Applied to some "Tutorial Trigger.prefab"
    

    public string text; // text to be displayed in pop-up
    // NOTE: use "<br>" for newlines as entering via inspector is weird

    // Displayed when player enters collider of this "Tutorial Trigger.prefab"
    private void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.name == "Player")
        {
            text = text.Replace("<br>", "\n");
            FancyTextHandler.Message(text, 5f, true);
            Destroy(gameObject);
        }
    }
}
