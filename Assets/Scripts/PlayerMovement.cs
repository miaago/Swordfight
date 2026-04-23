using System.Collections;
using System.Runtime.InteropServices.WindowsRuntime;
using NUnit.Framework;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float walkSpeed;
    public float sprintSpeed;
    private float moveSpeed;
    public float groundDrag;
    public float dashSpeed;
    public float dashSpeedChangeFactor;
    public float maxYSpeed;

    [Header("Jumping")]
    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    bool canJump;

    [Header("Crouching")]
    public float crouchSpeed;
    public float crouchYScale;
    private float startYScale;


    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;
    public KeyCode crouchKey = KeyCode.LeftControl;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundDistance;
    public LayerMask Ground;
    bool isGrounded;


    [Header("Slope Handling")]
    public float maxSlopeAngle;
    public float playerHeight;
    private RaycastHit slopeHit;
    private bool isExitingSlope;

    [Header("Misc")]
    public Transform orientation;
    float horizontalInput;
    float verticalInput;

    [Header("CameraEffects")]
    public CameraMovement cam;
    public float sprintFov;

    Vector3 moveDirection;

    Rigidbody rb;

    public MovementState state;
    public enum MovementState
    {
        walking,
        sprinting,
        crouching,
        dashing,
        air
    }

    public bool dashing;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        canJump = true;

        startYScale = transform.localScale.y;
    }

    private void Update()
    {
        // checks if groundCheck collider overlaps with "Ground" tagged layers colliders
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, Ground);
        
        MyInput();
        SpeedControl();
        StateHandler();

        if (state == MovementState.walking || state == MovementState.sprinting || state == MovementState.crouching)
        {
            rb.linearDamping = groundDrag;
        }
        else
        {
            rb.linearDamping = 0;
        }
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void MyInput() // get inputs
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        // jump detect
        if (Input.GetKey(jumpKey) && canJump && isGrounded)
        {
            canJump = false;

            Jump();

            // prevent continuous jumps
            Invoke(nameof(ResetJump), jumpCooldown);
        }

        // crouch detect
        if (Input.GetKeyDown(crouchKey)) // crouching
        {
            transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
            rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
        }
        if (Input.GetKeyUp(crouchKey)) // un-crouching
        {
            transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);
        }
    }
    private float desiredMoveSpeed; // variables for momentum
    private float lastDesiredMoveSpeed;
    private MovementState lastState;
    private bool keepMomentum;

    private void StateHandler()
    {
        if (Input.GetKey(crouchKey)) // Crouching
        {
            state = MovementState.crouching;
            desiredMoveSpeed = crouchSpeed;
        }
        else if (dashing) // Dashing
        {
            state = MovementState.dashing;
            desiredMoveSpeed = dashSpeed;
            speedChangeFactor = dashSpeedChangeFactor;
        }
        else if(isGrounded && Input.GetKey(sprintKey)) // Sprinting
        {
            state = MovementState.sprinting;
            desiredMoveSpeed = sprintSpeed;
            cam.DoFov(sprintFov);
        }
        else if (isGrounded) // Walking
        {
            state = MovementState.walking;
            desiredMoveSpeed = walkSpeed;
            cam.DoFov(60f);
        }
        else // Air
        {
            state = MovementState.air;

            if (desiredMoveSpeed < sprintSpeed)
            {
                desiredMoveSpeed = walkSpeed;
            }
            else
            {
                desiredMoveSpeed = sprintSpeed;
            }

            bool desiredMoveSpeedChanged = desiredMoveSpeed != lastDesiredMoveSpeed;
            if (lastState == MovementState.dashing)
            {
                keepMomentum = true;
            }

            if (desiredMoveSpeedChanged)
            {
                if (keepMomentum)
                {
                    StopAllCoroutines();
                    StartCoroutine(SmoothlyLerpMoveSpeed());
                }
                else
                {
                    StopAllCoroutines();
                    moveSpeed = desiredMoveSpeed;
                }
            }

            lastDesiredMoveSpeed = desiredMoveSpeed;
            lastState = state;
        }
    }

    // start momentum decrese code
    private float speedChangeFactor;

    private IEnumerator SmoothlyLerpMoveSpeed()
    {
        float time = 0;
        float difference = Mathf.Abs(desiredMoveSpeed - moveSpeed);
        float startValue = moveSpeed;

        float boostFactor = speedChangeFactor;

        while (time < difference)
        {
            moveSpeed = Mathf.Lerp(startValue, desiredMoveSpeed, time / difference);

            time += Time.deltaTime * boostFactor;

            yield return null;
        }

        moveSpeed = desiredMoveSpeed;
        speedChangeFactor = 1f;
        keepMomentum = false;
    }

    // end momentum decrease code
    private void MovePlayer() // move player
    {
        if (state == MovementState.dashing) return;

        // calculate movement direction by performing operations on our current orientation with our inputs
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        // move rigidbody via forces to simulate acceleration better
        if (OnSlope() && !isExitingSlope) // on slope
        {
            rb.AddForce(GetSlopeMoveDirection() * moveSpeed * 20f, ForceMode.Force);

            if (rb.linearVelocity.y > 0) // add downward force when moving up a slope (prevents bobbing)
            {
                rb.AddForce(Vector3.down * 80f, ForceMode.Force);
            }
        }

        if (isGrounded) // on ground
        {
            rb.AddForce(moveDirection * moveSpeed * 10f, ForceMode.Force);    
        }
        else if (!isGrounded) // in air
        {
            rb.AddForce(moveDirection * moveSpeed * airMultiplier * 10f, ForceMode.Force);
        }

        // turn gravity off while on slope
        rb.useGravity = !OnSlope();
  
    }

    private void SpeedControl()
    {
       
        if (OnSlope() && !isExitingSlope) // limit max speed on slope
        {
            if (rb.linearVelocity.magnitude > moveSpeed)
            {
                rb.linearVelocity = rb.linearVelocity.normalized * moveSpeed;
            }
        }
        else // limit max speed on ground or in air
        {
            Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);


            if (flatVel.magnitude > moveSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * moveSpeed;
                rb.linearVelocity = new Vector3(limitedVel.x, rb.linearVelocity.y, limitedVel.z);
            }
        }

        if (maxYSpeed != 0 && rb.linearVelocity.y > maxYSpeed) // limit y speed
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, maxYSpeed, rb.linearVelocity.z);
        }
    }

    private void Jump()
    {
        isExitingSlope = true;
        // reset y velocity
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);

    }

    private void ResetJump()
    {
        canJump = true;
        isExitingSlope = false;
    }

    private bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }
        
        return false;
    }

    private Vector3 GetSlopeMoveDirection()
    {
        return Vector3.ProjectOnPlane(moveDirection, slopeHit.normal.normalized);
    }
}
