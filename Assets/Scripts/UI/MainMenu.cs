using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public Button playButton;
    public Button quitButton;
    public Button creditsButton;
    public Button acceptButton;
    public Button declineButton;
    public Button credReturnButton;

    public GameObject splashScreen;
    public GameObject creditsScreen;

    // Start is called before the first frame update
    void Start()
    {
        if (playButton != null)
            playButton.onClick.AddListener(OnPlayButtonClick);

        if (quitButton != null)
            quitButton.onClick.AddListener(OnQuitButtonClick);

        if (creditsButton != null)
            creditsButton.onClick.AddListener(OnCreditsButtonClick);

        if (acceptButton != null)
            acceptButton.onClick.AddListener(OnAcceptButtonClick);

        if (declineButton != null)
            declineButton.onClick.AddListener(OnDeclineButtonClick);

        if (credReturnButton != null)
            credReturnButton.onClick.AddListener(OnCredReturnButtonClick);

        splashScreen.SetActive(false);
        creditsScreen.SetActive(false);
    }

    private void OnPlayButtonClick()
    {
        Debug.Log("Splash Screen");
        splashScreen.SetActive(true);
    }

    private void OnAcceptButtonClick()
    {     
        SceneManager.LoadScene("Dani B - Map");
        splashScreen.SetActive(false);
        Debug.Log("Play the scene");
    }
    private void OnQuitButtonClick()
    {
        Debug.Log("Quit Game");
        Application.Quit();
    }

    private void OnDeclineButtonClick()
    {
        splashScreen.SetActive(false);
        Debug.Log("no thank u");
    }    
    
    private void OnCreditsButtonClick()
    {
        creditsScreen.SetActive(true);
        Debug.Log("creds");
    }

    private void OnCredReturnButtonClick()
    {
        creditsScreen.SetActive(false);
        Debug.Log("close creds");
    }
}
