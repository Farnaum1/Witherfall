using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 3;
    private int currentHealth;
    private SpriteRenderer spriteRenderer;

    public HealthUI healthUI;

    [SerializeField] Light2D playerLight;

    [SerializeField] float slowMotionStrength = 0.2f;
    [SerializeField] float deathSequenceDelay = 1.5f;
    [SerializeField] float deathFlashDuration = 5f;

    [Header("IFrame")]
    [SerializeField] private float invincibilityDuration = 0.5f;
    private bool isInvincible = false;

    [Header("CameraShake")]
    [SerializeField] CameraShake cameraShake;

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

        if (enemy && !isInvincible)
        {
            TakeDamage(enemy.damage);
        }
            
    }

    IEnumerator InvincibilityFrames()
    {
        isInvincible = true;

        yield return new WaitForSeconds(invincibilityDuration);

        isInvincible = false;
    }

    private void TakeDamage(int damage)
    {
        currentHealth -= damage;
        healthUI.UpdateHearts(currentHealth);

        // Flash red effect
        StartCoroutine(FlashRed(0.2f));

        cameraShake.Shake();

        StartCoroutine(InvincibilityFrames());

        if (currentHealth <= 0)
        {
            // Handle player death (e.g., reload scene, show game over screen, etc.)

            StartCoroutine(DeathSequence());

        }
    }

    
    IEnumerator DeathSequence()
    {
        // Slow down time 
        Time.timeScale = slowMotionStrength;

        yield return new WaitForSecondsRealtime(deathSequenceDelay);

        yield return StartCoroutine(FlashRed(deathFlashDuration));

        Time.timeScale = 1f;

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

        
    }

    private IEnumerator FlashRed(float delay)
    {
        // Flash the sprite red for a brief moment
        Color spriteOriginalColor = spriteRenderer.color;
        Color glowOriginalColor = playerLight.color;

        spriteRenderer.color = Color.red;
        playerLight.color = Color.red;
        playerLight.intensity = 1.5f;

        yield return new WaitForSecondsRealtime(delay);
        spriteRenderer.color = spriteOriginalColor;
        playerLight.color = glowOriginalColor;
        playerLight.intensity = 1f;

    }
}
