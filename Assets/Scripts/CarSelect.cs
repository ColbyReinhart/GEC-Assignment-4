using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CarSelect : MonoBehaviour
{
    public void Back()
    {
        SceneManager.LoadScene("Scenes/TrackSelect");
    }

    public void SelectCar(string name)
    {
        PlayerPrefs.SetString("SelectedCar", name);
        SceneManager.LoadScene(PlayerPrefs.GetString("SelectedTrack"));
    }
}
