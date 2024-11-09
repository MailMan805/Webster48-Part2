using System.Collections;
using UnityEngine;

public class Closet : MonoBehaviour
{
    public GameManager gameManager;
    public Jester jester;
    private Vector3 LeftStartPosition;
    private Vector3 RightStartPosition;
    public Transform playerPosition;
    public Transform closetPosition;
    public Transform cameraRotation;
    public GameObject LeftDoor;
    public GameObject RightDoor;
    public float doorSlideSpeed = 2f;
    public float playerTransitionSpeed = 1f;
    public float hideCooldownTime = 5f;
    public float waitTime = 2f;  // Example wait time (2 seconds)

    private bool playerInRange = false;
    private bool isPlayerInCloset = false;
    private bool isTransitioning = false;
    private bool isCooldownActive = false;
    private float cooldownTimer = 0f;

    public Transform tempPlayerPosition;
    private PlayerControls playerControls;

    void Start()
    {
        gameManager = GameManager.Instance;
        LeftStartPosition = LeftDoor.transform.position;
        RightStartPosition = RightDoor.transform.position;
        playerControls = playerPosition.GetComponent<PlayerControls>();
    }

    void Update()
    {
        if (playerInRange && !isTransitioning && !isCooldownActive && Input.GetKeyDown(KeyCode.E))
        {
            if (jester != null && gameManager.isSeekMode)
            {
                CheckCart(); // Check only in Seek mode, no hiding
            }
            else if (isPlayerInCloset)
            {
                ExitCloset(false);
            }
            else
            {
                tempPlayerPosition.position = playerPosition.position;
                EnterCloset();
            }
        }

        if (isPlayerInCloset && gameManager.isSeekMode)
        {
            ExitCloset(true);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }

        if (other.CompareTag("Snooper") && isPlayerInCloset)
        {
            ForcePlayerOutOfHiding();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }

    private void EnterCloset()
    {
        if (gameManager.isSeekMode) return;

        isPlayerInCloset = true;
        isTransitioning = true;
        playerControls.enabled = false;
        StartCoroutine(SlideDoorsOpen(false));
        gameManager.isPlayerHiding = true;
    }

    private void ExitCloset(bool forced)
    {
        isPlayerInCloset = false;
        isTransitioning = true;
        playerControls.enabled = false;
        StartCoroutine(SlideDoorsOpen(forced));
        gameManager.isPlayerHiding = false;
    }

    private void CheckCart()
    {
        print("Test test");
        StartCoroutine(SlideDoorsOpen(false));
        jester.CheckHidingSpot(playerPosition);
    }

    private IEnumerator SlideDoorsOpen(bool forced)
    {
        Vector3 door1OpenPosition = LeftStartPosition + new Vector3(1f, 0f, 0f);
        Vector3 door2OpenPosition = RightStartPosition + new Vector3(-1f, 0f, 0f);

        float elapsedTime = 0f;

        while (elapsedTime < doorSlideSpeed)
        {
            LeftDoor.transform.position = Vector3.Lerp(LeftStartPosition, door1OpenPosition, elapsedTime / doorSlideSpeed);
            RightDoor.transform.position = Vector3.Lerp(RightStartPosition, door2OpenPosition, elapsedTime / doorSlideSpeed);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        LeftDoor.transform.position = door1OpenPosition;
        RightDoor.transform.position = door2OpenPosition;

        if (!gameManager.isSeekMode)
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

    private IEnumerator SlideDoorsClose()
    {
        Vector3 door1OpenPosition = LeftStartPosition + new Vector3(1f, 0f, 0f);
        Vector3 door2OpenPosition = RightStartPosition + new Vector3(-1f, 0f, 0f);

        float elapsedTime = 0f;

        while (elapsedTime < doorSlideSpeed)
        {
            LeftDoor.transform.position = Vector3.Lerp(door1OpenPosition, LeftStartPosition, elapsedTime / doorSlideSpeed);
            RightDoor.transform.position = Vector3.Lerp(door2OpenPosition, RightStartPosition, elapsedTime / doorSlideSpeed);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        LeftDoor.transform.position = LeftStartPosition;
        RightDoor.transform.position = RightStartPosition;

        isTransitioning = false;
        if (!isPlayerInCloset)
        {
            playerControls.enabled = true;
        }
    }

    private IEnumerator SmoothPlayerTransition()
    {
        Vector3 startPosition = playerPosition.position;
        Quaternion startRotation = playerPosition.rotation;
        Vector3 targetPosition = isPlayerInCloset ? closetPosition.position : tempPlayerPosition.position;
        Quaternion targetRotation = isPlayerInCloset ? cameraRotation.rotation : tempPlayerPosition.rotation;

        float elapsedTime = 0f;

        while (elapsedTime < playerTransitionSpeed)
        {
            playerPosition.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / playerTransitionSpeed);
            playerPosition.rotation = Quaternion.Lerp(startRotation, targetRotation, elapsedTime / playerTransitionSpeed);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        playerPosition.position = targetPosition;
        playerPosition.rotation = targetRotation;

        StartCoroutine(SlideDoorsClose());
    }

    private void ForcePlayerOutOfHiding()
    {
        if (isPlayerInCloset && !isCooldownActive)
        {
            ExitCloset(true);
            StartCoroutine(ActivateCooldown());
        }
    }

    private IEnumerator ActivateCooldown()
    {
        isCooldownActive = true;
        cooldownTimer = hideCooldownTime;

        while (cooldownTimer > 0f)
        {
            cooldownTimer -= Time.deltaTime;
            yield return null;
        }

        isCooldownActive = false;
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
