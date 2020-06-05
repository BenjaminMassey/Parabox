using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorHandler : MonoBehaviour
{
    public int numPressed;
    // Start is called before the first frame update
    void Awake()
    {
        numPressed = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddButton()
    {
        numPressed += 1;
    }

    public void SubtractButton()
    {
        numPressed -= 1;
        if (numPressed < 0)
        {
            Debug.Log("ERROR IN \"DoorHandler.SubtractButton()\"");
            numPressed = 0;
        }
    }

    public int GetNumPressed()
    {
        return numPressed;
    }

}
