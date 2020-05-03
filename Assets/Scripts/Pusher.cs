using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pusher : MonoBehaviour
{
    // From here: https://docs.unity3d.com/ScriptReference/CharacterController.OnControllerColliderHit.html

    public float pushPower;

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Rigidbody body = hit.collider.attachedRigidbody;

        // no rigidbody
        if (body == null || body.isKinematic)
        {
            return;
        }

        // We dont want to push objects below us
        if (hit.moveDirection.y < -0.3)
        {
            return;
        }

        // Calculate push direction from move direction,
        // we only push objects to the sides never up and down
        // push the object whichever direction we are heading in more
        float movex = hit.moveDirection.x;
        float movez = hit.moveDirection.z;
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
        body.velocity = pushDir * pushPower;
    }
}
