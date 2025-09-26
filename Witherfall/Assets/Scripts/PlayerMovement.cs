using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public Rigidbody2D rb;

    [Header("Movement")]
    public float moveSpeed = 5f;
    private float horizontalMovement;

    [Header("Jumping")]
    public float jumpForce = 10f;
    public int maxJumps = 2;
    private int jumpsRemaining;

    [Header("Ground Check")]
    public Transform groundCheckPos;
    public Vector2 groundCheckSize = new Vector2(0.5f, 0.5f);
    public LayerMask groundLayer;

    [Header("Gravity")]
    public float baseGravity = 2f;
    public float maxFallSpeed = 20f;
    public float fallMultiplier = 2f;




    void Start()
    {
        
    }

    void Update()
    {
        // Apply horizontal movement
        rb.velocity = new Vector2(horizontalMovement * moveSpeed, rb.velocity.y);
    }

    private void FixedUpdate()
    {
        GroundCheck();
        SetGravity();
    }

    private void OnDrawGizmosSelected()
    {
        // Draw the ground check box in the editor for visualization
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(groundCheckPos.position, groundCheckSize);
    }

    public void Move(InputAction.CallbackContext context)
    {
        // Read horizontal movement input
        horizontalMovement = context.ReadValue<Vector2>().x;
    }

    public void Jump(InputAction.CallbackContext context)
    {
        // Jump when performed and jumps remain
        if (context.performed && jumpsRemaining > 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            jumpsRemaining--;
        }
        // Reduce jump height when button released, if moving upwards
        else if (context.canceled && rb.velocity.y > 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
        }
    }

    private void GroundCheck()
    {
        // Check if the player is grounded

        if (Physics2D.OverlapBox(groundCheckPos.position, groundCheckSize, 0f, groundLayer))
        {
            jumpsRemaining = maxJumps;
        }
    }

    private void SetGravity()
    {
        if (rb.velocity.y < 0)
        {
            // Falling: increase gravity for faster fall
            rb.gravityScale = baseGravity * fallMultiplier;
            rb.velocity = new Vector2 (rb.velocity.x, Mathf.Max(rb.velocity.y, -maxFallSpeed));
        }
        else
        {
            // Normal gravity
            rb.gravityScale = baseGravity;
        }
    }
}
