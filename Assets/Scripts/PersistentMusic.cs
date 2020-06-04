using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistentMusic : MonoBehaviour
{
    private void Awake()
    {
        GameObject[] musicTracks = GameObject.FindGameObjectsWithTag("Music");
        if (musicTracks.Length > 1) {
            Destroy(this.gameObject);
        }

        DontDestroyOnLoad(this.gameObject);
    }
}
