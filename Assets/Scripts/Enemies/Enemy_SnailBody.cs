using UnityEngine;

public class Enemy_SnailBody : MonoBehaviour
{
    private SpriteRenderer sr;
    private Rigidbody2D rb;
    private float zRotation;

    public void SetUpBody(float yVelocity, float zRotation, int isFacingDirection)
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, yVelocity);

        this.zRotation = zRotation;

        if (isFacingDirection == 1 )
            sr.flipX = true;
    }

    private void Update()
    {
        transform.Rotate(0, 0, zRotation * Time.deltaTime);
    }
}
