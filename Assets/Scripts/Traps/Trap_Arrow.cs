using UnityEngine;

public class Trap_Arrow : Trap_Trampoline // Inheriting properties and method from 'Trampoline script'
{
    [Header("Trap Setting")]
    [SerializeField] private float cooldown = 1.5f;
    [SerializeField] private bool rotationRight;
    [SerializeField] private float rotationSpeed = 120f;
    private int direction = -1;
    [Space]
    [SerializeField] private float scaleUpSpeed = 10f;
    [SerializeField] private Vector3 targetScale;

    private void Start()
    {
        transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
    }

    private void Update()
    {
        HandleScaleUp();
        HandleRotation();
    }

    // Scales the arrow up towards its target size over time
    private void HandleScaleUp()
    {
        if (transform.localScale.x < targetScale.x)
            transform.localScale = Vector3.Lerp(transform.localScale, targetScale, scaleUpSpeed * Time.deltaTime);
    }

    // Rotates the arrow based on the configured speed and direction
    private void HandleRotation()
    {
        direction = rotationRight ? -1 : 1;
        transform.Rotate(0, 0, (rotationSpeed * direction) * Time.deltaTime);
    }

    // Spawns a replacement arrow via GameManager and destroys the current instance
    private void DestroyIt()
    {
        GameObject arrowPrefab = ObjectCreator.Instance.arrowPrefab;
        ObjectCreator.Instance.CreateObject(arrowPrefab, transform, cooldown);

        Destroy(gameObject);
    }
}
