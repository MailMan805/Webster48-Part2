using System.Collections;
using UnityEngine;

public class FoodCart : MonoBehaviour
{
    public GameManager gameManager;

    private Vector3 doorStartPosition;
    public Transform playerPosition;
    public Transform cartPosition; // The position where the player should be moved to inside the cart
    public Transform cameraRotation; // The desired rotation of the camera when the player enters the cart
    public GameObject door; // Door to slide open/close
    public float doorSlideSpeed = 2f; // Speed at which the door slides open/close
    public float playerTransitionSpeed = 1f; // Speed at which the player moves in/out of the cart

    private bool playerInRange = false; // Whether the player is in range of the cart
    private bool isPlayerInCart = false; // To track if the player is currently in the cart
    private bool isTransitioning = false; // To prevent player action during transition

    public Transform tempPlayerPosition;

    private PlayerControls playerControls; // Reference to the PlayerControls script

    void Start()
    {
        gameManager = GameManager.Instance;
        doorStartPosition = door.transform.position;

        // Get the PlayerControls component from the player object
        playerControls = playerPosition.GetComponent<PlayerControls>();
    }

    void Update()
    {
        // Check if the player is in range, not transitioning, and presses the 'E' key
        if (playerInRange && !isTransitioning && Input.GetKeyDown(KeyCode.E))
        {
            if (isPlayerInCart) // If player is inside cart, exit
            {
                ExitCart();
            }
            else // If player is outside the cart, enter
            {
                tempPlayerPosition.position = playerPosition.position;
                EnterCart();
            }
        }
    }

    // Trigger when the player enters the cart's collider
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    // Trigger when the player exits the cart's collider
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }

    // Method to teleport the player into the cart and rotate the camera
    private void EnterCart()
    {
        isPlayerInCart = true;
        isTransitioning = true; // Start transition
        playerControls.enabled = false; // Disable player movement
        StartCoroutine(SlideDoorsOpen());
        gameManager.isPlayerHiding = true;
    }

    // Method to teleport the player out of the cart and reset camera
    private void ExitCart()
    {
        isPlayerInCart = false;
        isTransitioning = true; // Start transition
        playerControls.enabled = false; // Disable player movement
        StartCoroutine(SlideDoorsOpen());
        gameManager.isPlayerHiding = false;
    }

    // Coroutine to open the doors
    private IEnumerator SlideDoorsOpen()
    {
        Vector3 doorOpenPosition = doorStartPosition + new Vector3(3f, 0f, 0f); // Change to desired sliding direction and distance

        float elapsedTime = 0f;

        // Slide door open over time
        while (elapsedTime < doorSlideSpeed)
        {
            door.transform.position = Vector3.Lerp(doorStartPosition, doorOpenPosition, elapsedTime / doorSlideSpeed);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure door reaches open position
        door.transform.position = doorOpenPosition;

        // Smoothly transition the player into/out of the cart
        StartCoroutine(SmoothPlayerTransition());
    }

    // Coroutine to close the doors
    private IEnumerator SlideDoorsClose()
    {
        Vector3 doorOpenPosition = doorStartPosition + new Vector3(3f, 0f, 0f); // Change to desired sliding direction and distance

        float elapsedTime = 0f;

        // Slide door close over time
        while (elapsedTime < doorSlideSpeed)
        {
            door.transform.position = Vector3.Lerp(doorOpenPosition, doorStartPosition, elapsedTime / doorSlideSpeed);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure door reaches closed position
        door.transform.position = doorStartPosition;

        // End transition and re-enable player movement if they are not in the cart
        isTransitioning = false;
        if (!isPlayerInCart)
        {
            playerControls.enabled = true; // Re-enable player movement after exit
        }
    }

    // Coroutine to smoothly move and rotate the player
    private IEnumerator SmoothPlayerTransition()
    {
        Vector3 startPosition = playerPosition.position;
        Quaternion startRotation = playerPosition.rotation;
        Vector3 targetPosition = isPlayerInCart ? cartPosition.position : tempPlayerPosition.position;
        Quaternion targetRotation = isPlayerInCart ? cameraRotation.rotation : tempPlayerPosition.rotation;

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
