using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public bool isPlayerHiding = false;
    public bool isPlayerDistracted = false;
    public bool isNeutralMode = true;
    public bool isHideMode = false;
    public bool isSeekMode = false;
    public bool isWon = false;
    public bool timesUp = false;
    public float phaseDuration;

    public float timeToShowText = 3f;
    public GameObject Hide;
    public GameObject Seek;

    // Reference to the BlinkingAnimation script
    public BlinkingAnimation blinkingAnimation1;
    public BlinkingAnimation blinkingAnimation2;

    private void Awake()
    {
        if (Instance == null)
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        Hide.gameObject.SetActive(false);
        Seek.gameObject.SetActive(false);

        StartCoroutine(GameLoop());
    }

    private IEnumerator GameLoop()
    {
        while (true)
        {
            // Phase 1: Neutral Mode
            float neutralWaitTime = Random.Range(10f, 15f);
            isNeutralMode = true;
            isHideMode = false;
            isSeekMode = false;
            Debug.Log("Neutral Mode");

            // Stop the flickering if active
            blinkingAnimation1?.StopFlickering();
            blinkingAnimation2?.StopFlickering();
            yield return new WaitForSeconds(neutralWaitTime);

            // Phase 2: Choose Hide or Seek Mode
            int phaseChoice = Random.Range(0, 2);
            phaseDuration = Random.Range(15f, 25f);

            if (phaseChoice == 0)
            {
                StartCoroutine(FlashText(Hide));
                isHideMode = true;
                isNeutralMode = false;
                isSeekMode = false;
                Debug.Log("Hide Mode");
            }
            else
            {
                StartCoroutine(FlashText(Seek));
                isSeekMode = true;
                isNeutralMode = false;
                isHideMode = false;
                Debug.Log("Seek Mode");
            }

            // Start the flickering animation in Hide or Seek Mode
            blinkingAnimation1?.StartFlickering();
            blinkingAnimation2?.StartFlickering();
            yield return new WaitForSeconds(phaseDuration);

            // Back to Neutral Mode
            isNeutralMode = true;
            isHideMode = false;
            isSeekMode = false;
            Debug.Log("Back to Neutral Mode");
        }
    }

    private IEnumerator FlashText(GameObject Text)
    {
        Text.gameObject.SetActive(true);
        yield return new WaitForSeconds(timeToShowText);
        Text.gameObject.SetActive(false);
    }
}
