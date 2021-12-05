using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class Timer : MonoBehaviour
{
    [Header("Timer Settings")]
    [Tooltip("Input in seconds but to be displayed in minutes")] public float totalTime = 300f;
    [HideInInspector] public float timeRemaining;
    public TMP_Text timeText;
    public string winScene;

    // Start is called before the first frame update
    void Start()
    {
        timeRemaining = totalTime;
        timeText.text = getMinuteSecondFormat(timeRemaining);
    }

    // Update is called once per frame
    void Update()
    {
        timeRemaining -= Time.deltaTime;

        //Update the UI
        timeText.text = getMinuteSecondFormat(timeRemaining);

        if (timeRemaining < 0)
        {
            WinGame();
        }
    }

    public string getMinuteSecondFormat(float seconds)
    {
        if (seconds % 60 < 10)
            return (int)(seconds / 60) + ":0" + (int)seconds % 60;
        else
            return (int)(seconds / 60) + ":" + (int)seconds % 60;
    }

    public void WinGame()
    {
        SceneManager.LoadScene(winScene);
    }
}