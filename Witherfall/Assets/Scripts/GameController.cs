using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public int progressAmount;
    public Slider progressSlider;
            
    void Start()
    {
        progressAmount = 0;
        progressSlider.value = 1;

        // Subscribe to the OnGemCollect event
        Gem.OnGemCollect += UpdateProgress;
    }

    void UpdateProgress(int amount)
    {
        progressAmount += amount;
        progressSlider.value = progressAmount;

        if (progressAmount >= 100)   
        {
            // Clamp progressAmount to 100
            progressAmount = 100;
            Destroy(GameObject.Find("Keycage"));
            Debug.Log("Keycage destroyed");
        }

    }


}