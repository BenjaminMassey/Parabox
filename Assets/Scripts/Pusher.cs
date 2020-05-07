using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pusher : MonoBehaviour
{
    public float pushPower;

    void OnCollisionStay(Collision collision)
    {
        Rigidbody rb = collision.rigidbody;

        if (!rb.gameObject.name.Contains("Push")) { return; }

        // no rigidbody
        if (rb == null || rb.isKinematic)
        {
            return;
        }

        // We dont want to push objects below us
        if (collision.impulse.y < -0.3)
        {
            return;
        }

        // Calculate push direction from move direction,
        // we only push objects to the sides never up and down
        // push the object whichever direction we are heading in more
        float movex = collision.impulse.x;
        float movez = collision.impulse.z;

        if (System.Math.Abs(movex) >= System.Math.Abs(movez))
        {
            movez = 0;
        }
        else
        {
            movex = 0;
        }
        Vector3 pushDir = new Vector3(movex, 0, movez);

        // If you know how fast your character is trying to move,
        // then you can also multiply the push velocity by that.

        // Apply the push
        rb.velocity = pushDir * pushPower;
    }
}
