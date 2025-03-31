using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public float elapsedTime = 0f;
    public Text timerText;
    public int hidecounter = 0;
    public int seekcounter = 0;

    public TextMeshProUGUI distractorText;
    public TextMeshProUGUI lonerText;

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
    public bool stop = false;
    public bool isJesterChasing = false;

    public bool Gameloop = false;

    public float timeToShowText = 3f;
    public GameObject Hide1;
    public GameObject Seek1;

    // Reference to the BlinkingAnimation script
    public BlinkingAnimation blinkingAnimation1;
    public BlinkingAnimation blinkingAnimation2;

    public AudioManager audioManager;


    public GameObject seekJesterMaskIcon;
    public GameObject hideGuardMaskIcon;

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
        Hide1 = FindAnyObjectByType<HideTag>().gameObject;
        Seek1 = FindAnyObjectByType<SeekTag>().gameObject;
        blinkingAnimation1 = Hide1.GetComponent<BlinkingAnimation>();
        blinkingAnimation2 = Seek1.GetComponent<BlinkingAnimation>();
        audioManager = AudioManager.Instance;
        audioManager.PlayMusic("HideDanceGame01");
        Hide1.gameObject.SetActive(false);
        Seek1.gameObject.SetActive(false);
        seekJesterMaskIcon.SetActive(false);
        hideGuardMaskIcon.SetActive(false);

        Gameloop = false;

        StartCoroutine(GameLoop());

        elapsedTime = 0f;
    }

    public void startGameLoop()
    {
        StartCoroutine(GameLoop());
    }

    private void Update()
    {
        
        if(Gameloop)
        {
            elapsedTime += Time.deltaTime;

            if (timerText != null)
            {
                timerText.text = FormatTime(elapsedTime);
            }
        }

        if (GameOverJester && !stop && isJesterChasing)
        {
            StopAllCoroutines();
            audioManager.PlaySFX("JesterKill");
            SceneManager.LoadScene("GO Jester");
            GameOverGuard = false;
            GameOverJester = false;
            isSeekMode = false;
            isHideMode = false;
            isNeutralMode = true;
            isWon = false;
            timesUp = false;
            Gameloop = false;
            stop = true;
            isJesterChasing = false;
            GameWon = false;
            elapsedTime = 0;
            Hide1.gameObject.SetActive(false);
            Seek1.gameObject.SetActive(false);
            seekJesterMaskIcon.SetActive(false);
            hideGuardMaskIcon.SetActive(false);

            Cursor.lockState = CursorLockMode.None;  // Unlocks the cursor
            Cursor.visible = true;                   // Makes the cursor visible

        }
        if (GameOverGuard && !stop)
        {
            StopAllCoroutines();
            audioManager.PlaySFX("GoreDeatht");
            SceneManager.LoadScene("GO Guard");
            GameOverGuard = false;
            GameOverJester = false;
            isSeekMode = false;
            isHideMode = false;
            isNeutralMode = true;
            isWon = false;
            timesUp = false;
            Gameloop = false;
            stop = true;
            isJesterChasing = false;
            GameWon = false;
            elapsedTime = 0;
            Hide1.gameObject.SetActive(false);
            Seek1.gameObject.SetActive(false);
            seekJesterMaskIcon.SetActive(false);
            hideGuardMaskIcon.SetActive(false);
            Cursor.lockState = CursorLockMode.None;  // Unlocks the cursor
            Cursor.visible = true;                   // Makes the cursor visible

        }
        if (isWon && !stop)
        {
            StopAllCoroutines();
            SceneManager.LoadScene("Tori Win Screen");
            GameOverGuard = false;
            GameOverJester = false;
            isSeekMode = false;
            isHideMode = false;
            isNeutralMode = true;
            isWon = false;
            timesUp = false;
            Gameloop = false;
            stop = true;
            isJesterChasing = false;
            GameWon = false;
            Hide1.gameObject.SetActive(false);
            Seek1.gameObject.SetActive(false);
            seekJesterMaskIcon.SetActive(false);
            hideGuardMaskIcon.SetActive(false);
            Cursor.lockState = CursorLockMode.None;  // Unlocks the cursor
            Cursor.visible = true;                   // Makes the cursor visible

        }
    }

    private IEnumerator GameLoop()
    {
        while (Gameloop)
        {
            Debug.Log("STARTED PLAYING");
                
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

            if (!isJesterChasing)
            {
                // Phase 2: Choose Hide or Seek Mode
                int phaseChoice = Random.Range(0, 2);
                if (phaseChoice == 0)
                {

                    if (hidecounter > 1)
                    {
                        phaseChoice = 1;
                        hidecounter = 0;
                    }
                    hidecounter += 1;
                }
                else
                {

                    if (seekcounter > 0)
                    {
                        phaseChoice = 0;
                        seekcounter = 0;
                    }
                    seekcounter += 1;
                }
                if (phaseChoice == 0)
                {
                    phaseDuration = Random.Range(15f, 25f);
                }
                else
                {
                    phaseDuration = Random.Range(30f, 45f);
                }


                if (phaseChoice == 0)
                {
                    audioManager.StopMusic();
                    audioManager.PlaySFX("RecordScratch");
                    audioManager.PlayMusic("HideDanceGame02");
                    StartCoroutine(FlashText(Hide1));
                    isHideMode = true;
                    isNeutralMode = false;
                    isSeekMode = false;
                    Debug.Log("Hide Mode");
                    hideGuardMaskIcon.SetActive(true);
                }
                else
                {
                    audioManager.StopMusic();
                    audioManager.PlaySFX("RecordScratch");
                    audioManager.PlayMusic("HideDanceGame02");
                    StartCoroutine(FlashText(Seek1));
                    isSeekMode = true;
                    isNeutralMode = false;
                    isHideMode = false;
                    Jester jester = FindObjectOfType<Jester>();
                    if (jester != null)
                    {
                        jester.chaseTimerActivated = false;
                    }
                    Debug.Log("Seek Mode");
                    seekJesterMaskIcon.SetActive(true);
                }

                // Start the flickering animation in Hide or Seek Mode
                blinkingAnimation1?.StartFlickering();
                blinkingAnimation2?.StartFlickering();


                yield return new WaitForSeconds(phaseDuration);

                // Back to Neutral Mode
                if(Gameloop)
                {
                    isNeutralMode = true;
                    isHideMode = false;
                    isSeekMode = false;
                    audioManager.StopMusic();
                    audioManager.PlayMusic("HideDanceGame01");
                    Debug.Log("Back to Neutral Mode");
                    seekJesterMaskIcon.SetActive(false);
                    hideGuardMaskIcon.SetActive(false);
                }
            }
            
        }
        Debug.Log("stopped PLAYING");
    


    }

    private IEnumerator FlashText(GameObject Text)
    {
        Text.gameObject.SetActive(true);
        yield return new WaitForSeconds(timeToShowText);
        Text.gameObject.SetActive(false);
    }

    private string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public float GetElapsedTime()
    {
        return elapsedTime;
    }

}
