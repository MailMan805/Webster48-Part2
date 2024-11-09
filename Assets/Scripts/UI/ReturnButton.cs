using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ReturnButton : MonoBehaviour
{
    public Button returnToMenuButton;

    // Start is called before the first frame update
    void Start()
    {
        if (returnToMenuButton != null)
            returnToMenuButton.onClick.AddListener(OnReturnButtonClick);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnReturnButtonClick()
    {
        Debug.Log("Return To Menu");
        SceneManager.LoadScene("Main Menu");
    }
}
