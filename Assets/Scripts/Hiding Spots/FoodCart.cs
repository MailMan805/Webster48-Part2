using System.Collections;
using UnityEngine;

public class FoodCart : MonoBehaviour
{
    public GameManager gameManager;


    public Jester jester; // Reference to the Jester script
    private Vector3 doorStartPosition;
    public Transform playerPosition;
    public Transform cartPosition; // The position where the player should be moved to inside the cart
    public Transform cameraRotation; // The desired rotation of the camera when the player enters the cart
    public GameObject door; // Door to slide open/close
    public float doorSlideSpeed = 2f; // Speed at which the door slides open/close
    public float playerTransitionSpeed = 1f; // Speed at which the player moves in/out of the cart
    public float hideCooldownTime = 5f; // Cooldown time before the player can hide again after being forced out

    public float waitTime = 2f;  // Example wait time (2 seconds)
    private bool playerInRange = false; // Whether the player is in range of the cart
    private bool isPlayerInCart = false; // To track if the player is currently in the cart
    private bool isTransitioning = false; // To prevent player action during transition
    private bool isCooldownActive = false; // Whether the cooldown is active
    private float cooldownTimer = 0f; // Timer for the cooldown

    public Transform tempPlayerPosition;

    private PlayerControls playerControls; // Reference to the PlayerControls script

    void Start()
    {
        gameManager = GameManager.Instance;
        jester = FindObjectOfType<Jester>();
        doorStartPosition = door.transform.position;

        // Get the PlayerControls component from the player object
        playerControls = playerPosition.GetComponent<PlayerControls>();
    }

    void Update()
    {
        
        // If player presses 'E' while in range and there's no cooldown, allow them to enter/exit the cart
        if (playerInRange && !isTransitioning && !isCooldownActive && Input.GetKeyDown(KeyCode.E))
        {
            
            if (jester != null && jester.gameManager.isSeekMode)
            {
                CheckCart();
            }
            else if (isPlayerInCart) // If player is inside cart, exit
            {
                ExitCart(false);
            }
            else // If player is outside the cart, enter
            {
                tempPlayerPosition.position = playerPosition.position;
                EnterCart();
            }
        }

        if (isPlayerInCart && gameManager.isSeekMode)
        {
            ExitCart(true);
        }
    }

    // Trigger when the player enters the cart's collider
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }

        if (other.CompareTag("Snooper") && isPlayerInCart) // If Snooper enters while player is hiding
        {
            ForcePlayerOutOfHiding();
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
        StartCoroutine(SlideDoorsOpen(false));
        gameManager.isPlayerHiding = true;
    }

    // Method to teleport the player out of the cart and reset camera
    private void ExitCart(bool forced)
    {
        isPlayerInCart = false;
        isTransitioning = true; // Start transition
        playerControls.enabled = false; // Disable player movement
        StartCoroutine(SlideDoorsOpen(forced));
        gameManager.isPlayerHiding = false;
    }

    private void CheckCart()
    {
        StartCoroutine(SlideDoorsOpen(false));

        jester.CheckHidingSpot(playerPosition);
    }

    // Coroutine to open the doors
    private IEnumerator SlideDoorsOpen(bool forced)
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
        if(!gameManager.isSeekMode)
        {
            StartCoroutine(SmoothPlayerTransition());
        }
        else if (forced)
        {
            StartCoroutine(SmoothPlayerTransition());
        }
        else
        {
            StartWaiting();
        }

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

    // Force the player out of the cart when the Snooper enters
    private void ForcePlayerOutOfHiding()
    {
        if (isPlayerInCart && !isCooldownActive) // Only force out if player is in cart and no cooldown is active
        {
            ExitCart(true); // Force the player out of the cart
            StartCoroutine(ActivateCooldown()); // Start the cooldown to prevent immediate re-hiding
        }
    }

    // Start the cooldown before the player can hide again
    private IEnumerator ActivateCooldown()
    {
        isCooldownActive = true;
        cooldownTimer = hideCooldownTime; // Set cooldown time

        // Wait for the cooldown to finish
        while (cooldownTimer > 0f)
        {
            cooldownTimer -= Time.deltaTime;
            yield return null;
        }

        isCooldownActive = false; // Cooldown finished, player can hide again
    }
    private IEnumerator WaitForTime(float timeToWait)
    {
        // Wait for the specified time
        yield return new WaitForSeconds(timeToWait);
        StartCoroutine(SlideDoorsClose());
    }

    // Example method to call the wait
    public void StartWaiting()
    {
        // Start the coroutine with the wait time you want
        StartCoroutine(WaitForTime(waitTime));
    }
}
