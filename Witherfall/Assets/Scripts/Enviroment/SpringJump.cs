using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpringJump : MonoBehaviour
{
    public Animator anim;
    public float jumpForce = 20f;
    public float launchDelay = 0.25f;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            anim.SetTrigger("boing");

            Rigidbody2D rb = collision.GetComponent<Rigidbody2D>();
            if (rb)
            {
                StartCoroutine(LaunchAfterDelay(rb));
            }
        }
    }

    private IEnumerator LaunchAfterDelay(Rigidbody2D rb)
    {
        yield return new WaitForSeconds(launchDelay);

        if (rb != null)
        {
            Animator playerAnim = rb.GetComponent<Animator>();
            playerAnim.SetTrigger("isSpringJumping");

            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
    }
}
