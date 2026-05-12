using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    [SerializeField]
    private float countdown;
    public GameObject spawnPoint;

    public Wave[] waves;

    public int currentWaveIndex = 0;
    private bool readyToCountDown;


    void Start()
    {
        readyToCountDown = true;

        for (int i = 0; i < waves.Length; i++)
        {
            waves[i].enemiesLeft = waves[i].enemies.Length;
        }
    }

    void Update()
    {
        if (currentWaveIndex >= waves.Length)
        {
            return;
        }

        if (readyToCountDown == true)
        {
            countdown -= Time.deltaTime;            
        }

        if (countdown <= 0)
        {
            readyToCountDown = false;
            countdown = waves[currentWaveIndex].timeToNextWave;
            SpawnWave();
        }

        if (waves[currentWaveIndex].enemiesLeft == 0)
        {
            readyToCountDown = true;
            currentWaveIndex++;
        }
    }

    private void SpawnWave()
    {
        if (currentWaveIndex < waves.Length)
        {
            for (int i = 0; i < waves[currentWaveIndex].enemies.Length; i++)
            {
                Instantiate(waves[currentWaveIndex].enemies[i], spawnPoint.transform);
            }            
        }

    }
}

[System.Serializable]
public class Wave
{
    public EnemyAI[] enemies;
    public float timeToNextWave;

    [HideInInspector] public int enemiesLeft;
}