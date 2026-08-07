using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private SpriteRenderer sr => GetComponent<SpriteRenderer>();
    protected List<Player> playerList;
    protected Animator anim;
    protected Rigidbody2D rb;
    protected Collider2D[] colliders;


    [Header("Genaral Info")]
    [SerializeField] protected float moveSpeed = 2f;
    [SerializeField] protected float idleDuration = 1.5f;
    protected float idleTimer;
    protected bool canMove = true;


    [Header("Death Details")]
    [SerializeField] protected float deathImpactSpeed = 5f;
    [SerializeField] protected float deathRotationSpeed = 150f;
    protected int deathRotationDirection = 1;
    protected bool isDead;


    [Header("Collision Detector")]
    [SerializeField] protected float groundCheckDistance = 1.1f;
    [SerializeField] protected float wallCheckDistance = 0.7f;
    [SerializeField] protected LayerMask groundLayer;
    [SerializeField] protected float playerDetectionDistance = 15f;
    [SerializeField] protected LayerMask playerLayer;
    [SerializeField] protected Transform groundCheck;
    protected bool isPlayerDetected;
    protected bool isGrounded;
    protected bool isWalled;
    protected bool isGroundInfrontDetected;

    protected float isFacingDirection = -1f;
    protected bool isFacingRight = false;

    protected virtual void Awake()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        colliders = GetComponentsInChildren<Collider2D>();
    }

    protected virtual void Start()
    {
        if (sr.flipX == true && !isFacingRight)
        {
            sr.flipX = false;
            Flip();
        }

        PlayerManager.OnPlayerRespawn += UpdatePlayersRef;
        PlayerManager.OnPlayerDeath += UpdatePlayersRef;
    }

    private void UpdatePlayersRef()
    {
        playerList = PlayerManager.instance.GetPlayerList();
    }

    protected virtual void Update()
    {
        HandleCollisions();
        HandleAnimator();

        idleTimer -= Time.deltaTime; // Decrease the value

        if (isDead)
            HandleDeathRotation();
    }

    public virtual void Die()
    {
        foreach (var collider in colliders)
        {
            collider.enabled = false;
        }

        anim.SetTrigger("hit");
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, deathImpactSpeed);
        isDead = true;

        if (Random.Range(0, 100) < 50)
            deathRotationDirection = deathRotationDirection * -1;

        PlayerManager.OnPlayerDeath -= UpdatePlayersRef;
        PlayerManager.OnPlayerRespawn -= UpdatePlayersRef;
        Destroy(gameObject, 10);
    }


    private void HandleDeathRotation()
    {
        transform.Rotate(0, 0, (deathRotationSpeed * deathRotationDirection) * Time.deltaTime);
    }

    protected virtual void HandleFlip(float xValue)
    {
        if (xValue < transform.position.x && isFacingRight || xValue > transform.position.x && !isFacingRight)
            Flip();
    }

    protected virtual void Flip()
    {
        isFacingDirection *= -1; // Change the facing direction multiplier
        transform.Rotate(0f, 180f, 0f); // Rotate the player 180 degrees around the Y-axis
        isFacingRight = !isFacingRight;
    }

    [ContextMenu("Change Facing Direction")]
    public void FlipDefaultFacingDirection()
    {
        sr.flipX = !sr.flipX;
    }

    protected virtual void HandleAnimator()
    {
        anim.SetFloat("xVelocity", rb.linearVelocity.x);
    }

    protected virtual void HandleCollisions()
    {
        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, groundLayer); // 
        isGroundInfrontDetected = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, groundLayer); // Check if the player is grounded by casting a ray downwards
        isWalled = Physics2D.Raycast(transform.position, Vector2.right * isFacingDirection, wallCheckDistance, groundLayer); // Check if the player is against a wall by casting a ray in the direction the player is facing
        isPlayerDetected = Physics2D.Raycast(transform.position, Vector2.right * isFacingDirection, playerDetectionDistance, playerLayer); // 
    }

    // Draw a line in the Scene View to visualize the ground check distance
    protected virtual void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, new Vector2(transform.position.x, transform.position.y - groundCheckDistance));
        Gizmos.DrawLine(groundCheck.position, new Vector2(groundCheck.position.x, groundCheck.position.y - groundCheckDistance));
        Gizmos.DrawLine(transform.position, new Vector2(transform.position.x + (wallCheckDistance * isFacingDirection), transform.position.y));
        Gizmos.DrawLine(transform.position, new Vector2(transform.position.x + (playerDetectionDistance * isFacingDirection), transform.position.y));
    }
}