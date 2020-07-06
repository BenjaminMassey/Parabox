using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeLimit : MonoBehaviour
{
    ReverseTime rt;
    
    // Times in seconds
    int first_warning = 240; // 240 production
    int second_warning = 270; // 270 production
    int force_reverse = 300; // 300 production

    bool first_warned;
    bool second_warned;

    // Start is called before the first frame update
    void Start()
    {
        rt = GameObject.Find("Player").GetComponent<ReverseTime>();
        first_warned = false;
        second_warned = false;
    }

    // FixedUpdate is called 50 times a second
    void FixedUpdate()
    {
        if (rt.GetTimeForward())
        {
            int frame_count = rt.GetNumFramesRecorded();
            int seconds = Mathf.RoundToInt(frame_count / 50.0f);
            if (seconds > force_reverse && first_warned && second_warned)
            {
                FancyTextHandler.Message("TOO MUCH HISTORY:\nFORCING REVERSAL", 5.0f, true);
                rt.OutsideReverse();
                first_warned = false;
                second_warned = false;
            }
            else if (seconds > second_warning && first_warned && !second_warned)
            {
                Debug.Log("TimeLimit.cs Second Warning");
                rt.StartCoroutine("Warning");
                second_warned = true;
            }
            else if (seconds > first_warning && !first_warned)
            {
                Debug.Log("TimeLimit.cs First Warning");
                rt.StartCoroutine("Warning");
                first_warned = true;
            }
        }
        
    }
}
