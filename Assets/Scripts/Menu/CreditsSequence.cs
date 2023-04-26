using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditsSequence : MonoBehaviour
{
    public float duration = 3f;

    private void Start()
    {
        StartCoroutine(DoCreditsSequence());
    }

    private IEnumerator DoCreditsSequence()
    {
        yield return new WaitForSeconds(duration);
        SceneManager.LoadScene("Scenes/MainMenu");
    }
}
