using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public string defaultTrackSelection = "Scenes/RaceArea01/RaceArea01";
    private string selectedTrack;

    private void Awake()
    {
        selectedTrack = PlayerPrefs.GetString("SelectedTrack", defaultTrackSelection);
    }

    public void PlayGame()
    {
        SceneManager.LoadScene(selectedTrack);
    }

    public void TrackSelect()
    {
        SceneManager.LoadScene("Scenes/TrackSelect");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
