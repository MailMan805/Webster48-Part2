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

    private bool playerInRange = false; // Whether the player is in range of the closet
    private bool isPlayerInCloset = false; // To track if the player is currently in the closet

    public Transform tempPlayerPosition;

    void Start()
    {
        gameManager = GameManager.Instance;
        LeftStartPosition = LeftDoor.transform.position;
        RightStartPosition = RightDoor.transform.position;
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
                tempPlayerPosition.position = playerPosition.position;
                
                EnterCloset();
            }
        }
        print(tempPlayerPosition.position);
    }

    // Trigger when the player enters the closet's collider
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            print("Enter");
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
        StartCoroutine(SlideDoorsOpen());

        
        gameManager.isPlayerHiding = true;

    }

    // Method to teleport the player out of the closet and reset camera
    private void ExitCloset()
    {
        isPlayerInCloset = false;
        gameManager.isPlayerHiding = false;

        StartCoroutine(SlideDoorsOpen());

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
        if(isPlayerInCloset)
        {
            playerPosition.position = closetPosition.position;
            playerPosition.rotation = cameraRotation.rotation;
        }
        else
        {
            playerPosition.position = tempPlayerPosition.position;
        }
        StartCoroutine(SlideDoorsClose());
    }

    // Coroutine to close the doors
    private IEnumerator SlideDoorsClose()
    {

        Vector3 door1ClosePosition = LeftStartPosition;
        Vector3 door2ClosePosition = RightStartPosition;

        float elapsedTime = 0f;

        // Slide doors close over time
        while (elapsedTime < doorSlideSpeed)
        {
            LeftDoor.transform.position = Vector3.Lerp(LeftStartPosition, door1ClosePosition, elapsedTime / doorSlideSpeed);
            RightDoor.transform.position = Vector3.Lerp(RightStartPosition, door2ClosePosition, elapsedTime / doorSlideSpeed);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

    }
}
