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

    // Start is called before the first frame update
    void Start()
    {
        if (playButton != null)
            playButton.onClick.AddListener(OnPlayButtonClick);

        if (quitButton != null)
            quitButton.onClick.AddListener(OnQuitButtonClick);
    }

private void OnPlayButtonClick()
    {
        Debug.Log("Play Scene");
        SceneManager.LoadScene("SampleScene");
    }

    private void OnQuitButtonClick()
    {
        Debug.Log("Quit Game");
        Application.Quit();
    }
}
