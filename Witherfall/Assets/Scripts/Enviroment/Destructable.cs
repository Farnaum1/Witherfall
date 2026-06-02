using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destructable : MonoBehaviour
{
    [SerializeField] private int health = 1;
    [SerializeField] private GameObject destructionFX;

    [Header("Destruction Sound Effects")]
    [SerializeField] float SfxVolume;

    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (destructionFX != null)
        {
            Instantiate(destructionFX, transform.position, transform.rotation);
            AudioManager.Instance.PlaySFX(AudioManager.Instance.brickBreak, SfxVolume);
        }
        Destroy(gameObject);
    }
}
