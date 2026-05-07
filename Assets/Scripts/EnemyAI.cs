using UnityEngine;
using UnityEngine.AI;

public class NewEnemyAI : MonoBehaviour
{
    private Transform playerTarget;
    private NavMeshAgent agent;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

        // This looks for a GameObject tagged "Player"
        GameObject player = GameObject.FindWithTag("Player");

        if (player != null)
        {
            playerTarget = player.transform;
        }
    }

    void Update()
    {
        if (playerTarget != null)
        {
            agent.SetDestination(playerTarget.position);
        }
    }
}
