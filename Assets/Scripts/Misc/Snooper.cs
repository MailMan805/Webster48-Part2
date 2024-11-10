using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Snooper : MonoBehaviour
{
    private NavMeshAgent agent;
    private List<Transform> hidingSpots = new List<Transform>();
    private int currentSpotIndex = 0;
    public float waitTimeAtHidingSpot = 2f; // Time to wait at each hiding spot

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
        if (hidingSpots.Count > 0)
        {
            // Set the next hiding spot as the agent's destination
            agent.SetDestination(hidingSpots[currentSpotIndex].position);

            // Print the destination for debugging

            // Move to the next index in the list, looping back to the start if at the end
            currentSpotIndex = (currentSpotIndex + 1) % hidingSpots.Count;
        }
    }
}
