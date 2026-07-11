using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private string firstLevelSceneName = "Level_01";

    public void StartGame()
    {
        SceneManager.LoadScene(firstLevelSceneName);
    }

    public void ContinueGame()
    {
        // We'll connect this to save data later
        SceneManager.LoadScene(firstLevelSceneName);
    }

    public void OpenLevelSelect()
    {
        Debug.Log("Open Level Select");
        // We'll hook this to UIManager in the next step
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quit called");
    }
}
