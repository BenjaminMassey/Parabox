using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSCharacterController : MonoBehaviour
{
    public Rigidbody rb;

    public float speed = 10.0f;
    public float jumpForce = 10.0f;

    private Vector3 movement;
    private float distanceToGround;

    private float x;
    private float z;

    private float prevX;
    private float prevZ;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        distanceToGround = GetComponent<Collider>().bounds.extents.y;
    }

    private void Update()
    {
        x = Input.GetAxis("Horizontal");
        z = Input.GetAxis("Vertical");

        // This stores and applies the last movement in air
        // Makes it so in air movement doesn't stop when letting go of a movement key
        if (!IsGrounded())
        {
            if (x != 0 && Mathf.Abs(x) > Mathf.Abs(prevX))
            {
                prevX = x;
            }
            else if (prevX != 0)
            {
                if (prevX < 0)
                {
                    prevX += 0.01f;
                }
                else
                {
                    prevX -= 0.01f;
                }
            }

            if (z != 0 && Mathf.Abs(z) > Mathf.Abs(prevZ))
            {
                prevZ = z;
            }
            else if (prevZ != 0)
            {
                if (prevZ < 0)
                {
                    prevZ += 0.01f;
                }
                else
                {
                    prevZ -= 0.01f;
                }
            }
        }
        else
        {
            prevX = x;
            prevZ = z;
        }

        movement = transform.right * prevX + transform.forward * prevZ;

        // NOTE: Might want to move this call into FixedUpdate
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (IsGrounded())
            {
                rb.AddForce(0f, jumpForce, 0f, ForceMode.Impulse);
            }
        }
    }

    private void FixedUpdate()
    {
        MovePlayer(movement);
    }

    private void MovePlayer(Vector3 movement)
    {
        // Tried using MovePosition but it has wonky collision effects (jitter near walls / corners)
        Vector3 vel = rb.velocity;
        vel.x = movement.x * speed;
        vel.z = movement.z * speed;
        rb.velocity = vel;
    }

    private bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, distanceToGround + 0.1f);
    }
}
