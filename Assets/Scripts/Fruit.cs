using UnityEngine;

public enum FruitType
{
    Apple,
    Banana,
    Cherry,
    Kiwi,
    Melon,
    Orange,
    Pineapple,
    Strawberry
}

public class Fruit : MonoBehaviour
{
    [SerializeField] private FruitType fruitType;
    [SerializeField] private GameObject pickupVFX;

    private GameManager gameManager;
    protected Animator anim;
    protected SpriteRenderer sr;

    // Awake is called when the script instance is being loaded
    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        sr = GetComponentInChildren<SpriteRenderer>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected virtual void Start()
    {
        gameManager = GameManager.Instance;
        SetRandomFruitIfNeeded();
    }

    // Sets a random fruit index for the animator if the game manager allows it
    private void SetRandomFruitIfNeeded() 
    {
        if (gameManager.FruitRandomize() == false)
        {
            UpdateFruitType();
            return;
        }

        int randomIndex = Random.Range(0, 8); // Max value is exclusive
        fruitType = (FruitType)randomIndex; // Adds
        anim.SetFloat("fruitIndex", randomIndex);
    }

    // Updates the fruit type in the animator based on the current fruitType 
    private void UpdateFruitType() => anim.SetFloat("fruitIndex", (int)fruitType);

    // This method is called when another collider enters the trigger collider attached to the object where this script is attached
    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        Player player = collision.GetComponent<Player>();

        if (player != null)
        {
            gameManager.CollectFruit(fruitType);
            AudioManager.instance.PlaySFX(8);
            Destroy(gameObject);

            GameObject newFx = Instantiate(pickupVFX, transform.position, Quaternion.identity);
            Destroy(newFx, 0.5f);
        }
     }
}
