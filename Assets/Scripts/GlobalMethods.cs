using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GlobalMethods
{
    // A collection of useful functions

    public static ObjectLists OL = null;

    public static Vector3 GetVectorInDirection(Transform t, float amount, Vector3 direction)
    {
        // Returns a position <amount> units <direction> of <transform>'s position
        Vector3 origPos = t.position;
        Quaternion origRot = t.rotation;
        t.Translate(direction * amount);
        Vector3 result = t.position;
        t.position = origPos;
        t.rotation = origRot;

        return result;
    }

    public static Vector3 GetVectorInFront(Transform t, float amount)
    {
        return GetVectorInDirection(t, amount, Vector3.forward);
    }

    public static GameObject TestHit(Transform t, float distance, float offset = 0.0f)
    {
        // Sees if object is within distance units of transform's looking, returns if so (otherwise null)

        RaycastHit rchit;
        bool hit = Physics.Raycast(GetVectorInFront(t, offset),
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

    public static void VelocityMove(GameObject go, Vector3 dest, Quaternion desiredRot, float force = 0.3f)
    {
        // Moves <go> to <dest> position (with <desiredRot> rotation) using physics
        // if want no new rotation, then run with an all zero quaternion

        //if (!go.name.Equals("Player")) { Debug.Log("VelocityMove gets called on " + go.name); }

        Vector3 start = go.transform.position;

        Vector3 calc = (dest - start) / Time.deltaTime;
        calc = calc * force;
        go.GetComponent<Rigidbody>().velocity = calc;

        if (!desiredRot.Equals(new Quaternion(0, 0, 0, 0)))
        {
            //Debug.Log("rotation given");
            go.transform.rotation = Quaternion.RotateTowards(go.transform.rotation,
                                                             desiredRot,
                                                             90.0f/*Time.deltaTime*/);
        }
    }

    private static void InitializeOL() {
        OL = GameObject.Find("GlobalLists").GetComponent<ObjectLists>();
    }

    public static GameObject[] GetReversables() {
        if (OL == null) { InitializeOL(); }
        return OL.Reversables;
    }

    public static GameObject[] GetPickupables()
    {
        if (OL == null) { InitializeOL(); }
        return OL.Pickupables;
    }

    public static bool ObjectInArray<T>(T a, T[] b) {
        // Like List.Contains but for arrays
        for (int i = 0; i < b.Length; i++)
        {
            if (b[i].Equals(a))
            {
                return true;
            }
        }
        return false;
    }
}
