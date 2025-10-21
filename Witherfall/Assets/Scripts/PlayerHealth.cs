using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 3;
    private int currentHealth;
    private SpriteRenderer spriteRenderer;

    public HealthUI healthUI;

    [SerializeField] Light2D playerLight;

    void Start()
    {
        // Set current health to max health at the start
        currentHealth = maxHealth;

        // Initialize the SetMaxHearts method the health UI script 
        healthUI.SetMaxHearts(maxHealth);

        // Take the sprite renderer component
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Instantiate enemy component from the collided object
        Enemy enemy = collision.GetComponent<Enemy>();

        if (enemy)
        {
            TakeDamage(enemy.damage);
        }
            
    }

    private void TakeDamage(int damage)
    {
        currentHealth -= damage;
        healthUI.UpdateHearts(currentHealth);

        // Flash red effect
        StartCoroutine(FlashRed());

        if (currentHealth <= 0)
        {
            // Handle player death (e.g., reload scene, show game over screen, etc.)
            Debug.Log("Player has died!");
        }
    }

    private IEnumerator FlashRed()
    {
        // Flash the sprite red for a brief moment
        Color spriteOriginalColor = spriteRenderer.color;
        Color glowOriginalColor = playerLight.color;

        spriteRenderer.color = Color.red;
        playerLight.color = Color.red;
        playerLight.intensity = 1.5f;

        yield return new WaitForSeconds(0.2f);
        spriteRenderer.color = spriteOriginalColor;
        playerLight.color = glowOriginalColor;
        playerLight.intensity = 1f;

    }
}
