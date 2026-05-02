using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UIElements;

public class Enemy : MonoBehaviour
{
    [SerializeField] Transform player;
    [SerializeField] float chaseSpeed;
    [SerializeField] float jumpForce;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] GameObject headHitbox;
    [SerializeField] BoxCollider2D headCollider;

    private Rigidbody2D enemyRb;
    private bool isGrounded;
    private bool shouldJump;
    private Vector2 directionToPlayer;

    private Animator animator;

    [SerializeField] float groundRaycastDistance;
    [SerializeField] float gapRaycastDistance;
    [SerializeField] float platformRaycastDistance;

    public int damage = 1;



    void Start()
    {
        // Get components
        enemyRb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }


    void Update()
    {
        animator.SetFloat("EnemyVelocity", Mathf.Abs(enemyRb.velocity.x));
        animator.SetBool("IsGrounded", isGrounded);

        // Check if the enemy is grounded
        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, 0.7f, groundLayer);

        // Direction of enemy to player
        directionToPlayer = (player.position - transform.position).normalized;

        // Check if player is above enemy
        bool isPlayerAbove = Physics2D.Raycast(transform.position, Vector2.up, 3f, LayerMask.GetMask("Player"));

        if (isGrounded)
        {
            MoveEnemy();

            // Jump if there's gap ahead && no grounds infront
            // Else if there's player above and platform above

            // Check for ground in front of enemy
            RaycastHit2D groundInfront = Physics2D.Raycast(transform.position, new Vector2(directionToPlayer.x, 0), groundRaycastDistance, groundLayer);

            // Check for gap in front of enemy
            RaycastHit2D gapInfront = Physics2D.Raycast(transform.position + new Vector3(directionToPlayer.x, 0, 0), Vector2.down, gapRaycastDistance, groundLayer);

            // Check for platform above enemy
            RaycastHit2D platformAbove = Physics2D.Raycast(transform.position, Vector2.up, platformRaycastDistance, groundLayer);

            // Decide to jump
            if (!groundInfront.collider && !gapInfront.collider)
            {
                shouldJump = true;
            }
            else if (isPlayerAbove && platformAbove.collider)
            {
                shouldJump = true;
            }

        }
    }

    private void FixedUpdate()
    {
        if (isGrounded && shouldJump)
        {
            // Reset jump flag
            shouldJump = false;

            // Calculate direction to player
            Vector2 direction = (player.position - transform.position).normalized;

            // Defining jump direction
            Vector2 jumpDirection = direction * jumpForce;

            // Apply jump force
            enemyRb.AddForce(new Vector2(jumpDirection.x, jumpForce), ForceMode2D.Impulse);
        }
    }

    private void MoveEnemy()
    {
        // Move the enemy left and right
        enemyRb.velocity = new Vector2(directionToPlayer.x * chaseSpeed, enemyRb.velocity.y);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider == headCollider && collision.gameObject.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
    }

}

