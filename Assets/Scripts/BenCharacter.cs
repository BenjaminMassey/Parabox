using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BenCharacter : MonoBehaviour
{
    // Currently our main script for movement
    // Applied on Player.prefab

    public float moveSpeed = 1.0f;
    public float jumpForce = 40.0f;

    private Rigidbody rb;
    private bool grounded;
    private float distanceToGround;

    private float horInp;
    private float verInp;
    private Vector3 horizontal;
    private Vector3 vertical;
    private Vector3 desiredSpot;
    float ry;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        grounded = true;
        InvokeRepeating("UpdateGrounded", 0.0f, 1.0f / 10.0f);
        distanceToGround = GetComponent<Collider>().bounds.extents.y + 0.01f;
    }

    // Update is called once per frame
    void Update()
    {
        float speed = moveSpeed * Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.Space) && grounded)
        {
            rb.AddForce(0f, jumpForce, 0f, ForceMode.Impulse);
        }

        horInp = 0.0f;
        if (Input.GetKey(KeyCode.A)) { horInp += -1.0f; }
        if (Input.GetKey(KeyCode.D)) { horInp += 1.0f; }

        verInp = 0.0f;
        if (Input.GetKey(KeyCode.S)) { verInp += -1.0f; }
        if (Input.GetKey(KeyCode.W)) { verInp += 1.0f; }

        if (Mathf.Abs(horInp) + Mathf.Abs(verInp) == 2.0f)
        {
            horInp *= 0.66f;
            verInp *= 0.66f;
        }

        horizontal = GlobalMethods.GetVectorInDirection(transform, horInp * speed, Vector3.right);
        vertical = GlobalMethods.GetVectorInFront(transform, verInp * speed);

        desiredSpot = (horizontal + vertical) / 2.0f;

        ry = rb.velocity.y;

        GlobalMethods.VelocityMove(gameObject, desiredSpot, new Quaternion(0, 0, 0, 0));

        rb.velocity += new Vector3(0.0f, ry, 0.0f);
    }

    private void UpdateGrounded()
    {
        RaycastHit rch;
        grounded = Physics.Raycast(transform.position, Vector3.down, out rch, distanceToGround);
        if (rch.collider != null && rch.collider.gameObject.layer == 8)
        {
            grounded = false;
        }
    }
}
