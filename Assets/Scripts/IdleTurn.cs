using UnityEngine;

public class ObjectRotationController : MonoBehaviour
{
    private Quaternion initialRotation; // To store the object's initial rotation
    public Transform player; // Reference to the player's transform
    public GameManager manager;


    private bool isLookingAtPlayer = false;
    private bool isLookingAwayFromPlayer = false;

    void Start()
    {
        manager = GameManager.Instance;
        player = FindAnyObjectByType<PlayerControls>().transform;
        // Save the initial rotation of the object when the game starts
        initialRotation = transform.rotation;
    }

    void Update()
    {
        if (manager.isHideMode)
        {
            FacePlayer();
        }
        else if (manager.isSeekMode)
        {
            LookAwayFromPlayer();
        }
        else
        {
            ReturnToOriginalRotation();
        }
    }

    // Method to make the object continuously face the player
    public void StartFacingPlayer()
    {
        isLookingAtPlayer = true;
        isLookingAwayFromPlayer = false;
    }

    // Method to make the object continuously look away from the player
    public void StartLookingAwayFromPlayer()
    {
        isLookingAtPlayer = false;
        isLookingAwayFromPlayer = true;
    }

    // Method to return the object to its original rotation
    public void ReturnToOriginalRotation()
    {
        isLookingAtPlayer = false;
        isLookingAwayFromPlayer = false;
        transform.rotation = initialRotation;
    }

    // Helper method to make the object face the player
    private void FacePlayer()
    {
        Vector3 directionToPlayer = player.position - transform.position;
        Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
    }

    // Helper method to make the object look away from the player
    private void LookAwayFromPlayer()
    {
        Vector3 directionAwayFromPlayer = transform.position - player.position;
        Quaternion targetRotation = Quaternion.LookRotation(directionAwayFromPlayer);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
    }
}
