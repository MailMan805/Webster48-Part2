using System.Collections;
using UnityEngine;

public class Closet : MonoBehaviour
{
    public GameManager gameManager;

    private Vector3 LeftStartPosition;
    private Vector3 RightStartPosition;
    public Transform playerPosition;
    public Transform closetPosition; // The position where the player should be moved to inside the closet
    public Transform cameraRotation; // The desired rotation of the camera when the player enters the closet
    public GameObject LeftDoor; // First door to slide open/close
    public GameObject RightDoor; // Second door to slide open/close
    public float doorSlideSpeed = 2f; // Speed at which the doors slide open/close
    public float playerTransitionSpeed = 1f; // Speed at which the player moves in/out of the closet

    private bool playerInRange = false; // Whether the player is in range of the closet
    private bool isPlayerInCloset = false; // To track if the player is currently in the closet
    private bool isTransitioning = false; // To prevent player action during transition

    public Transform tempPlayerPosition;

    private PlayerControls playerControls; // Reference to the PlayerControls script

    void Start()
    {
        gameManager = GameManager.Instance;
        LeftStartPosition = LeftDoor.transform.position;
        RightStartPosition = RightDoor.transform.position;

        // Get the PlayerControls component from the player object
        playerControls = playerPosition.GetComponent<PlayerControls>();
    }

    void Update()
    {
        // Check if the player is in range, not transitioning, and presses the 'E' key
        if (playerInRange && !isTransitioning && Input.GetKeyDown(KeyCode.E))
        {
            if (isPlayerInCloset) // If player is inside closet, exit
            {
                ExitCloset();
            }
            else // If player is outside the closet, enter
            {
                tempPlayerPosition.position = playerPosition.position;
                EnterCloset();
            }
        }
    }

    // Trigger when the player enters the closet's collider
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    // Trigger when the player exits the closet's collider
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }

    // Method to teleport the player into the closet and rotate the camera
    private void EnterCloset()
    {
        isPlayerInCloset = true;
        isTransitioning = true; // Start transition
        playerControls.enabled = false; // Disable player movement
        StartCoroutine(SlideDoorsOpen());
        gameManager.isPlayerHiding = true;
    }

    // Method to teleport the player out of the closet and reset camera
    private void ExitCloset()
    {
        isPlayerInCloset = false;
        isTransitioning = true; // Start transition
        playerControls.enabled = false; // Disable player movement
        StartCoroutine(SlideDoorsOpen());
        gameManager.isPlayerHiding = false;
    }

    // Coroutine to open the doors
    private IEnumerator SlideDoorsOpen()
    {
        Vector3 door1OpenPosition = LeftStartPosition + new Vector3(1f, 0f, 0f); // Change to desired sliding direction and distance
        Vector3 door2OpenPosition = RightStartPosition + new Vector3(-1f, 0f, 0f);  // Adjust distance accordingly

        float elapsedTime = 0f;

        // Slide doors open over time
        while (elapsedTime < doorSlideSpeed)
        {
            LeftDoor.transform.position = Vector3.Lerp(LeftStartPosition, door1OpenPosition, elapsedTime / doorSlideSpeed);
            RightDoor.transform.position = Vector3.Lerp(RightStartPosition, door2OpenPosition, elapsedTime / doorSlideSpeed);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure doors reach open position
        LeftDoor.transform.position = door1OpenPosition;
        RightDoor.transform.position = door2OpenPosition;

        // Smoothly transition the player into/out of the closet
        StartCoroutine(SmoothPlayerTransition());
    }

    // Coroutine to close the doors
    private IEnumerator SlideDoorsClose()
    {
        Vector3 door1OpenPosition = LeftStartPosition + new Vector3(1f, 0f, 0f); // Change to desired sliding direction and distance
        Vector3 door2OpenPosition = RightStartPosition + new Vector3(-1f, 0f, 0f);  // Adjust distance accordingly

        float elapsedTime = 0f;

        // Slide doors close over time
        while (elapsedTime < doorSlideSpeed)
        {
            LeftDoor.transform.position = Vector3.Lerp(door1OpenPosition, LeftStartPosition, elapsedTime / doorSlideSpeed);
            RightDoor.transform.position = Vector3.Lerp(door2OpenPosition, RightStartPosition, elapsedTime / doorSlideSpeed);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure doors reach closed position
        LeftDoor.transform.position = LeftStartPosition;
        RightDoor.transform.position = RightStartPosition;

        // End transition and re-enable player movement if they are not in the closet
        isTransitioning = false;
        if (!isPlayerInCloset)
        {
            playerControls.enabled = true; // Re-enable player movement after exit
        }
    }

    // Coroutine to smoothly move and rotate the player
    private IEnumerator SmoothPlayerTransition()
    {
        Vector3 startPosition = playerPosition.position;
        Quaternion startRotation = playerPosition.rotation;
        Vector3 targetPosition = isPlayerInCloset ? closetPosition.position : tempPlayerPosition.position;
        Quaternion targetRotation = isPlayerInCloset ? cameraRotation.rotation : tempPlayerPosition.rotation;

        float elapsedTime = 0f;

        // Smoothly move and rotate the player to the target position and rotation
        while (elapsedTime < playerTransitionSpeed)
        {
            playerPosition.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / playerTransitionSpeed);
            playerPosition.rotation = Quaternion.Lerp(startRotation, targetRotation, elapsedTime / playerTransitionSpeed);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure final position and rotation are exactly as the target
        playerPosition.position = targetPosition;
        playerPosition.rotation = targetRotation;

        StartCoroutine(SlideDoorsClose());
    }
}
