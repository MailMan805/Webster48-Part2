using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControls : MonoBehaviour
{
    private GameManager gameManager;
    public float moveSpeed = 5f;  // Movement speed
    public float lookSpeedX = 2f; // Mouse look sensitivity on X axis
    public float lookSpeedY = 2f; // Mouse look sensitivity on Y axis

    private float rotationX = 0f; // To store current rotation on X axis for the camera
    private Transform playerBody; // Reference to player's body for rotation
    private Rigidbody rb; // Reference to player's Rigidbody for physics-based movement

    void Start()
    {
        gameManager = GameManager.Instance;

        // Lock the cursor and make it invisible
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Get the reference to the player's body (to rotate the body on the Y axis)
        playerBody = transform;

        // Get the Rigidbody component for physics-based movement
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("Player does not have a Rigidbody component.");
            return;
        }

        // Ensure the Rigidbody is not kinematic, allowing physics-based interactions
        rb.isKinematic = false;

        // Set Rigidbody settings to help smooth movement
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

        // Disable gravity to allow movement in all directions
        rb.useGravity = false;
    }

    void Update()
    {
        if (!gameManager.isPlayerHiding && !gameManager.isPlayerDistracted)
        {
            // Mouse Look - rotating the camera
            float mouseX = Input.GetAxis("Mouse X") * lookSpeedX;
            float mouseY = Input.GetAxis("Mouse Y") * lookSpeedY;

            rotationX -= mouseY;
            rotationX = Mathf.Clamp(rotationX, -90f, 90f); // Clamp the X rotation so the camera doesn't flip upside down

            Camera.main.transform.localRotation = Quaternion.Euler(rotationX, 0f, 0f); // Apply X rotation to camera
            playerBody.Rotate(Vector3.up * mouseX); // Apply Y rotation to the player body
        }
    }

    void FixedUpdate()
    {
        if (!gameManager.isPlayerHiding && !gameManager.isPlayerDistracted)
        {
            // Player movement (WASD or arrow keys)
            float moveX = Input.GetAxis("Horizontal"); // A/D or Left/Right arrows
            float moveZ = Input.GetAxis("Vertical");   // W/S or Up/Down arrows
            float moveY = 0f;

            // Check for Up/Down movement keys (e.g., Space for up, Left Shift for down)
            if (Input.GetKey(KeyCode.Space)) moveY = 1f;
            if (Input.GetKey(KeyCode.LeftShift)) moveY = -1f;

            // Movement vector with all three axes
            Vector3 move = (transform.right * moveX + transform.forward * moveZ).normalized;

            // Apply the movement using Rigidbody's velocity to enable collision detection
            rb.velocity = move * moveSpeed;
        }
    }
}
