using System;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;

public class CameraMovement : MonoBehaviour
{
    public float sensX;
    public float sensY;

    public Transform orientation;
    public Transform holdPoint;

    float xRotation;
    float yRotation;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        // get mouse inputs in live play
        float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * sensX;
        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * sensY;

        // alter current rotations
        yRotation += mouseX;
        xRotation -= mouseY;

        // clamp xRotation so player can't look too far up or down
        xRotation = Math.Clamp(xRotation, -90f, 90f);

        // rotates camera
        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        // rotates player model
        orientation.rotation = Quaternion.Euler(0, yRotation, 0);
    }

    public void DoFov(float endValue) // alter FOV
    {
        //smoothly transition fov using DOTween Library
        GetComponent<Camera>().DOFieldOfView(endValue, 0.25f);
    }
}

