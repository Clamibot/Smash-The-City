using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuButtonFunctions : MonoBehaviour
{
    public Text loadingText;
    public void RestartLevel()
    {
        // Start loading the Scene asynchronously and output the progress bar
        StartCoroutine(LoadScene());
        Time.timeScale = 1;
    }

    IEnumerator LoadScene()
    {
        yield return null;
        // Load the scene and display the loading progress
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(0);
        while (!asyncOperation.isDone)
        {
            loadingText.text = "Loading: " + (asyncOperation.progress * 100) + "%";
            yield return null;
        }
    }

    public void GoToMainMenu()
    {

    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
