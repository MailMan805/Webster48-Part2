using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Loner : MonoBehaviour
{
    private GameManager gameManager;
    public Transform pointA;
    public Transform pointB;
    public float moveSpeed = 2f;
    public float danceSpeed = 1.5f;
    public float swayAmplitude = 0.5f;
    public float playerDanceDuration = 5f;
    public float interactionCooldown = 10f;

    private bool isDancingWithPlayer = false;
    private float cooldownTimer = 0f;
    private Vector3 initialPosition;
    private bool playerInRange = false; // Track if player is in range


    public TextMeshProUGUI lonerText; //Flavor text

    private void Start()
    {
        gameManager = GameManager.Instance;
        lonerText = gameManager.lonerText;
        initialPosition = transform.position;

        if (lonerText != null) // Flavor Text
        {
            lonerText.gameObject.SetActive(false);  // Hide text at the start
        }
    }

    private void Update()
    {
        // Handle cooldown for player interaction
        if (cooldownTimer > 0f)
        {
            cooldownTimer -= Time.deltaTime;
        }

        if (!isDancingWithPlayer)
        {
            // Move capsule in a continuous waltz pattern between point A and point B
            ContinuousWaltzMovement();
        }

        // Check if player is in range, cooldown has elapsed, and "E" is pressed
        if (playerInRange && cooldownTimer <= 0f && Input.GetKeyDown(KeyCode.E))
        {
            StartCoroutine(DanceWithPlayer(GameObject.FindGameObjectWithTag("Player").transform));
        }
    }

    private void ContinuousWaltzMovement()
    {
        // Calculate the position between point A and point B using PingPong
        float time = Mathf.PingPong(Time.time * moveSpeed, 1);
        Vector3 position = Vector3.Lerp(pointA.position, pointB.position, time);

        // Apply the left-right sway effect
        float sway = Mathf.Sin(Time.time * danceSpeed) * swayAmplitude;
        transform.position = new Vector3(position.x + sway, initialPosition.y, position.z);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true; // Set player in range
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false; // Reset player in range
        }
    }

    private IEnumerator DanceWithPlayer(Transform player)
    {
        isDancingWithPlayer = true;
        gameManager.isPlayerHiding = true;

        Vector3 playerOriginalPosition = player.position;
        Quaternion playerOriginalRotation = player.rotation;

        float timer = 0f;

        // Make player mimic capsule's waltz movement
        while (timer < playerDanceDuration)
        {
            timer += Time.deltaTime;

            // Set player position in front of the capsule, mimicking left-right sway
            player.position = transform.position + transform.forward * 1.5f;
            float playerSway = Mathf.Sin(Time.time * danceSpeed) * swayAmplitude;
            player.position = new Vector3(initialPosition.x + playerSway, player.position.y, player.position.z);

            // Rotate player to face capsule
            player.LookAt(transform);

            lonerText.gameObject.SetActive(true);  // Show the text

            yield return null;

            lonerText.gameObject.SetActive(false); // turn off text
        }

        // Restore player position and rotation
        player.position = playerOriginalPosition;
        player.rotation = playerOriginalRotation;

        // Start cooldown
        cooldownTimer = interactionCooldown;
        gameManager.isPlayerHiding = false;
        isDancingWithPlayer = false;
    }
}
