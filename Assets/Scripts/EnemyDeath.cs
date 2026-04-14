using UnityEngine;

public class EnemyDeath : MonoBehaviour
{
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Weapon"))
        {
            Destroy(gameObject);
        }
    }
}
