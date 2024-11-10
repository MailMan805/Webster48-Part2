using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BlinkingAnimation : MonoBehaviour
{
    public Image displayImage;
    public Sprite[] images;
    public float flickerInterval = 0.5f;

    private Coroutine flickerCoroutine;

    private void Start()
    {
        if (images.Length > 0 && displayImage != null)
        {
            Debug.Log("BlinkingAnimation initialized.");
        }
        else
        {
            Debug.LogError("Images array or displayImage is not assigned properly.");
        }
    }

    public void StartFlickering()
    {
        if (flickerCoroutine == null && images.Length > 0 && displayImage != null)
        {
            flickerCoroutine = StartCoroutine(FlickerImages());
        }
    }

    private IEnumerator FlickerImages()
    {
        int index = 0;

        while (true)
        {
            displayImage.sprite = images[index];
            index = (index + 1) % images.Length;
            yield return new WaitForSeconds(flickerInterval);
        }
    }

    public void StopFlickering()
    {
        if (flickerCoroutine != null)
        {
            StopCoroutine(flickerCoroutine);
            flickerCoroutine = null;
        }
    }
}
