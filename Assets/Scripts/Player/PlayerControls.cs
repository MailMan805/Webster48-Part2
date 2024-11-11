using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControls : MonoBehaviour
{
    private GameManager gameManager;
    public float moveSpeed = 5f;  // Movement speed
    public float lookSpeedX = 2f; // Mouse look sensitivity on X axis
    public float lookSpeedY = 2f; // Mouse look sensitivity on Y axis
    public float timeToKill = 0;

    public GameObject Knife;
    public float KnifeSpeed;
    private Vector3 KnifeOffset; // Offset from player position to keep knife in front
    private Vector3 KnifeStartPosition; // Store initial position of Knife
    private Quaternion KnifeStartRotation; // Store initial rotation of Knife

    private float rotationX = 0f; // To store current rotation on X axis for the camera
    private Transform playerBody; // Reference to player's body for rotation
    private Rigidbody rb; // Reference to player's Rigidbody for physics-based movement

    private bool kingSlayer = false;
    private bool canMove = true; // Flag to control player movement

    public AudioManager audioManager;

    void Start()
    {
        KnifeStartPosition = Knife.transform.position; // Store initial position
        KnifeOffset = Knife.transform.position - transform.position; // Calculate initial offset from player
        KnifeStartRotation = Knife.transform.rotation; // Store initial rotation of Knife
        audioManager = AudioManager.Instance;
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
            return;
        }

        // Ensure the Rigidbody is not kinematic, allowing physics-based interactions
        rb.isKinematic = false;

        // Disable gravity to allow movement in all directions
        rb.useGravity = false;

        // Initially hide the knife until KingSlayer mode is activated
        Knife.SetActive(false);
    }

    void Update()
    {
        if (!gameManager.isPlayerHiding && !gameManager.isPlayerDistracted && canMove)
        {
            // Mouse Look - rotating the camera
            float mouseX = Input.GetAxis("Mouse X") * lookSpeedX;
            float mouseY = Input.GetAxis("Mouse Y") * lookSpeedY;

            rotationX -= mouseY;
            rotationX = Mathf.Clamp(rotationX, -90f, 90f); // Clamp the X rotation so the camera doesn't flip upside down

            Camera.main.transform.localRotation = Quaternion.Euler(rotationX, 0f, 0f); // Apply X rotation to camera
            playerBody.Rotate(Vector3.up * mouseX); // Apply Y rotation to the player body
        }

        // Update the Knife position and rotation relative to the player when KingSlayer mode is active
        if (!kingSlayer)
        {
            // Keep the knife at its initial offset from the player
            Knife.transform.position = transform.position + transform.rotation * KnifeOffset;

            // Rotate the knife along with the player (relative to the player's body rotation)
            Knife.transform.rotation = playerBody.rotation * KnifeStartRotation;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Jester") && gameManager.isJesterChasing)
        {
            gameManager.GameOverJester = true;
        }
        else if (other.CompareTag("Gaurd") && gameManager.isHideMode && !gameManager.isPlayerHiding)
        {
            gameManager.GameOverGuard = true;
        }
        else if (other.CompareTag("King"))
        {
            KingSlayer();
        }
    }

    void FixedUpdate()
    {
        if (canMove && !gameManager.isPlayerHiding && !gameManager.isPlayerDistracted && !kingSlayer)
        {
            // Player movement (WASD or arrow keys)
            float moveX = Input.GetAxis("Horizontal"); // A/D or Left/Right arrows
            float moveZ = Input.GetAxis("Vertical");   // W/S or Up/Down arrows

            // Movement vector with all three axes
            Vector3 move = (transform.right * moveX + transform.forward * moveZ).normalized;

            // Apply the movement using Rigidbody's velocity to enable collision detection
            rb.velocity = move * moveSpeed;
        }
        else
        {
            // Stop player movement if KingSlayer is active or movement is disabled
            rb.velocity = Vector3.zero;
        }
    }

    void KingSlayer()
    {
        kingSlayer = true;
        canMove = false; // Disable player movement
        Knife.SetActive(true); // Make the knife visible
        StartCoroutine(KingSlayerAnim());
    }

    private IEnumerator KingSlayerAnim()
    {
        float elapsedTime = 0f;

        // Stabbing animation: Knife moves up and down in a loop
        Vector3 stabPosition1 = Knife.transform.position + Vector3.up * 1.2f; // Move up a bit to simulate a stab
        Vector3 stabPosition2 = Knife.transform.position + Vector3.down * 1f; // Move down a bit to simulate a stab

        // First stabbing phase (up-down motion)
        while (elapsedTime < KnifeSpeed)
        {
            Knife.transform.position = Vector3.Lerp(Knife.transform.position, stabPosition1, Mathf.PingPong(elapsedTime * KnifeSpeed, 1f));

            elapsedTime += Time.deltaTime;
            yield return null;
        }

 

        // Wait for the specified time before continuing (timeToKill)
        yield return new WaitForSeconds(timeToKill);
        elapsedTime = 0f;

        // Second stabbing phase (down-up motion)
        while (elapsedTime < KnifeSpeed)
        {
            Knife.transform.position = Vector3.Lerp(stabPosition1, stabPosition2, Mathf.PingPong(elapsedTime * (KnifeSpeed * 2) , 1f));

            elapsedTime += Time.deltaTime;
            yield return null;
        }
        Knife.SetActive(false);
        // End the KingSlayer animation
        gameManager.isWon = true;
        canMove = true; // Re-enable player movement
        kingSlayer = false; // Reset KingSlayer status
    }
}
