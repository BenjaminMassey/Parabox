using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEnder : MonoBehaviour
{
    // Script to handle hitting the end of level
    // Currently barebones: just get rid of end marker
    
    // Attached to end marker

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Player")
        {
            Destroy(gameObject);
        }
    }
}
