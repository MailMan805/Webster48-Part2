using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Jester : MonoBehaviour
{
    public float waitTime = 2f;  // Example wait time (2 seconds)
    public GameManager gameManager;
    private NavMeshAgent agent;
    public Transform farAwayLocation; // A point far from the player for Neutral and Hide phases
    public float seekRadius = 50f; // Radius to find hiding spots
    private Transform currentHidingSpot;
    public Transform player;
    private float phaseDuration;

    public GameObject TimesUp;
    public float timeToShowText = 3f;

    public Animator jesterAnim;

    public bool found = false;
    private bool isChasingPlayer = false; // If the Jester is chasing the player

    public BlinkingAnimation blinkingAnimation1;

    public AudioManager audioManager;
    void Start()
    {
        audioManager = AudioManager.Instance;
        TimesUp.SetActive(false);
        gameManager = GameManager.Instance;
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        blinkingAnimation1?.StopFlickering();
        phaseDuration = gameManager.phaseDuration;
        if (gameManager.isSeekMode)
        {
            StartCoroutine(ChaseTimer());
        }
        if (gameManager.isSeekMode && !isChasingPlayer)
        {
            SeekMode();
        }
    }

    // Enter Seek Mode - Find a hiding spot near the player
    void SeekMode()
    {
        if (currentHidingSpot == null) // If Jester hasn't hidden yet
        {
            found = false;
            FindHidingSpot();
        }
    }

    // Finds a random hiding spot within the seek radius
    void FindHidingSpot()
    {
        // Find all GameObjects with the tag "Jester Hiding Spot"
        GameObject[] hidingSpots = GameObject.FindGameObjectsWithTag("Jester Hiding Spot");

        // List to store hiding spots within range of the player
        List<Transform> validHidingSpots = new List<Transform>();

        // Iterate through all hiding spots and check if they are within the specified radius of the player
        foreach (GameObject hidingSpot in hidingSpots)
        {
            float distance = Vector3.Distance(player.position, hidingSpot.transform.position);

            if (distance <= seekRadius)  // Only add hiding spots within the radius
            {
                validHidingSpots.Add(hidingSpot.transform);
            }
        }

        if (validHidingSpots.Count > 0)
        {
            // Choose a random hiding spot from the valid spots within range
            Transform selectedHidingSpot = validHidingSpots[Random.Range(0, validHidingSpots.Count)];
            currentHidingSpot = selectedHidingSpot;

            // Disable the NavMeshAgent to allow direct teleportation
            agent.enabled = false;

            // Teleport the jester to the selected hiding spot
            transform.position = selectedHidingSpot.position;

            // Re-enable the NavMeshAgent (optional)
            agent.enabled = true;
        }
        else
        {
            print("No hiding spots found within range");
        }
    }

    // Return to a far location when in Neutral or Hide mode
    void GoToFarAwayLocation()
    {
        if (currentHidingSpot != null) currentHidingSpot = null;
        {
            // Disable the NavMeshAgent to allow direct teleportation
            agent.enabled = false;

            // Teleport the jester to the selected hiding spot
            transform.position = farAwayLocation.position;

            // Re-enable the NavMeshAgent (optional)
            agent.enabled = true;
        }
    }

    // Call when player opens a hiding spot
    public void CheckHidingSpot(Transform playerLocation)
    {
        if (currentHidingSpot != null && Vector3.Distance(playerLocation.position, currentHidingSpot.position) < 5f)
        {
            found = true;
            
            // Player found the Jester
            StartWaiting();
            
             // Move Jester to far location
        }
    }

    // Called if Seek mode timer runs out and Jester chases player
    public void StartChasingPlayer(Transform player)
    {
        isChasingPlayer = true;
        agent.SetDestination(player.position); // Set destination to player
    }

    private IEnumerator WaitForTime(float timeToWait)
    {
        // Wait for the specified time
        audioManager.PlaySFX("JesterFoundFinal_mixdown");
        audioManager.StopMusic();
        audioManager.PlayMusic("HideDanceGame01");
        yield return new WaitForSeconds(timeToWait);
        gameManager.isSeekMode = false;
        gameManager.isNeutralMode = true;
        if(found)
        {
            GoToFarAwayLocation();
        }


    }

    // Example method to call the wait
    public void StartWaiting()
    {
        // Start the coroutine with the wait time you want
        StartCoroutine(WaitForTime(waitTime));
    }

    private IEnumerator ChaseTimer()
    {
        float chaseTime = phaseDuration;
        yield return new WaitForSeconds(chaseTime);
        if (!found)
        {
            StartCoroutine(FlashText(TimesUp));
            StartChasingPlayer(player);
            audioManager.StopMusic();
            audioManager.musicVolume = 1.0f;
            audioManager.PlaySFX("SeekFailSting");
            audioManager.PlayMusic("CreepingAmbienceBG");
            jesterAnim.SetBool("IsChasing", true);
            gameManager.isJesterChasing = true;
        }
        
    }

    private IEnumerator FlashText(GameObject Text)
    {
        Text.SetActive(true);
        blinkingAnimation1?.StartFlickering();
        yield return new WaitForSeconds(timeToShowText);
        Text.SetActive(false);
    }
}
