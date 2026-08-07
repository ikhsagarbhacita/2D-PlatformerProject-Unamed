using UnityEngine;

public class LevelCameraTrigger : MonoBehaviour
{
    private LevelCamera levelCamera;
    private int PlayerInTrigger;

    private void Awake()
    {
        levelCamera = GetComponentInParent<LevelCamera>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Player player = collision.gameObject.GetComponent<Player>();

        if (player != null)
        {
            PlayerInTrigger++;

            if (PlayerInTrigger == levelCamera.playerList.Count)
            {
                levelCamera.EnableCamera(true);
                levelCamera.EnableLimits(true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Player player = collision.gameObject.GetComponent<Player>();

        if (player != null)
        {
            PlayerInTrigger--;

            if (PlayerInTrigger == 0)
            {
                levelCamera.EnableCamera(false);
                levelCamera.EnableLimits(false);
            }
        }
    }
}
