using UnityEngine;
using UnityEngine.AI;

public class Guard : MonoBehaviour
{
    public float normalWanderRadius = 20f;
    public float hideModeWanderRadius = 10f; // Smaller radius for Hide mode to make the guard wander closer
    public float detectionRadius = 5f; // Detection radius for when the guard finds the player in Hide mode
    public float closeDistance = 5f; // Minimum distance to player in Seek/Neutral modes
    public float hideModeSpeedMultiplier = 1.5f; // Speed multiplier for Hide mode
    public float wanderInterval = 3f; // Time interval between random wander movements

    private NavMeshAgent agent;
    private GameObject player;
    private GameManager gameManager;
    private float normalSpeed;
    private float wanderTimer;

    public AudioManager audioManager;

    private void Start()
    {
        audioManager = AudioManager.Instance;
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player");
        gameManager = GameObject.FindObjectOfType<GameManager>();

        if (agent != null)
        {
            normalSpeed = agent.speed; // Save the original speed
        }
    }

    private void Update()
    {
        if (gameManager == null || player == null) return;

        if (gameManager.isHideMode)
        {
            agent.speed = normalSpeed * hideModeSpeedMultiplier; // Increase speed in Hide mode
            HideModeBehavior();
        }
        else
        {
            agent.speed = normalSpeed; // Reset speed in Seek/Neutral modes
            WanderAroundPlayer(normalWanderRadius);
        }
    }

    private void HideModeBehavior()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        if (distanceToPlayer < detectionRadius && !gameManager.isPlayerHiding)
        {
            // Player is "found" if within detection radius and not hiding
            agent.SetDestination(player.transform.position);
        }
        else
        {
            // Wander around player's position within hideModeWanderRadius
            if (wanderTimer <= 0f)
            {
                SetDestinationInRadius(player.transform.position, hideModeWanderRadius);
                wanderTimer = wanderInterval; // Reset the wander timer
            }
            else
            {
                wanderTimer -= Time.deltaTime; // Countdown timer
            }
        }
    }

    private void WanderAroundPlayer(float radius)
    {
        // Random wander around player's current position
        if (wanderTimer <= 0f)
        {
            SetDestinationInRadius(player.transform.position, radius);
            wanderTimer = wanderInterval; // Reset the wander timer
        }
        else
        {
            wanderTimer -= Time.deltaTime; // Countdown timer
        }
    }

    private void SetDestinationInRadius(Vector3 center, float radius)
    {
        // Find a random point within a radius around a given center point
        Vector3 randomDirection = Random.insideUnitSphere * radius;
        randomDirection += center;
        NavMeshHit navHit;

        if (NavMesh.SamplePosition(randomDirection, out navHit, radius, NavMesh.AllAreas))
        {
            agent.SetDestination(navHit.position);
        }
    }
}
