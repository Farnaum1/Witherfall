using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public static event Action<ProjectileMovement, Vector2> OnShoot;

    public Rigidbody2D rb;
    public Animator animator;
    public ParticleSystem dustBurstFX;
    public ParticleSystem jumpFX;
    public ParticleSystem dashFX;

    [Header("Player Sound Effects")]
    [SerializeField] float jumpSfxVolume;
    [SerializeField] float jumpSfxMinPitch;
    [SerializeField] float jumpSfxMaxPitch;
    [SerializeField] float landSfxVolume;
    [SerializeField] float landSfxMinPitch;
    [SerializeField] float landSfxMaxPitch;


    [Header("Movement")]
    public float moveSpeed = 5f;
    private float horizontalMovement;

    [Header("Acceleration & Deceleration")]
    public float groundAcceleration = 60f;
    public float groundDeceleration = 50f;
    public float airAcceleration = 40f;
    public float airDeceleration = 35f;

    [Header("Jumping")]
    public float jumpForce = 10f;
    public int maxJumps = 2;
    private int jumpsRemaining;


    [Header("Jump Buffer")]
    [SerializeField] private float jumpBufferTime = 0.12f;
    private float jumpBufferCounter;

    [Header("Jump Forgiveness")]
    public float coyoteTime = 0.15f;
    private float coyoteCounter;

    [Header("Apex Bonus")]
    public float apexThreshold = 4f;
    public float apexBonus = 2f;
    private float apexPoint;

    [Header("Ground Check")]
    public Transform groundCheckPos;
    public Vector2 groundCheckSize = new Vector2(0.5f, 0.05f);
    public LayerMask groundLayer;
    private bool isGrounded;
    private float groundedRememberTime = 0.1f;
    private float groundedRemember;

    [Header("Wall Check")]
    public Transform wallCheckPos;
    public Vector2 wallCheckSize = new Vector2(0.5f, 0.5f);
    public LayerMask wallLayer;

    [Header("Wall Slide")]
    public float wallSlideSpeed = 2f;
    private bool isWallSliding = false;
    private bool wallLeft;
    private bool wallRight;
    private int lastWallSide = 0;
    private float wallContactTimer;
    public float wallContactBuffer = 0.1f;

    [Header("Wall Slide Friction")]
    public float wallSlideSlowSpeed = 1.5f;
    public float wallSlideFastSpeed = 6f;

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

    [Header("Player Shooting")]
    public GameObject projectilePrefab;
    public float wallSlideShootYOffset = -0.2f;
    [SerializeField] float spawnDistance = 1f;
    [SerializeField] Transform firePoint;

    [Header("Platform Support")]
    private MovingPlatform currentPlatform;
    private Vector3 lastPlatformDelta;

    [Header("Dash")]
    public bool enableDashes = true;
    public float dashSpeed = 20f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 0.5f;
    [SerializeField] float dashVerticalPush = 0f;
    [SerializeField] float stretchModifyX;
    [SerializeField] float stretchModifyY;
    [SerializeField] private TrailRenderer dashTrail;
    private float dashStretchX = 1f;
    private float dashStretchY = 1f;
    private bool isDashing = false;
    private bool canDash = true;
    private float dashTimer;
    private float dashCooldownTimer;
    private Vector2 dashDirection;
    private Coroutine scaleCoroutine;

    public GameObject afterImagePrefab;
    public float afterImageDelay = 0.05f;
    float afterImageTimer;


    private void Start()
    {
        //Trail is active on default
        if (dashTrail != null)
        {
            dashTrail.emitting = false;
        }
    }

    void Update()
    {
        animator.SetFloat("yVelocity", rb.velocity.y);
        animator.SetFloat("magnitude", rb.velocity.magnitude);
        animator.SetBool("isWallSliding", isWallSliding);
        animator.SetBool("isGrounded", isGrounded);

    }

    private void FixedUpdate()
    {
        GroundCheck();
        WallCheck();
        SetWallSlide();
        SetWallJump();

        SetJumpCounters();
        TryExecuteBufferedJump();

        SetGravity();
        CalculateApexBonusSpeed();
        ApplyPlatformDelta();
        SetDash();

    }

   

    private void ApplyPlatformDelta()
    {
        if (currentPlatform != null)
        {
            lastPlatformDelta = currentPlatform.DeltaMovement;
            transform.position += lastPlatformDelta;
        }
        else
        {
            lastPlatformDelta = Vector3.zero;
        }

        if (!isWallJumping)
        {
            ApplyHorizontalMovement();

            if (!isWallSliding)
            {
                SetFlip();
            }
                
        }
    }

    private void ApplyHorizontalMovement()
    {
        float platformVelX = lastPlatformDelta.x / Time.fixedDeltaTime;
        float targetSpeed = horizontalMovement * CalculateApexBonusSpeed();
        float accelRate;

        // Choose acceleration depending on grounded state
        if (isGrounded)
        {
            accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? groundAcceleration : groundDeceleration;
        }
        else
        {
            accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? airAcceleration : airDeceleration;
        }

        // Move velocity.x toward targetSpeed
        float newX = Mathf.MoveTowards(rb.velocity.x,
                                       targetSpeed + platformVelX,
                                       accelRate * Time.fixedDeltaTime);

        rb.velocity = new Vector2(newX, rb.velocity.y);
    }

    private void OnCollisionEnter2D(Collision2D c)
    {
        if (((1 << c.collider.gameObject.layer) & groundLayer) != 0)
        {
            if (c.collider.TryGetComponent(out MovingPlatform platform))
            {
                // Only attach if landing on top
                foreach (var contact in c.contacts)
                {
                    if (contact.normal.y > 0.5f)
                    {
                        currentPlatform = platform;
                        break;
                    }
                }
            }
        }
    }

    private void OnCollisionExit2D(Collision2D c)
    {
        if (c.collider.TryGetComponent(out MovingPlatform platform))
        {
            if (currentPlatform == platform)
            {
                currentPlatform = null;
            }
        }
    }

    public void Move(InputAction.CallbackContext context)
    {
        horizontalMovement = context.ReadValue<Vector2>().x;
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {

            if (isDashing) return;

            // Wall Jump
            if (wallJumpTimer > 0)
            {
                isWallJumping = true;
                wallJumpDirection = -lastWallSide;

                if ((wallJumpDirection > 0 && !isFacingRight) || (wallJumpDirection < 0 && isFacingRight))
                {
                    isFacingRight = wallJumpDirection > 0;
                    Vector3 localScale = transform.localScale;
                    localScale.x = Mathf.Abs(localScale.x) * wallJumpDirection;
                    transform.localScale = localScale;
                }

                rb.velocity = new Vector2(
                    wallJumpDirection * wallJumpForce.x,
                    wallJumpForce.y
                );

                wallJumpTimer = 0f;

                animator.SetTrigger("jump");
                dustBurstFX.Play();
                AudioManager.Instance.PlaySFX(AudioManager.Instance.playerWallJump,
                    jumpSfxVolume);

                // prevent wall stick re-trigger
                CancelInvoke(nameof(CancelWallJump));
                Invoke(nameof(CancelWallJump), wallJumpDuration);

                return;
            }

            // Air Jump
            if (!isGrounded && jumpsRemaining > 0 && coyoteCounter <= 0f)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                jumpsRemaining--;

                animator.SetTrigger("jump");

                if (jumpsRemaining == 0)
                    jumpFX.Play();
                    AudioManager.Instance.PlaySFXRandomPitch(AudioManager.Instance.playerDoubleJump,
                       jumpSfxVolume, jumpSfxMinPitch, jumpSfxMaxPitch );

                return;
            }

            // Store jump buffer
            jumpBufferCounter = jumpBufferTime;
        }

        // Jump Cut
        if (context.canceled && rb.velocity.y > 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
        }
    }

    private void SetJumpCounters()
    {
        if (jumpBufferCounter > 0f)
        {
            jumpBufferCounter -= Time.deltaTime;
            if (jumpBufferCounter < 0f)
                jumpBufferCounter = 0f;
        }

        if (coyoteCounter > 0f)
        {
            coyoteCounter -= Time.deltaTime;
        }
    }

    private void TryExecuteBufferedJump()
    {
        if (jumpBufferCounter > 0f && coyoteCounter > 0f && jumpsRemaining > 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            jumpsRemaining--;
            jumpBufferCounter = 0f;

            animator.SetTrigger("jump");
            AudioManager.Instance.PlaySFX(AudioManager.Instance.playerJump);

            if (jumpsRemaining == 0)
                jumpFX.Play();
        }
    }

    public float CalculateApexBonusSpeed()
    {
        // Only apply apex bonus when falling or at peak
        if (rb.velocity.y > 0)
            return moveSpeed;  // moving upward = no apex bonus

        // How close we are to apex (0 = peak)
        apexPoint = Mathf.InverseLerp(apexThreshold, 0, Mathf.Abs(rb.velocity.y));

        float bonusSpeed = apexPoint * apexBonus;
        return moveSpeed + bonusSpeed;
    }

    public void Shoot(InputAction.CallbackContext context)
    {
        if (context.performed && GameController.Instance.ConsumeProjectile())
        {
            Vector2 direction;

            // override direction when wall sliding
            if (isWallSliding)
            {
                if (wallLeft)
                    direction = Vector2.right;
                else if (wallRight)
                    direction = Vector2.left;
                else
                    direction = isFacingRight ? Vector2.right : Vector2.left;
            }
            else
            {
                direction = isFacingRight ? Vector2.right : Vector2.left;
            }

            Vector3 offset = Vector3.zero;
            if (isWallSliding)
                offset = new Vector3(0f, wallSlideShootYOffset, 0f);

            Vector3 spawnPos = firePoint != null
                ? firePoint.position + offset
                : transform.position + (Vector3)(direction * spawnDistance) + offset;

            GameObject proj = Instantiate(projectilePrefab, spawnPos, Quaternion.identity);
            ProjectileMovement projectile = proj.GetComponent<ProjectileMovement>();

            OnShoot?.Invoke(projectile, direction);
        }
    }

    public void Dash(InputAction.CallbackContext context)
    {
        if (!enableDashes) return;

        if (context.performed)
        {
            StartDash();
        }
    }

    private void SetDash()
    {
        if (dashCooldownTimer > 0f)
        {
            dashCooldownTimer -= Time.fixedDeltaTime;
        }

        if (dashCooldownTimer <= 0f)
        {
            canDash = true;
        }

        if (isDashing)
        {
            dashTimer -= Time.fixedDeltaTime;

            rb.velocity = new Vector2(dashDirection.x * dashSpeed, dashVerticalPush);

            if (afterImageTimer <= 0)
            {
                SpawnAfterImage();
                afterImageTimer = afterImageDelay;
            }
            else
            {
                afterImageTimer -= Time.deltaTime;
            }

            if (dashTimer <= 0f)
            {
                EndDash();
            }
                

            return;
        }
    }

    private void SpawnAfterImage()
    {
        GameObject img = Instantiate(afterImagePrefab, transform.position, Quaternion.identity);

        SpriteRenderer imgSR = img.GetComponent<SpriteRenderer>();
        SpriteRenderer playerSR = GetComponent<SpriteRenderer>();

        imgSR.sprite = playerSR.sprite;
        img.transform.localScale = transform.localScale;
    }

    private void StartDash()
    {
        if (!canDash) return;
        if (isDashing) return;
        if (isWallJumping) return;
        if (isWallSliding) return;

        isDashing = true;
        canDash = false;
        dashTimer = dashDuration;
        dashCooldownTimer = dashCooldown;

        dashStretchX = stretchModifyX;
        dashStretchY = stretchModifyY;

        dashDirection = isFacingRight ? Vector2.right : Vector2.left;

        float facingSign = isFacingRight ? 1f : -1f;

        //Need to stop any existing SmoothScale coroutine whenever a new one starts
        if (scaleCoroutine != null) StopCoroutine(scaleCoroutine);
        scaleCoroutine = StartCoroutine(SmoothScale(new Vector3(dashStretchX * facingSign, dashStretchY, 1f), dashDuration));

        dashTrail.emitting = true;
        dashFX.Play();
        animator.SetBool("isDashing", true);
        dashFX.transform.localRotation = Quaternion.Euler(0, 0, isFacingRight ? 180 : 0);

        rb.velocity = new Vector2(dashDirection.x * dashSpeed, 0f);

        // Optional: stop wall slide while dashing
        isWallSliding = false;
    }

    private void EndDash()
    {
        dashStretchX = 1f;
        dashStretchY = 1f;

        float facingSign = isFacingRight ? 1f : -1f;

        //Need to stop any existing SmoothScale coroutine whenever a new one starts
        if (scaleCoroutine != null) StopCoroutine(scaleCoroutine);
        scaleCoroutine = StartCoroutine(SmoothScale(new Vector3(dashStretchX * facingSign, dashStretchY, 1f), dashDuration));


        dashTrail.emitting = false;
        animator.SetBool("isDashing", false);

        isDashing = false;

    }

    public float GetDashFill()
    {
        return 1f - (dashCooldownTimer / dashCooldown);
    }

    private IEnumerator SmoothScale(Vector3 newScale, float duration)
    {
        Vector3 startScale = transform.localScale;
        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            float lerp = t / duration;

            transform.localScale = Vector3.Lerp(startScale, newScale, lerp);

            yield return null;
        }

        transform.localScale = newScale;
    }

    private void GroundCheck()
    {
        bool isMovingUp = rb.velocity.y > 0.1f;

        if (Physics2D.OverlapBox(groundCheckPos.position, groundCheckSize,
            0f, groundLayer) && !isMovingUp)
        {
            if (!isGrounded)
            {
                jumpsRemaining = maxJumps;
                groundedRemember = groundedRememberTime;
                AudioManager.Instance.PlaySFXRandomPitch(AudioManager.Instance.playerLand,landSfxVolume, landSfxMinPitch, landSfxMaxPitch);
            }

            //isGrounded = true;
            isGrounded = groundedRemember > 0;
            coyoteCounter = coyoteTime;
            
        }
        else
        {
            isGrounded = false;
            groundedRemember -= Time.deltaTime;
        }
    }

    private void WallCheck()
    {
        Collider2D hit = Physics2D.OverlapBox(wallCheckPos.position, wallCheckSize, 0f, wallLayer);

        if (hit == null)
        {
            wallLeft = false;
            wallRight = false;
            wallContactTimer -= Time.deltaTime;
            return;
        }

        wallLeft = wallCheckPos.position.x < transform.position.x;
        wallRight = wallCheckPos.position.x > transform.position.x;

        if (wallLeft)
        {
            lastWallSide = -1;
            wallContactTimer = wallContactBuffer;
        }
        else if (wallRight)
        {
            lastWallSide = 1;
            wallContactTimer = wallContactBuffer;
        }
    }

    private void SetFlip()
    {
        if (isFacingRight && horizontalMovement < 0f || !isFacingRight && horizontalMovement > 0f)
        {
            isFacingRight = !isFacingRight;

            // STOP the coroutine so it doesn't overwrite your manual flip
            if (scaleCoroutine != null) StopCoroutine(scaleCoroutine);

            float facingSign = isFacingRight ? 1f : -1f;
            transform.localScale = new Vector3(dashStretchX * facingSign, dashStretchY, 1f);

            if (isGrounded)
                dustBurstFX.Play();
        }
    }

    private void SetGravity()
    {
        if (!isGrounded && rb.velocity.y < -0.1f)
        {
            rb.gravityScale = baseGravity * fallMultiplier;
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Max(rb.velocity.y, -maxFallSpeed));
        }
        else
        {
            rb.gravityScale = baseGravity;
        }
    }

    private void SetWallSlide()
    {
        // do not slide if grounded
        if (isGrounded)
        {
            isWallSliding = false;
            return;
        }

        // only block slides if wall jump just started (short grace period)
        if (isWallJumping && !wallLeft && !wallRight)
        {
            isWallSliding = false;
            return;
        }

        bool touchingWall = wallContactTimer > 0;

        // must be touching a wall to begin sliding
        if (!touchingWall)
        {
            isWallSliding = false;
            return;
        }

        // determine friction based on input
        bool pushingTowardWall = (wallLeft && horizontalMovement < 0) || (wallRight && horizontalMovement > 0);
        float targetSlideSpeed = pushingTowardWall ? wallSlideSlowSpeed : wallSlideFastSpeed;

        // detect start of wall slide
        if (!isWallSliding)
        {
            jumpsRemaining = maxJumps-1;
        }

        // apply wall slide
        isWallSliding = true;

        if (rb.velocity.y < -targetSlideSpeed)
        {
            rb.velocity = new Vector2(rb.velocity.x, -targetSlideSpeed);
        }
           
    }

    private void SetWallJump()
    {
        if (isWallSliding)
        {
            // reset wall jump timer while sliding
            wallJumpTimer = wallJumpDuration;
            wallJumpDirection = -lastWallSide;
            return;
        }

        if (wallJumpTimer > 0)
        {
            wallJumpTimer -= Time.deltaTime;
        }
            
    }

    private void CancelWallJump()
    {
        isWallJumping = false;
    }
}
