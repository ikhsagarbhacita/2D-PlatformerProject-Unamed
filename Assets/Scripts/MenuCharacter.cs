using System.Collections;
using UnityEngine;
using UnityEngine.Windows;

public class MenuCharacter : MonoBehaviour
{
    [SerializeField] private float speed = 5f;

    private Vector3 destination;
    private Animator anim;

    private bool isMoving;
    private int isFacingDirection = 1;
    private bool isFacingRight = true;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }


    private void Update()
    {
        anim.SetBool("isMoving", isMoving);

        if (isMoving)
        {
            transform.position = Vector2.MoveTowards(transform.position, destination, Time.deltaTime * speed);

            if (Vector2.Distance(transform.position, destination) < 0.1f)
                isMoving = false;
        }
    }

    // 
    public void MoveTo(Transform newDestination)
    {
        destination = newDestination.position;
        destination.y = transform.position.y;

        isMoving = true;
        HandleFlip(destination.x);
    }

    private void HandleFlip(float xValue)
    {
        if (xValue < transform.position.x && isFacingRight || xValue > transform.position.x && !isFacingRight)
            Flip();
    }

    private void Flip()
    {
        isFacingDirection *= -1; // Change the facing direction multiplier
        transform.Rotate(0f, 180f, 0f); // Rotate the player 180 degrees around the Y-axis
        isFacingRight = !isFacingRight;
    }
}
