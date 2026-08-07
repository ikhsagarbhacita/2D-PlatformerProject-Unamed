using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class Enemy_Chicken : Enemy
{
    [Header("Chicken Details")]
    [SerializeField] private float aggroDuration;
    [SerializeField] private float detectionRange;

    private Transform player;
    private float aggroTimer;
    private bool playerDetected;
    private bool canFlip = true;

    protected override void Update()
    {
        base.Update();

        aggroTimer -= Time.deltaTime;

        if (isDead)
            return;

        if (playerDetected)
        {
            canMove = true;
            aggroTimer = aggroDuration;
        }

        if (aggroTimer < 0)
            canMove = false;

        HandleMovement();

        if (isGrounded)
            HandleTurnAround();
    }

    private void HandleTurnAround()
    {
        if (!isGroundInfrontDetected || isWalled)
        {
            Flip();
            canMove = false;
            rb.linearVelocity = Vector2.zero;
        }
    }

    private void HandleMovement()
    {
        if (canMove == false)
            return;

        HandleFlip(player.position.x);

        rb.linearVelocity = new Vector2(moveSpeed * isFacingDirection, rb.linearVelocity.y);
    }

    protected override void HandleFlip(float xValue)
    {
        if (xValue < transform.position.x && isFacingRight || xValue > transform.position.x && !isFacingRight)
        {
            if (canFlip)
            {
                canFlip = false;
                Invoke(nameof(Flip), 0.3f);
            }
        }
    }

    protected override void Flip()
    {
        base.Flip();
        canFlip = true;
        FindClosestPlayer();
    }

    private void FindClosestPlayer()
    {
        float closesDistance = float.MaxValue;
        foreach (Player p in playerList)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, p.transform.position);

            if (distanceToPlayer < closesDistance)
            {
                closesDistance = distanceToPlayer;
                player = p.transform;
            }
        }
    }
}