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

    void Start()
    {
        gameManager = GameManager.Instance;
        // Lock the cursor and make it invisible
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Get the reference to the player's body (to rotate the body on the Y axis)
        playerBody = transform;
    }

    void Update()
    {
        // Mouse Look - rotating the camera
        if (!gameManager.isPlayerHiding && !gameManager.isPlayerDistracted)
        {
            float mouseX = Input.GetAxis("Mouse X") * lookSpeedX;
            float mouseY = Input.GetAxis("Mouse Y") * lookSpeedY;

            rotationX -= mouseY;
            rotationX = Mathf.Clamp(rotationX, -90f, 90f); // Clamp the X rotation so the camera doesn't flip upside down

            Camera.main.transform.localRotation = Quaternion.Euler(rotationX, 0f, 0f); // Apply X rotation to camera
            playerBody.Rotate(Vector3.up * mouseX); // Apply Y rotation to the player body

            // Player movement (WASD or arrow keys)
            float moveX = Input.GetAxis("Horizontal"); // A/D or Left/Right arrows
            float moveZ = Input.GetAxis("Vertical");   // W/S or Up/Down arrows

            // Movement vector
            Vector3 move = transform.right * moveX + transform.forward * moveZ;
            transform.Translate(move * moveSpeed * Time.deltaTime, Space.World); // Move the player
        }
       
    }
}
