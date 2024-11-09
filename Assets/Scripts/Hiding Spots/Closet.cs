using System.Collections;
using UnityEngine;

public class Closet : MonoBehaviour
{
    public GameManager gameManager;

    private Transform playerPosition = FindAnyObjectByType<PlayerControls>().gameObject.GetComponentInParent<Transform>();
    public Transform closetPosition; // The position where the player should be moved to inside the closet
    public Transform cameraRotation; // The desired rotation of the camera when the player enters the closet
    public GameObject door1; // First door to slide open/close
    public GameObject door2; // Second door to slide open/close
    public float doorSlideSpeed = 2f; // Speed at which the doors slide open/close

    private bool playerInRange = false; // Whether the player is in range of the closet
    private bool isPlayerInCloset = false; // To track if the player is currently in the closet
    private bool doorsOpen = false; // To track if the doors are open or closed

    private Vector3 originalPlayerPosition; // To store the player's original position outside the closet
    private Quaternion originalCameraRotation; // To store the original camera rotation

    void Start()
    {
        gameManager = GameManager.Instance;
        // Save the player's original position and camera rotation
        originalPlayerPosition = Camera.main.transform.parent.position;
        originalCameraRotation = Camera.main.transform.rotation;

        // Ensure the doors start in a closed position
        door1.transform.position = door1.transform.position; // Or set to a start position if necessary
        door2.transform.position = door2.transform.position;
    }

    void Update()
    {
        // Check if the player is in range and presses the 'E' key
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            if (isPlayerInCloset) // If player is inside closet, exit
            {
                ExitCloset();
            }
            else // If player is outside the closet, enter
            {
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
       // SlideDoorsOpen();

        // Move player to closet position
       // .position = closetPosition.position;

        // Change the camera rotation
        
    }

    // Method to teleport the player out of the closet and reset camera
    private void ExitCloset()
    {
        isPlayerInCloset = false;
        doorsOpen = false; // Close doors when exiting the closet

        // Return player to their original position
        Transform playerTransform = Camera.main.transform.parent;
        playerTransform.position = originalPlayerPosition;

        // Reset camera rotation
        Camera.main.transform.rotation = originalCameraRotation;
    }

    // Coroutine to open the doors
    private IEnumerator SlideDoorsOpen()
    {
        Vector3 door1StartPosition = door1.transform.position;
        Vector3 door2StartPosition = door2.transform.position;

        Vector3 door1OpenPosition = door1StartPosition + new Vector3(-3f, 0f, 0f); // Change to desired sliding direction and distance
        Vector3 door2OpenPosition = door2StartPosition + new Vector3(3f, 0f, 0f);  // Adjust distance accordingly

        float elapsedTime = 0f;

        // Slide doors open over time
        while (elapsedTime < doorSlideSpeed)
        {
            door1.transform.position = Vector3.Lerp(door1StartPosition, door1OpenPosition, elapsedTime / doorSlideSpeed);
            door2.transform.position = Vector3.Lerp(door2StartPosition, door2OpenPosition, elapsedTime / doorSlideSpeed);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        door1.transform.position = door1OpenPosition;
        door2.transform.position = door2OpenPosition;
    }

    // Coroutine to close the doors
    private IEnumerator SlideDoorsClose()
    {
        Vector3 door1StartPosition = door1.transform.position;
        Vector3 door2StartPosition = door2.transform.position;

        Vector3 door1ClosePosition = door1StartPosition - new Vector3(-3f, 0f, 0f); // Return doors to initial closed position
        Vector3 door2ClosePosition = door2StartPosition - new Vector3(3f, 0f, 0f);

        float elapsedTime = 0f;

        // Slide doors close over time
        while (elapsedTime < doorSlideSpeed)
        {
            door1.transform.position = Vector3.Lerp(door1StartPosition, door1ClosePosition, elapsedTime / doorSlideSpeed);
            door2.transform.position = Vector3.Lerp(door2StartPosition, door2ClosePosition, elapsedTime / doorSlideSpeed);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        door1.transform.position = door1ClosePosition;
        door2.transform.position = door2ClosePosition;
    }
}
