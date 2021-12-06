using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public string gameScene;
    public AudioSource buttonSFX;

    public void startGame()
    {
        if (buttonSFX != null)
            buttonSFX.Play();

        SceneManager.LoadScene(gameScene);
    }

    public void quitButton()
    {
        if (buttonSFX != null)
            buttonSFX.Play();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
