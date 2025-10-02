using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Runtime.CompilerServices;

public class GameController : MonoBehaviour
{
    public int progressAmount;
    public Slider progressSlider;

    public GameObject player;

    [SerializeField] private float loadDelay = 6f; // Delay before loading the next level

    void Start()
    {
        progressAmount = 0;
        progressSlider.value = 1;

        // Subscribe to the OnGemCollect event
        Gem.OnGemCollect += UpdateProgress;
        HoldToLoad.OnHoldComplete += LoadNextLevel;

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
        }

    }

    private void LoadNextLevel()
    {
        // Start the coroutine to load the next level after a delay
        StartCoroutine(LoadNextLevelWithDelay());
    }


    private IEnumerator LoadNextLevelWithDelay()
    {

        yield return new WaitForSeconds(loadDelay);


        // Get the current active scene index
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;

        // Calculate and load the next level index
        SceneManager.LoadScene(currentSceneIndex + 1);

        // If next scene index exceeds the total number of scenes, loop back to the first scene
        if (nextSceneIndex >= SceneManager.sceneCountInBuildSettings)
        {
            nextSceneIndex = 0; 
        }

        // Load the first scene
        SceneManager.LoadScene(nextSceneIndex);

    }

}