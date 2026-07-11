using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSelectScreen : UIScreen
{
    public void LoadLevel1()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Level 1");
    }

    public void LoadLevel2()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Level 1");
    }

    public void LoadLevel3()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Level 1");
    }

    public void Back()
    {
        UIManager.Instance.ShowScreen(UIManager.Instance.MainMenuScreen);
    }
}
