using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Runtime.CompilerServices;
using System;

public class GameController : MonoBehaviour
{
    //Singleton
    public static GameController Instance { get; private set; }

    public static event Action<int> OnAmmoChanged;

    public int progressAmount;
    public int projectileAmount;
    public Slider progressSlider;

    public GameObject player;

    [SerializeField] private float loadDelay = 6f; 

    private void Awake()
    {
        //Singleton implementation
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    void Start()
    {
        progressAmount = 0;
        progressSlider.value = 1;

        // Subscribe to the OnGemCollect event
        Gem.OnGemCollect += UpdateProgress;
        SoulDust.OnSoulCollect += UpdateProjectile;
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

    private void UpdateProjectile(int amount)
    {
        projectileAmount += amount;
        OnAmmoChanged?.Invoke(projectileAmount);
    }

    public bool ConsumeProjectile()
    {
        if (projectileAmount <=0)
        {
            Debug.Log("No bullets");
            return false;
        }

        projectileAmount--;
        OnAmmoChanged?.Invoke(projectileAmount);
        return true;
    }

    private void LoadNextLevel()
    {
        // Start the coroutine to load the next level after a delay
        StartCoroutine(LoadNextLevelWithDelay());
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        projectileAmount = 0;
        OnAmmoChanged?.Invoke(projectileAmount);
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