using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreAttackToken : MonoBehaviour
{
    public int worth = 500;

    private ScoreAttackUI ui;

    public void RegsisterUI(ScoreAttackUI ui)
    {
        this.ui = ui;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            ui.AddPoints(worth);
            gameObject.SetActive(false);
        }
    }
}
