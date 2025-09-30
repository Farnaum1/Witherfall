using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public Rigidbody2D rb;
    public Animator animator;
    public ParticleSystem dustBurstFX;
    public ParticleSystem JumpFX;

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
    private bool isGrounded;

    [Header("Wall Check")]
    public Transform wallCheckPos;
    public Vector2 wallCheckSize = new Vector2(0.5f, 0.5f);
    public LayerMask wallLayer;

    [Header("Wall Slide")]
    public float wallSlideSpeed = 2f;
    private bool isWallSliding = false;

    [Header("Wall Jump")]
    private bool isWallJumping = false;
    public Vector2 wallJumpForce = new Vector2(5f, 10f);
    public float wallJumpDirection;
    public float wallJumpDuration = 0.5f;
    private float wallJumpTimer;

    [Header("Flip")]
    private bool isFacingRight = true;

    [Header("Gravity")]
    public float baseGravity = 2f;
    public float maxFallSpeed = 20f;
    public float fallMultiplier = 2f;




    void Start()
    {
        
    }

    void Update()
    {
      
    }

    private void FixedUpdate()
    {

        if (!isWallJumping)
        {
            // Apply horizontal movement
            rb.velocity = new Vector2(horizontalMovement * moveSpeed, rb.velocity.y);
            SetFlip();
        }

        GroundCheck();
        SetGravity();
        WallCheck();
        SetWallSlide();
        SetWallJump();

        // Update animator parameters
        animator.SetFloat("yVelocity", rb.velocity.y);
        animator.SetFloat("magnitude", rb.velocity.magnitude);
        animator.SetBool("isWallSliding", isWallSliding);
    }

    private void OnDrawGizmosSelected()
    {
        // Draw the ground check box in the editor for visualization
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(groundCheckPos.position, groundCheckSize);

        // Draw the wall check box in the editor for visualization
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(wallCheckPos.position, wallCheckSize);
    }

    public void Move(InputAction.CallbackContext context)
    {
            horizontalMovement = context.ReadValue<Vector2>().x;   
    }

    public void Jump(InputAction.CallbackContext context)
    {
        // Jump when performed and jumps remain
        if (context.performed && jumpsRemaining > 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            jumpsRemaining--;
            animator.SetTrigger("jump");

            if (jumpsRemaining == 0)
            {
                JumpFX.Play();
            }
        }
        // Reduce jump height when button released, if moving upwards
        else if (context.canceled && rb.velocity.y > 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
            jumpsRemaining--;
            animator.SetTrigger("jump");

            if (jumpsRemaining == 0)
            {
                JumpFX.Play();
            }
            
        }

        if (context.performed && wallJumpTimer > 0)
        {
            isWallJumping = true;
            // Determine wall jump direction based on facing direction
            rb.velocity = new Vector2(wallJumpDirection * wallJumpForce.x, wallJumpForce.y);
            wallJumpTimer = 0f; // Reset wall jump timer
            Invoke(nameof(CancelWallJump), wallJumpDuration + 0.1f); // Cancel wall jump after a short delay
            animator.SetTrigger("jump");
            dustBurstFX.Play();

            if (transform.localScale.x != wallJumpDirection)
            {
                // force flip if wall jump direction is opposite to facing direction
                isFacingRight = !isFacingRight;
                Vector2 localScale = transform.localScale;
                localScale.x *= -1f;
                transform.localScale = localScale;
                dustBurstFX.Play();
            }
        }

    }

    private void GroundCheck()
    {
        // Check if the player is grounded
        if (Physics2D.OverlapBox(groundCheckPos.position, groundCheckSize, 0f, groundLayer))
        {
            jumpsRemaining = maxJumps;
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
    }

    private void WallCheck()
    {
        // Check if the player is touching a wall
        if (Physics2D.OverlapBox(wallCheckPos.position, wallCheckSize, 0f, wallLayer))
        {
            // Reset jumps when touching a wall
            //jumpsRemaining = maxJumps;
        }
    }

    private void SetFlip()
    {
        if (isFacingRight && horizontalMovement < 0f || !isFacingRight && horizontalMovement > 0f)
        {
            // Flip the player's facing direction 
            isFacingRight = !isFacingRight;
            Vector2 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
            if (isGrounded)
            {
                dustBurstFX.Play();
            }
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

    private void SetWallSlide()
    {
        if (Physics2D.OverlapBox(wallCheckPos.position, wallCheckSize, 0f, wallLayer) && !isGrounded && horizontalMovement != 0)
        {
            isWallSliding = true;
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Max(rb.velocity.y, -wallSlideSpeed));
        }
        else
        {
            isWallSliding = false;
        }
    }

    private void SetWallJump()
    {
        if (isWallSliding)
        {
            isWallJumping = false;
            wallJumpDirection = -transform.localScale.x;
            wallJumpTimer = wallJumpDuration;
            CancelInvoke(nameof(CancelWallJump));
        }
        else if (wallJumpTimer > 0)
        {
            // Countdown wall jump timer
            wallJumpTimer -= Time.deltaTime;
        }
    }

    private void CancelWallJump()
    {
        isWallJumping = false;
    }

}
