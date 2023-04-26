using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadScene("Scenes/TrackSelect");
    }

    public void ViewCredits()
    {
        SceneManager.LoadScene("Scenes/Credits");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
