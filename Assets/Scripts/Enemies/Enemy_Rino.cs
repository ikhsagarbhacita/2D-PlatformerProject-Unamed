using System;
using Unity.Cinemachine;
using UnityEngine;

public class Enemy_Rino : Enemy
{
    [Header("Rino Details")]
    [SerializeField] private float maxSpeed = 8f;
    [SerializeField] private float speedUpRate = 0.6f;
    private float defaultSpeed;
    [SerializeField] private Vector2 impactPower;

    [Header("Effect")]
    [SerializeField] private ParticleSystem dustFX;
    [SerializeField] private Vector2 cameraImpulseDir;
    private CinemachineImpulseSource impulseSource;

    protected override void Start()
    {
        base.Start();

        canMove = false;
        defaultSpeed = moveSpeed;
        impulseSource = GetComponent<CinemachineImpulseSource>();
    }

    protected override void Update()
    {
        base.Update();

        HandleCharge();
    }

    private void HitWallImpact()
    {
        dustFX.Play();
        impulseSource.DefaultVelocity = new Vector2(cameraImpulseDir.x * isFacingDirection, cameraImpulseDir.y);
        impulseSource.GenerateImpulse();
    }

    private void HandleCharge()
    {
        if (canMove == false)
            return;

        HandleSpeedUp();

        rb.linearVelocity = new Vector2(moveSpeed * isFacingDirection, rb.linearVelocity.y);

        if (isWalled)
            WallHit();

        if (!isGroundInfrontDetected)
            TurnAround();
    }

    private void HandleSpeedUp()
    {
        moveSpeed = moveSpeed + (Time.deltaTime * speedUpRate);

        if (moveSpeed >= maxSpeed)
            maxSpeed = moveSpeed;
    }

    private void TurnAround()
    {
        SpeedReset();
        canMove = false;
        rb.linearVelocity = Vector2.zero;
        Flip();
        moveSpeed = defaultSpeed;
    }

    private void WallHit()
    {
        canMove = false;

        HitWallImpact();
        SpeedReset();

        anim.SetBool("hitWall", true);
        rb.linearVelocity = new Vector2(impactPower.x * -isFacingDirection, impactPower.y);
    }

    private void SpeedReset()
    {
        moveSpeed = defaultSpeed;
    }

    private void ChargeIsOver()
    {
        anim.SetBool("hitWall", false);
        Invoke(nameof(Flip), 1);
    }

    protected override void HandleCollisions()
    {
        base.HandleCollisions();

        if (isPlayerDetected && isGrounded)
            canMove = true;
    }
}
