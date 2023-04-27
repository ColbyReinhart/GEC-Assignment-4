using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CashCounter : MonoBehaviour
{
    private void Start()
    {
        TMP_Text cashCounter = GetComponent<TMP_Text>();
        cashCounter.text = "x " + PlayerPrefs.GetInt("Cash");
    }

    public void UpdateAmount()
    {
        Start();
    }
}
