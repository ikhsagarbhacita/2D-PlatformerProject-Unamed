using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] private GameObject fruitDrop;
    [SerializeField] private DifficultyType gameDifficulty;
    private GameManager gameManager;

    private Rigidbody2D rb;
    private Animator anim;
    private CapsuleCollider2D cd;

    public InputActionAsset playerInput {  get; private set; }
    private Vector2 moveInput;

    private bool canBeControlled = false; // Variable to enable or disable player's controll

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f; // 7f
    [SerializeField] private float jumpForce = 5.75f; // 12.75f
    [SerializeField] private float doubleJumpForce = 3f; // 9.5f // Force applied during the double jump
    private float defaultGravityScale; // Variable to ebable or disablle gravityScale
    private bool doubleJumpEnabled = false; // Variable to enable or disable double jump functionality

    [Header("Flying Settings")]
    [SerializeField] private float flyForce = 5f; // Force applied during flying
    [SerializeField] private float flyDuration = 0.6f; // Duration of the flying effect
    //[SerializeField] private KeyCode flyKey = KeyCode.LeftShift; // Key to activate flying
    private float flyCooldown = 0f; // Cooldown time between flying attempts
    private bool canFly = true; // Variable to enable or disable flying functionality
    private bool isFlying = false; // Variable to track if the player is currently flying   

    [Header("Buffer & Coyote Jump Settings")]
    [SerializeField] private float bufferJumpTime = 0.25f; // Time window for jump buffering when pressing the jump button before landing 
    private float bufferJumpActivated = -1; // Time window for jump buffering when pressing the jump button before landing
    [SerializeField] private float coyoteJumpTime = 0.5f; // Time window for coyote time when the player is in the air after leaving a platform
    private float coyoteJumpActivated = -1; // Time window for coyote time when the player is in the air after leaving a platform

    [Header("Wall Interaction Settings")]
    [SerializeField] private float wallJumpDuration = 0.15f; // Duration of the wall jump effect
    [SerializeField] private Vector2 wallJumpForce = new Vector2(7f, 12.5f); // Force applied during the wall jump
    private bool isWallJumping = false; // Variable to track if the player is currently wall jumping

    [Header("Knockback Settings")]
    [SerializeField] private float knockbackDuration = 1f; // Duration of the knockback effect
    [SerializeField] private Vector2 knockbackForce = new Vector2(5f, 5f); // Force applied during knockback
    private bool isKnockedback = false; // Variable to track if the player is currently being knocked back
    //private bool canBeKnockedback = true; // Variable to enable or disable knockback functionality

    [Header("Collison Settings")]
    [SerializeField] private float groundCheckDistance = 0.8f;
    [SerializeField] private float wallCheckDistance = 0.8f;
    [SerializeField] private LayerMask groundLayer; // Layer mask to specify which layers are considered ground
    [Space]
    [SerializeField] private Transform enemyCheck;
    [SerializeField] private float enemyCheckRadius;
    [SerializeField] private LayerMask enemyLayer; // Layer mask to specify which layers are cinsidered enemy
    private bool isGrounded; // Variable to track if the player is grounded
    private bool isAirborne; // Variable to track if the player is airborne
    private bool isWalled; // Variable to track if the player is against a wall
    private bool isFacingRight = true; // Variable to track the direction the player is facing
    private int isFacingDirection = 1; // Helping variable to determine the direction the player is facing (1 for right, -1 for left)

    [Header("Player Visuals")]
    [SerializeField] private AnimatorOverrideController[] animators;
    [SerializeField] private GameObject deathVFX;
    [SerializeField] private ParticleSystem dustFX; 
    [SerializeField] private int skinId;

    // Awake is called when the script instance is being loaded
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        cd = GetComponent<CapsuleCollider2D>();
        anim = GetComponentInChildren<Animator>();

        playerInput = GetComponent<PlayerInput>().actions;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        defaultGravityScale = rb.gravityScale;
        gameManager = GameManager.Instance;

        UpdateGameDifficulty();
        RespawnFinished(false); // Disable player control and gravity at the start of the game
    }

    // Update is called once per frame
    void Update()
    {
        AirborneStatus();

        if (canBeControlled == false)
        {
            HandleCollisions();
            HandleAnimations();
            return;
        }

        if (isKnockedback)
            return;

        HandleEnemyDetection();
        //HandleInput();
        HandleWallSlide();
        HandleFlying();
        HandleMovement();
        HandleFlip();
        HandleCollisions();
        HandleAnimations();
    }

    // Apply damage logic to the player based on current game difficulty
    public void Damage()
    {
        // Check player health status on Normal difficulty
        if (gameDifficulty == DifficultyType.Normal)
        {
            if (gameManager.FruitsCollected() <= 0)
            {
                Die();
            }
            else
            {
                //ObjectCreator.Instance.CreateObject(fruitDrop, transform, 0f, true);
                //gameManager.RemoveFruit(); // Remove the Fruits


                // Adds
                FruitType droppedType = gameManager.RemoveFruit();
                GameObject droppedFruitObj = ObjectCreator.Instance.CreateObjectAndReturn(fruitDrop, transform, 0f, true);

                if (droppedFruitObj != null)
                {
                    Fruit_DroppedByPlayer droppedScript = droppedFruitObj.GetComponent<Fruit_DroppedByPlayer>();
                    if (droppedScript != null)
                    {
                        droppedScript.SetFruitType(droppedType);
                    }
                }
            }

            return;
        }

        if (gameDifficulty == DifficultyType.Hard)
        {
            Die();
        }
    }

    // Update the game's difficulty based on the selected Difficulty
    private void UpdateGameDifficulty()
    {
        DifficultyManager difficultyManager = DifficultyManager.Instance;

        // 
        if (difficultyManager != null)
            gameDifficulty = difficultyManager.difficulty;
    }

    // Update the player's skin based on the selected skin ID
    public void UpdateSkin(int skinIndex)
    {
        SkinManager skinManager = SkinManager.Instance;

        if (skinManager == null)
            return;

        GetComponentInChildren<Animator>().runtimeAnimatorController = animators[skinIndex];
    }

    private void HandleEnemyDetection()
    {
        if (rb.linearVelocity.y >= 0)
            return;

        Collider2D[] colliders = Physics2D.OverlapCircleAll(enemyCheck.position, enemyCheckRadius, enemyLayer);

        foreach (var enemy in colliders)
        {
            Enemy newEnemy = enemy.GetComponent<Enemy>();

            if (newEnemy != null)
            {
                AudioManager.instance.PlaySFX(1);
                newEnemy.Die();
                Jump();
            }
        }
    }

    // Enable or disable player physics and controls when respawning
    public void RespawnFinished(bool finished)
    {
        if (finished)
        {
            rb.gravityScale = defaultGravityScale;
            canBeControlled = true;
            cd.enabled = true;

            AudioManager.instance.PlaySFX(11);
        }
        else
        {
            rb.gravityScale = 0;
            canBeControlled = false;
            cd.enabled = false;
        }
    }

    public void KnockBack(float sourceDamageXPosition)
    {
        float knockBackDir = 1;

        if (transform.position.x < sourceDamageXPosition)
            knockBackDir = -1;

        if (isKnockedback)
            return;

        AudioManager.instance.PlaySFX(9);
        CameraManager.instance.ScreenShake(knockBackDir);
        StartCoroutine(KnockbackCooldown());
        rb.linearVelocity = new Vector2(knockbackForce.x * knockBackDir, knockbackForce.y); // Apply knockback force in the opposite direction of the player
    }

    private IEnumerator KnockbackCooldown()
    {
        //canBeKnockedback = false; // Disable knockback during the cooldown period
        isKnockedback = true; // Set the knockback state to true to prevent movement during knockback
        anim.SetBool("knockBack", true);
        yield return new WaitForSeconds(knockbackDuration); // Wait for the knockback duration before allowing the player to move again
        //canBeKnockedback = true; // Enable knockback after the cooldown period
        isKnockedback = false; // Reset the knockback state to false to allow movement again
        anim.SetBool("knockBack", false);
    }

    public void Die()
    {
        AudioManager.instance.PlaySFX(0);

        GameObject newDeathVFX = Instantiate(deathVFX, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    // Apply push impulse force to the player for specified duration
    public void Push(Vector2 direction, float duration = 0f)
    {
        StartCoroutine(PushCourotine(direction, duration));
    }

    // Coroutine to apply push force and temporarily disable controls
    private IEnumerator PushCourotine(Vector2 direction, float duration)
    {
        canBeControlled = false;
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(direction, ForceMode2D.Impulse);

        yield return new WaitForSeconds(duration);
        canBeControlled = true;
    }

    private void AirborneStatus()
    {
        if (isGrounded && isAirborne)
            HandleLanding();

        if (!isAirborne && !isGrounded)
            BecomeAirborne();
    }

    private void BecomeAirborne()
    {
        isAirborne = true; // Player is in the air
        
        if (rb.linearVelocity.y <= 0)
        {
            RequestCoyoteJump();
        }
    }

    private void HandleLanding()
    {
        if (dustFX != null)
            dustFX.Play();

        isAirborne = false; // Player has landed
        doubleJumpEnabled = true; // Reset double jump when the player lands
        AttemptBufferJump(); // Attempt to perform a buffered jump if the jump button was pressed before landing

        // Reset flying ability and duration when the player lands
        canFly = true;
        flyCooldown = 0f;

        if (isFlying)
            StopFlying(); // Stop flying when the player lands
    }

    private void HandleInput()
    {
        //xInput = Input.GetAxisRaw("Horizontal");
        //yInput = Input.GetAxisRaw("Vertical");

        //if (Input.GetKeyDown(KeyCode.Space)) // Check if the jump button (spacebar) is pressed
        //{
        //    JumpButton();
        //    RequestBufferJump();
        //}

        //if (Input.GetKey(flyKey) && isAirborne && !isWalled && canFly) // Check if the fly key is pressed, the player is airborne, not against a wall, and can fly
        //{
        //    if (!isFlying) // Start the DustFX while start to Flying
        //    {
        //        isFlying = true; // Set the flying state to true when the fly key is pressed and the player is airborne and not against a wall
        //        if (dustFX != null && !dustFX.isPlaying)
        //            dustFX.Play();
        //    }
        //}
        //else
        //{
        //    if (isFlying)
        //        StopFlying(); // Stop flying when the fly key is released or the player is grounded or against a wall
        //}
    }

    private void HandleFlying()
    {
        if (!isFlying)
            return;

        rb.linearVelocity = new Vector2(rb.linearVelocity.x, flyForce); // Apply upward force during flying
        
        flyCooldown += Time.deltaTime; // Increment the fly cooldown timer while flying
        if (flyCooldown >= flyDuration) // Check if the fly cooldown timer has reached the fly duration
        {
            canFly = false; // Disable flying after the cooldown time
            StopFlying(); // Stop flying after the cooldown time
        }
    }

    private void StartFlying()
    {
        if (isAirborne && !isWalled && canFly)
        {
            if (!isFlying)
            {
                isFlying = true;
                if (dustFX != null && !dustFX.isPlaying)
                    dustFX.Play();
            }
        }
    }

    private void StopFlying()
    { 
        isFlying = false; // Reset the flying state

        if (dustFX != null && dustFX.isPlaying)
            dustFX.Stop(); // Stop DustFX
    }

    #region Buffer & Coyote Jump
    private void RequestBufferJump()
    {
        if (isAirborne)
            bufferJumpActivated = Time.time;
    }

    private void AttemptBufferJump()
    {
        if (Time.time < bufferJumpActivated + bufferJumpTime)
        {
            bufferJumpActivated = Time.time - 1; // Reset the buffer jump activation time to prevent multiple jumps
            Jump();
        }
    }

    private void RequestCoyoteJump() => coyoteJumpActivated = Time.time;
    private void CancelCoyoteJump() => coyoteJumpActivated = Time.time - 1;
    #endregion

    private void JumpButton()
    {
        bool canCoyoteJump = Time.time < coyoteJumpActivated + coyoteJumpTime; // Check if the player can perform a coyote jump

        if (isGrounded || canCoyoteJump) // Check if the player is grounded or can perform a coyote jump
        {
            Jump();
        }
        else if (isWalled && !isGrounded)
        {
            WallJump();
        }
        else if (doubleJumpEnabled)
        {
            DoubleJump();
        }

        CancelCoyoteJump(); // Cancel coyote jump after executing a jump to prevent multiple jumps
    }

    private void Jump() 
    {
        dustFX.Play();
        AudioManager.instance.PlaySFX(3);

        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
    }

    private void DoubleJump()
    {
        dustFX.Play();
        AudioManager.instance.PlaySFX(3);

        isWallJumping = false; // Reset wall jump state to allow movement during the double jump
        doubleJumpEnabled = false; // Disable double jump after using it
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, doubleJumpForce);
    }

    private void WallJump()
    {
        dustFX.Play();
        AudioManager.instance.PlaySFX(12);

        isWallJumping = true; // Set the wall jump state to true to prevent movement during the wall jump
        rb.linearVelocity = new Vector2(wallJumpForce.x * -isFacingDirection, wallJumpForce.y); // Apply force in the opposite direction of the wall
        
        Flip(); // Flip the player to face the opposite direction after the wall jump

        StopAllCoroutines(); // Stop any existing coroutines to prevent overlapping wall jump cooldowns
        StartCoroutine(WallJumpCooldown()); // Start the wall jump cooldown coroutine to prevent movement during the wall jump
    }

    private IEnumerator WallJumpCooldown()
    {
        isWallJumping = true; // Set the wall jump state to true to prevent movement during the wall jump
        yield return new WaitForSeconds(wallJumpDuration); // Wait for the wall jump duration before allowing the player to move again
        isWallJumping = false; // Reset the wall jump state to false to allow movement again
    }

    private void HandleWallSlide()
    {
        bool isSliding = isWalled && !isGrounded && rb.linearVelocity.y < 0; // Check if the player is sliding down a wall
        float yModifier = moveInput.y < 0 ? 1 : 0.05f; // If the player is pressing down, reduce the downward velocity more

        if (isSliding == false)
            return;

        rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * yModifier); // Reduce the downward velocity to create a wall slide effect
    }

    private void HandleCollisions()
    {
        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, groundLayer); // Check if the player is grounded by casting a ray downwards
        isWalled = Physics2D.Raycast(transform.position, Vector2.right * isFacingDirection, wallCheckDistance, groundLayer); // Check if the player is against a wall by casting a ray in the direction the player is facing
    }

    private void HandleAnimations()
    {
        anim.SetFloat("xVelocity", rb.linearVelocity.x);
        anim.SetFloat("yVelocity", rb.linearVelocity.y);
        anim.SetBool("isGrounded", isGrounded);
        anim.SetBool("isWalled", isWalled);
    }

    private void HandleMovement()
    {
        if (isWalled)
            return;

        if (isWallJumping)
            return;

        rb.linearVelocity = new Vector2(moveInput.x * moveSpeed, rb.linearVelocity.y);
    }

    private void HandleFlip()
    {
        if (moveInput.x < 0 && isFacingRight || moveInput.x > 0 && !isFacingRight)
            Flip();
    }

    private void Flip()
    {
        isFacingDirection *= -1; // Change the facing direction multiplier
        transform.Rotate(0f, 180f, 0f); // Rotate the player 180 degrees around the Y-axis
        isFacingRight = !isFacingRight;
    }

    // Draw a line in the Scene View to visualize the ground check distance
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(enemyCheck.position, enemyCheckRadius);
        Gizmos.DrawLine(transform.position, new Vector2(transform.position.x, transform.position.y - groundCheckDistance));
        Gizmos.DrawLine(transform.position, new Vector2(transform.position.x + (wallCheckDistance * isFacingDirection), transform.position.y));
    }

    // 
    private void OnEnable()
    {
        playerInput.Enable();

        playerInput.FindAction("Jump").performed += OnJumpPerformed; // Subs
        playerInput.FindAction("Movement").performed += OnMovementPerformed; // 
        playerInput.FindAction("Movement").canceled += OnMovementCanceled; // 
        playerInput.FindAction("Fly").performed += OnFlyPerformed; // 
        playerInput.FindAction("Fly").canceled += OnFlyCanceled; //
    }


    //
    private void OnDisable()
    {
        playerInput.Disable();

        playerInput.FindAction("Jump").performed -= OnJumpPerformed; // Unsubs
        playerInput.FindAction("Movement").performed -= OnMovementPerformed; // 
        playerInput.FindAction("Movement").canceled -= OnMovementCanceled; // 
        playerInput.FindAction("Fly").performed -= OnFlyPerformed; // 
        playerInput.FindAction("Fly").canceled -= OnFlyCanceled; //
    }

    private void OnFlyCanceled(InputAction.CallbackContext context)
    {
        StopFlying();
    }

    private void OnFlyPerformed(InputAction.CallbackContext context)
    {
        StartFlying();
    }

    private void OnMovementCanceled(InputAction.CallbackContext context)
    {
        moveInput = Vector2.zero;
    }

    private void OnMovementPerformed(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    private void OnJumpPerformed(InputAction.CallbackContext context)
    {
        JumpButton();
        AttemptBufferJump();
    }
}