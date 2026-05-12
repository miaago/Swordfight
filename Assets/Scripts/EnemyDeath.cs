// using UnityEngine;

// public class EnemyDeath : MonoBehaviour
// {
//     void OnCollisionEnter(Collision collision)
//     {
//         if (collision.gameObject.CompareTag("Weapon"))
//         {
//             Destroy(gameObject);
//         }
//     }
// }


using UnityEngine;


public class EnemyDeath : MonoBehaviour
{
    public WaveSpawner waveSpawner;
    // Activates as a trigger instead
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Weapon"))
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Enemy has been defeated!");

        Destroy(gameObject);
        waveSpawner.waves[waveSpawner.currentWaveIndex].enemiesLeft--;
    }
}
