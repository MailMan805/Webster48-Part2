using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Snooper : MonoBehaviour
{
    private NavMeshAgent agent;
    private List<Transform> hidingSpots = new List<Transform>();
    public float waitTimeAtHidingSpot = 2f; // Time to wait at each hiding spot
    public float seekRadius = 20f; // Radius to find nearby hiding spots

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        // Find all objects tagged "Hiding Spot" and store their transforms
        GameObject[] hidingSpotObjects = GameObject.FindGameObjectsWithTag("Hiding Spot");
        foreach (GameObject hidingSpot in hidingSpotObjects)
        {
            hidingSpots.Add(hidingSpot.transform);
        }

        if (hidingSpots.Count > 0)
        {
            MoveToNextHidingSpot(); // Start by moving to the first hiding spot
        }
        else
        {
            Debug.LogWarning("No hiding spots found in the scene!");
        }
    }

    void Update()
    {
        // Check if the agent has reached the current destination and then wait before moving to the next spot
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            // Only start waiting if the agent has reached the destination
            if (!agent.isStopped)
            {
                StartCoroutine(WaitAndMoveNext());
            }
        }
    }

    private IEnumerator WaitAndMoveNext()
    {
        // Stop the agent temporarily while waiting
        agent.isStopped = true;

        // Wait for a certain time at the current hiding spot
        yield return new WaitForSeconds(waitTimeAtHidingSpot);

        // After waiting, move to the next hiding spot
        agent.isStopped = false;

        // Now move to the next hiding spot
        MoveToNextHidingSpot();
    }

    private void MoveToNextHidingSpot()
    {
        // List to store nearby hiding spots within the seek radius
        List<Transform> nearbyHidingSpots = new List<Transform>();

        // Filter hiding spots based on distance to the Snooper's current position
        foreach (Transform hidingSpot in hidingSpots)
        {
            float distance = Vector3.Distance(transform.position, hidingSpot.position);
            if (distance <= seekRadius)
            {
                nearbyHidingSpots.Add(hidingSpot);
            }
        }

        // If there are nearby hiding spots, choose a random one
        if (nearbyHidingSpots.Count > 0)
        {
            Transform randomHidingSpot = nearbyHidingSpots[Random.Range(0, nearbyHidingSpots.Count)];
            agent.SetDestination(randomHidingSpot.position);
        }
        else
        {
            Debug.LogWarning("No hiding spots found within range!");
        }
    }
}
