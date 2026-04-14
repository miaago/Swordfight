using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEditor.Compilation;
using UnityEngine;

public class PlayerDash : MonoBehaviour
{
    [Header("References")]
    public Transform orientation;
    public Transform playerCam;
    private Rigidbody rb;
    private PlayerMovement pm;

    [Header("Dashing")]
    public float dashForce;
    public float dashUpwardForce;
    public float maxDashYSpeed;
    public float dashDuration;

    [Header("CameraEffects")]
    public CameraMovement cam;
    public float dashFov;

    [Header("Cooldown")]
    public float dashCooldown;
    private float dashCooldownTimer;

    [Header("Keybinds")]
    public KeyCode dashKey = KeyCode.E;

    [Header("Settings")]
    public bool useCameraForward = true;
    public bool allowAllDirections = true;
    public bool disableGravity = false;
    public bool resetVel = true;


    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        pm = GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(dashKey))
        {
            Dash();
        }
        
        if(dashCooldownTimer > 0)
        {
            dashCooldownTimer -= Time.deltaTime;
        }
    }

    private void Dash()
    {
        if (dashCooldownTimer > 0) return;
        else dashCooldownTimer = dashCooldown;

        pm.dashing = true;
        pm.maxYSpeed = maxDashYSpeed;

        cam.DoFov(dashFov);

        Transform forwardT;

        if (useCameraForward)
        {
            forwardT = playerCam;
        }
        else
        {
            forwardT = orientation;
        }

        Vector3 direction = GetDirection(forwardT);

        Vector3 forceToApply = direction * dashForce + orientation.up * dashUpwardForce;
        
        delayedForceToApply = forceToApply;

        if (disableGravity)
        {
            rb.useGravity = false;
        }

        Invoke(nameof(DelayedDashForce), 0.025f);
        
        Invoke(nameof(ResetDash), dashDuration);
    }

    private Vector3 delayedForceToApply;

    private void DelayedDashForce()
    {
        if (resetVel)
        {
            rb.linearVelocity = Vector3.zero;
        }

        rb.AddForce(delayedForceToApply, ForceMode.Impulse);
    }

    private void ResetDash()
    {
        pm.dashing = false;
        pm.maxYSpeed = 0;

        cam.DoFov(60f);

        if (disableGravity)
        {
            rb.useGravity = true;
        }
    }

    private Vector3 GetDirection(Transform forwardT)
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float VerticalInput = Input.GetAxisRaw("Vertical");

        Vector3 direction = new Vector3();

        if(allowAllDirections)
        {
            direction = forwardT.forward * VerticalInput + forwardT.right * horizontalInput;
        }
        else
        {
            direction = forwardT.forward;
        }

        if (VerticalInput == 0 && horizontalInput == 0)
        {
            direction = forwardT.forward;
        }

        return direction.normalized;
    }
}
