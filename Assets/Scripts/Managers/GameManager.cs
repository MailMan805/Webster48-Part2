using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public bool isPlayerHiding = false;
    public bool isPlayerDistracted = false;
    public bool isNeutralMode = true;
    public bool isHideMode = false;
    public bool isSeekMode = false;
    public bool isWon = false;

    private void Awake()
    {
        if (Instance == null) // If there is no instance already
        {
            DontDestroyOnLoad(gameObject); // Keep the GameObject, this component is attached to, across different scenes
            Instance = this;
        }
        else if (Instance != this) // If there is already an instance and it's not `this` instance
        {
            Destroy(gameObject); // Destroy the GameObject, this component is attached to
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        // Start the game loop
        StartCoroutine(GameLoop());
    }

    private IEnumerator GameLoop()
    {
        while (true) // Infinite loop to keep repeating phases
        {
            // Phase 1: Neutral Mode - Wait for a random time between 10 and 15 seconds
            float neutralWaitTime = Random.Range(10f, 15f);
            isNeutralMode = true;
            isHideMode = false;
            isSeekMode = false;
            Debug.Log("Neutral Mode");
            yield return new WaitForSeconds(neutralWaitTime);

            // Phase 2: Choose Hide or Seek Mode - Random time between 15 and 25 seconds
            int phaseChoice = Random.Range(0, 2); // 0 = Hide, 1 = Seek
            float phaseDuration = Random.Range(15f, 25f);

            if (phaseChoice == 0)
            {
                isHideMode = true;
                isNeutralMode = false;
                isSeekMode = false;
                Debug.Log("Hide Mode");
            }
            else
            {
                isSeekMode = true;
                isNeutralMode = false;
                isHideMode = false;
                Debug.Log("Seek Mode");
            }

            yield return new WaitForSeconds(phaseDuration);

            // After Hide or Seek mode, return to Neutral mode
            isNeutralMode = true;
            isHideMode = false;
            isSeekMode = false;
            Debug.Log("Back to Neutral Mode");
        }
    }
}
