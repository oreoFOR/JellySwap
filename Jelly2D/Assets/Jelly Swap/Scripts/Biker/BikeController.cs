using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BikeController : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] float spinspeed;
    [SerializeField] float errorHeight;
    float actualSpeed;
    [SerializeField] WheelJoint2D wheelBack;
    bool cycling;
    bool spinning;
    [SerializeField]CapsuleCollider2D frameCol;
    [SerializeField]Rigidbody2D bikeFrame;
    public bool grounded;
    [SerializeField] LayerMask groundMask;
    private void FixedUpdate()
    {
        if (cycling)
        {
            JointMotor2D motor = new JointMotor2D
            {
                motorSpeed = actualSpeed,
                maxMotorTorque = 15000
            };
            wheelBack.motor = motor;
        }
        else if (spinning)
        {
            bikeFrame.angularVelocity = -spinspeed;
        }
        grounded = IsGrounded();
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (IsGrounded())
            {
                cycling = true;
                wheelBack.useMotor = true;
                actualSpeed = speed;
            }
            else
            {
                spinning = true;
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            spinning = false;
            cycling = false;
            wheelBack.useMotor = false;
            actualSpeed = 0;
            if (!IsGrounded())
            {
                bikeFrame.angularVelocity = 0;
            }
        }
    }
    bool IsGrounded()
    {
        float extraHeightTest = errorHeight;
        RaycastHit2D hit = Physics2D.Raycast(frameCol.bounds.center, -bikeFrame.transform.up, frameCol.bounds.extents.y + extraHeightTest,groundMask);
        Debug.DrawRay(frameCol.bounds.center, -bikeFrame.transform.up * (frameCol.bounds.extents.y + extraHeightTest), Color.red, 5);
        if (hit.collider != null)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
