using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public void LoadScene(string path)
    {
        SceneManager.LoadScene(path);
    }

    public void SelectMode(string mode)
    {
        PlayerPrefs.SetString("SelectedMode", mode);
    }

    public void SelectTrack(string track)
    {
        PlayerPrefs.SetString("SelectedTrack", track);
    }

    public void SelectCar(string car)
    {
        PlayerPrefs.SetString("SelectedCar", car);
    }

    public void StartRace()
    {
        LoadScene(PlayerPrefs.GetString("SelectedTrack"));
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
