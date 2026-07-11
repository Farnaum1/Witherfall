using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [field: SerializeField] public UIScreen MainMenuScreen { get; private set; }
    [field: SerializeField] public UIScreen LevelSelectScreen { get; private set; }

    private UIScreen currentScreen;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        ShowScreen(MainMenuScreen);
    }

    public void ShowScreen(UIScreen newScreen)
    {
        if (currentScreen != null)
            currentScreen.Hide();

        currentScreen = newScreen;
        currentScreen.Show();
    }
}
