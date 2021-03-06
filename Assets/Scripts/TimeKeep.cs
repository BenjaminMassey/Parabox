using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class TimeKeep
{
    // Handles keeping track of the amount of time it has taken
        // the player to beat the game
    
    // MenuClicker.cs uses Start() to start the timer
    // LevelEnder.cs uses End() to end the timer
    // TimerDisplay.cs uses GetTime() to display the time

    public static bool first = true; // timer never started: used by TimerDisplay.cs

    private static float start = -1.0f; // by fixed frames
    private static float end = -1.0f;

    private static void SetStart()
    {
        start = Time.realtimeSinceStartup;
    }

    private static void SetEnd()
    {
        end = Time.realtimeSinceStartup;
    }

    public static void Start()
    {
        SetStart();
        end = -1.0f;
        first = false;
    }

    public static void End()
    {
        SetEnd();
    }

    public static string GetTime()
    {
        float t = GetTimeReal();
        if (t != -1.0f)
        {
            return BuildOutput(t);
        }
        else
        {
            return "Timer Unstarted";
        }
    }

    private static float GetTimeReal()
    {
        if (start == -1.0f)
        {
            return -1.0f;
        }
        if (end == -1.0f)
        {
            return Time.realtimeSinceStartup - start;
        }
        else
        {
            return end - start;
        }
    }

    private static string BuildOutput(float x)
    {
        float rounded = (Mathf.Round(x * 1000.0f) / 1000.0f);
        string time = rounded.ToString();
        string[] secondsANDmilli = time.Split('.');
        int minutes = int.Parse(secondsANDmilli[0]) / 60;
        int seconds = int.Parse(secondsANDmilli[0]) % 60;
        string milli = "000";
        if (secondsANDmilli.Length > 1)
        {
            milli = secondsANDmilli[1];
            while (milli.Length != 3)
            {
                milli += '0';
            }
        }

        string result = "";
        if (minutes > 0)
        {
            result += minutes.ToString();
            result += ":";
            if (seconds.ToString().Length == 1)
            {
                result += "0";
            }
        }
        result += seconds.ToString();
        result += ".";
        result += milli;
        return result;
    }
}