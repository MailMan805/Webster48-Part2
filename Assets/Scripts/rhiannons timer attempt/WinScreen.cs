using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WinScreen : MonoBehaviour
{
    public TextMeshProUGUI winTimeText;  // Reference to the TextMeshProUGUI component

    void Start()
    {
        // Get the elapsed time from the GameManager
        float time = GameManager.Instance.GetElapsedTime();

        // Display the formatted time in the TextMeshProUGUI field
        winTimeText.text = FormatTime(time);
    }

    private string FormatTime(float time)
    {
        // Format the time as hh:mm:ss
        int hours = Mathf.FloorToInt(time / 3600); // 1 hour = 3600 seconds
        int minutes = Mathf.FloorToInt((time % 3600) / 60); // 1 minute = 60 seconds
        int seconds = Mathf.FloorToInt(time % 60);
        return string.Format("{0:00}:{1:00}:{2:00}", hours, minutes, seconds);
    }
}
