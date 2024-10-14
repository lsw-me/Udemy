using UnityEngine;

public class ThunderStrike_Controller : MonoBehaviour
{

    protected PlayerStats playerStats;


    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Enemy>() != null)
        {
            PlayerStats  playerStats = PlayerManager.instance.player.GetComponent<PlayerStats>();
            EnemyStats enemtTarget = collision.GetComponent<EnemyStats>();
            playerStats.DoMagicDamage(enemtTarget);
        }
    }
}
