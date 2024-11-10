using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlashingMask : MonoBehaviour
{
    public Image maskDisplayImage;
    public Sprite[] images;
    public float maskFlickerInterval = 0.5f;

    private Coroutine flickerMaskCoroutine;

    private void Start()
    {
        if (images.Length > 0 && maskDisplayImage != null)
        {
            Debug.Log("Flashing Mask Animation initialized.");
            StartMaskFlickering();
        }
        else
        {
            Debug.LogError("Images array or displayImage is not assigned properly.");
        }
    }

    public void StartMaskFlickering()
    {
        if (flickerMaskCoroutine == null && images.Length > 0 && maskDisplayImage != null)
        {
            flickerMaskCoroutine = StartCoroutine(FlickerMask());
        }
    }

    private IEnumerator FlickerMask()
    {
        int index = 0;

        while (true)
        {
            maskDisplayImage.sprite = images[index];
            index = (index + 1) % images.Length;
            yield return new WaitForSeconds(Random.Range(0.05f, 0.5f));
        }
    }

    public void StopMaskFlickering()
    {
        if (flickerMaskCoroutine != null)
       {
            StopCoroutine(flickerMaskCoroutine);
            flickerMaskCoroutine = null;
        }
    }
}
