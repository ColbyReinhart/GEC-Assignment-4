using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TrackSelect : MonoBehaviour
{
    public void MainMenu()
    {
        SceneManager.LoadScene("Scenes/MainMenu");
    }

    public void SelectTrack(string scenePath)
    {
        PlayerPrefs.SetString("SelectedTrack", scenePath);
        MainMenu();
    }
}
