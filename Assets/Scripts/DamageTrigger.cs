using UnityEngine;

public class DamageTrigger : MonoBehaviour
{
    public void OnTriggerEnter2D(Collider2D collision)
    {
        Player player = collision.gameObject.GetComponent<Player>();

        if (player != null)
        {
            player.Damage();
            player.KnockBack(transform.position.x);
        }
    }
}
