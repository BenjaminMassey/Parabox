using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GlobalMethods
{
    public static Vector3 getVectorInFront(Transform t, float amount)
    {
        Vector3 origPos = t.position;
        Quaternion origRot = t.rotation;
        t.Translate(Vector3.forward);
        Vector3 result = t.position;
        t.position = origPos;
        t.rotation = origRot;

        return result;
    }

    public static GameObject TestHit(Transform t, float distance, float offset = 0.0f)
    {
        // Sees if object is within distance units of player's looking, returns if so (otherwise null)

        RaycastHit rchit;
        bool hit = Physics.Raycast(getVectorInFront(t, offset),
                                   t.TransformDirection(Vector3.forward),
                                   out rchit,
                                   distance);
        if (!hit)
        {
            return null;
        }
        else
        {
            return rchit.collider.gameObject;
        }
    }
}
