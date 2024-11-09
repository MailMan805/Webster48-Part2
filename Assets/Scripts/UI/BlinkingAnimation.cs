using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlinkingAnimation : MonoBehaviour
{
    // Reference to the UI Image component
    public Image displayImage;

    // Array to hold the images you want to cycle through
    public Sprite[] images;

    // Time interval between switching images
    public float flickerInterval = 0.5f;

    // Coroutine reference to allow stopping it if needed
    private Coroutine flickerCoroutine;

    private void Start()
    {
        // Check if images array and displayImage are properly assigned
        if (images.Length > 0 && displayImage != null)
        {
            // Start the flickering process
            Debug.Log("Starting the flickering effect.");
            flickerCoroutine = StartCoroutine(FlickerImages());
        }
        else
        {
            Debug.LogError("Images array or displayImage is not assigned properly.");
        }
    }

    // Coroutine that cycles through the images
    private IEnumerator FlickerImages()
    {
        int index = 0;

        // Infinite loop to keep flickering
        while (true)
        {
            // Set the sprite of the Image component to the current image in the array
            displayImage.sprite = images[index];
            Debug.Log($"Image switched to: {images[index].name}");

            // Increment the index and loop back to 0 if we reach the end of the array
            index = (index + 1) % images.Length;

            // Wait for the next interval before changing the image again
            yield return new WaitForSeconds(flickerInterval);
        }
    }

    // Optionally, stop the flickering (useful if you need to stop it based on certain conditions)
    public void StopFlickering()
    {
        if (flickerCoroutine != null)
        {
            StopCoroutine(flickerCoroutine);
            flickerCoroutine = null;
        }
    }
}