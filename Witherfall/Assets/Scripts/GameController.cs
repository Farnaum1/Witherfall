using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public int progressAmount;
    public Slider progressSlider;

    public GameObject player;
    public GameObject loadCanvas;
    public List<GameObject> levels;
    private int currentLevelIndex;

    void Start()
    {
        progressAmount = 0;
        progressSlider.value = 1;

        // Subscribe to the OnGemCollect event
        Gem.OnGemCollect += UpdateProgress;
        HoldToLoad.OnHoldComplete += LoadNextLevel;

        loadCanvas.SetActive(false);
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
            loadCanvas.SetActive(true);
        }

    }

    private void LoadNextLevel()
    {
        // if our current level index is the last index in the levels list, set it to 0, otherwise increment it by 1
        int nextLevelIndex = currentLevelIndex == levels.Count - 1 ? 0 : currentLevelIndex + 1;
        loadCanvas.SetActive(false);

        levels[currentLevelIndex].gameObject.SetActive(false);
        levels[nextLevelIndex].gameObject.SetActive(true);

        // Reset player position to origin or any desired position
        player.transform.position = new Vector3(0, 0, 0);

        currentLevelIndex = nextLevelIndex;
        progressAmount = 0;
        progressSlider.value = 0;
    }

}