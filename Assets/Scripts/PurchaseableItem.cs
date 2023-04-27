using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PurchaseableItem : MonoBehaviour
{
    public int price;
    public string playerPrefName;
    public TMP_Text text;
    public GameObject revealedButton;
    public AudioSource purchaseSound;

    private int currentCash;

    private void Start()
    {
        // Check if the player has already purchased
        if (PlayerPrefs.GetInt(playerPrefName) == 1)
        {
            revealedButton.gameObject.SetActive(true);
            Destroy(this);
            return;
        }

        // Assign price
        text.text = "Price: $" + price.ToString();

        // Get current cash
        currentCash = PlayerPrefs.GetInt("Cash");
    }

    public void PurchaseItem()
    {
        if (currentCash >= price)
        {
            // Update any cash counters in the scene
            foreach (var counter in GameObject.FindObjectsOfType<CashCounter>())
            {
                counter.UpdateAmount();
            }

            // Purchase the item
            purchaseSound.Play();
            currentCash -= price;
            revealedButton.SetActive(true);
            PlayerPrefs.SetInt("Cash", currentCash);
            PlayerPrefs.SetInt(playerPrefName, 1);
            this.gameObject.SetActive(false);
        }
    }
}
