using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthUI : MonoBehaviour
{
    [SerializeField] Image heartPrefab;
    [SerializeField] Sprite fullHeartSprite;
    [SerializeField] Sprite emptyHeartSprite;

    private List<Image> hearts = new List<Image>();


    public void SetMaxHearts(int maxHearts)
    {
        // Clear existing hearts
        foreach (Image heart in hearts)
        {
            Destroy(heart.gameObject);
        }
        hearts.Clear();


        // Instantiate new hearts
        for (int i = 0; i < maxHearts; i++)
        {
            Image heartInstance = Instantiate(heartPrefab, transform);
            heartInstance.sprite = fullHeartSprite;
            hearts.Add(heartInstance);
        }
    }

    public void UpdateHearts(int currentHealth)
    {
        for (int i = 0; i < hearts.Count; i++)
        {
            if (i < currentHealth)
            {
                hearts[i].sprite = fullHeartSprite;
            }
            else
            {
                hearts[i].sprite = emptyHeartSprite;
                hearts[i].color = Color.white;
            }
        }
    }



}
