using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }


    public bool isPlayerHiding = false;
    public bool isPlayerDistracted = false;
    public bool isNeutralMode = true;
    public bool isHideMode = false;
    public bool isSeekMode = false;
    public bool isWon = false;
    public bool timesUp = false;
    public float phaseDuration;
    public bool GameOverJester = false;
    public bool GameOverGuard = false;
    public bool GameWon = false;
    bool stop = false;
    public bool isJesterChasing = false;

    private bool Gameloop = true;

    public float timeToShowText = 3f;
    public GameObject Hide;
    public GameObject Seek;

    // Reference to the BlinkingAnimation script
    public BlinkingAnimation blinkingAnimation1;
    public BlinkingAnimation blinkingAnimation2;

    public AudioManager audioManager;

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
        audioManager = AudioManager.Instance;
        audioManager.PlayMusic("HideDanceGame01");
        Hide.gameObject.SetActive(false);
        Seek.gameObject.SetActive(false);

        StartCoroutine(GameLoop());
    }

    private void Update()
    {
        
        if (GameOverJester && !stop && isJesterChasing)
        {
            SceneManager.LoadScene("Tori GO Screen");
            stop = true;
            Cursor.lockState = CursorLockMode.None;  // Unlocks the cursor
            Cursor.visible = true;                   // Makes the cursor visible

        }
        if (GameOverGuard && !stop)
        {
            SceneManager.LoadScene("Tori GO Screen");
            stop = true;
            Cursor.lockState = CursorLockMode.None;  // Unlocks the cursor
            Cursor.visible = true;                   // Makes the cursor visible

        }
        if (isWon && !stop)
        {
            SceneManager.LoadScene("Tori Win Screen");
            stop = true;
            Cursor.lockState = CursorLockMode.None;  // Unlocks the cursor
            Cursor.visible = true;                   // Makes the cursor visible

        }
    }

    private IEnumerator GameLoop()
    {
        while (Gameloop)
        {
            int hidecounter = 0;
            int seekcounter = 0;
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
            if(phaseChoice == 0)
            {
               
                if(hidecounter >= 2)
                {
                    phaseChoice = 1;
                    hidecounter = 0;
                }
                hidecounter += 1;
            }
            else
            {

                if (seekcounter >= 2)
                {
                    phaseChoice = 0;
                    hidecounter = 0;
                }
                seekcounter += 1;
            }
            phaseDuration = Random.Range(15f, 25f);

            if (phaseChoice == 0)
            {
                audioManager.StopMusic();
                audioManager.PlayMusic("HideDanceGame02");
                StartCoroutine(FlashText(Hide));
                isHideMode = true;
                isNeutralMode = false;
                isSeekMode = false;
                Debug.Log("Hide Mode");
            }
            else
            {
                audioManager.StopMusic();
                audioManager.PlayMusic("HideDanceGame02");
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
            audioManager.StopMusic();
            audioManager.PlayMusic("HideDanceGame01");
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
