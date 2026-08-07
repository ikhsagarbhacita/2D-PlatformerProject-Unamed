using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Trap_FallingPlatform : MonoBehaviour
{
    private Animator anim;
    private Rigidbody2D rb;
    private BoxCollider2D[] colliders;

    [SerializeField] private float speed = 0.75f;
    [SerializeField] private float travelDistance = 0.5f;
    private Vector3[] wayPoints;
    private int wayPointIndex;
    private bool canMove = false;

    // Adds
    [Header("Platform Respawn")]
    [SerializeField] private float respawnDelay = 0.8f;
    [SerializeField] private float destroyDelay = 0.8f;
    private Vector3 defaultPosition;

    [Header("Trap Setting")]
    [SerializeField] private float impactSpeed = 3f; // Platform's movement fast when get touch
    [SerializeField] private float impactDuration = 0.1f; // Platform's movement duration when get touch
    private float impactTimer; // 
    private bool isImpact;
    [Space]
    [SerializeField] private float fallDelay = 0.5f;


    private void Awake()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        colliders = GetComponents<BoxCollider2D>();
    }

    private IEnumerator Start()
    {
        defaultPosition = transform.position;

        SetWayPoints();

        float randomDelay = Random.Range(0, 0.6f);
        yield return new WaitForSeconds(randomDelay);
        canMove = true;
    }

    // Calculates upper and lower waypoints relative to initial position
    public void SetWayPoints()
    {
        wayPoints = new Vector3[2];
        float yOffset = travelDistance / 2;

        wayPoints[0] = transform.position + new Vector3(0, yOffset, 0);
        wayPoints[1] = transform.position + new Vector3(0, -yOffset, 0);
    }

    private void Update()
    {
        // Adds
        if (this == null || !gameObject.activeInHierarchy)
            return;

        HandleImpact();
        HandleMovement();
    }

    // Handles ambient platform oscillation between waypoints
    private void HandleMovement()
    {
        if (!canMove || !gameObject.activeInHierarchy) // Adds if (canMove == false);
            return;

        transform.position = Vector2.MoveTowards(transform.position, wayPoints[wayPointIndex], speed * Time.deltaTime);

        if (Vector2.Distance(transform.position, wayPoints[wayPointIndex]) < 0.1f)
        {
            wayPointIndex++;

            if (wayPointIndex >= wayPoints.Length)
                wayPointIndex = 0;
        }
    }

    // Applies downward nudge effect upon player contact
    private void HandleImpact()
    {
        if (impactTimer < 0)
            return;

        impactTimer -= Time.deltaTime; // Decrease timer when > 0
        transform.position = // 
            Vector2.MoveTowards(transform.position, transform.position + (Vector3.down * 10), impactSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isImpact)
            return;

        Player player = collision.gameObject.GetComponent<Player>();

        if (player != null) 
        {
            Invoke(nameof(SwitchOffPlatform), fallDelay);
            impactTimer = impactDuration;
            isImpact = true;
        }
    }

    // Disables platform physics/colliders and triggers fall phase
    private void SwitchOffPlatform()
    {
        canMove = false;
        anim.SetTrigger("deactive");
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 3.5f;
        rb.linearDamping = 0.5f;

        foreach (BoxCollider2D collider in colliders)
        {
            collider.enabled = false;
        }

        Invoke(nameof(DestroyIt), destroyDelay); // Adds
    }

    // Adds
    private void DestroyIt()
    {
        CancelInvoke();
        StopAllCoroutines();

        if (GameManager.Instance != null)
        {
            GameObject platformPrefab = ObjectCreator.Instance.fallingPlatformPrefab;
            ObjectCreator.Instance.CreateObject(platformPrefab, defaultPosition, respawnDelay);
        }

        gameObject.SetActive(false);
        Destroy(gameObject);
    }
}
