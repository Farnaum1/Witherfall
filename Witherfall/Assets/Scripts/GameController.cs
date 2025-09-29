using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    int progressAmount;
    public Slider progressSlider;
            
    void Start()
    {
        progressAmount = 0;
        progressSlider.value = 0;

        // Subscribe to the OnGemCollect event
        Gem.OnGemCollect += UpdateProgress;
    }

    void UpdateProgress(int amount)
    {
        progressAmount += amount;
        progressSlider.value = progressAmount;

        if (progressAmount >= 100)   
        {
            Destroy(GameObject.Find("Keycage"));
            Debug.Log("Keycage destroyed");
        }

    }


}