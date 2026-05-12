// using UnityEngine;
// using UnityEngine.AI;

// public class NewEnemyAI : MonoBehaviour
// {
//     private Transform playerTarget;
//     private NavMeshAgent agent;

//     void Awake()
//     {
//         agent = GetComponent<NavMeshAgent>();

//         // This looks for a GameObject tagged "Player"
//         GameObject player = GameObject.FindWithTag("Player");

//         if (player != null)
//         {
//             playerTarget = player.transform;
//         }
//     }

//     void Update()
//     {
//         if (playerTarget != null)
//         {
//             agent.SetDestination(playerTarget.position);
//         }
//     }
// }

using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [Header("References")]
    public NavMeshAgent agent;
    public Transform player;
    private PlayerMovement pm;
    private PlayerDash pd;
    private SwordSwing ss;

    [Header("Behavior Settings")]
    public float chaseRange = 15f;
    public float attackRange = 2f;
    public float dashChance = 0.05f; // Chance per frame to dash if in range

    private string lastState = "";

    void Start()
    {
        pm = GetComponent<PlayerMovement>();
        pd = GetComponent<PlayerDash>();
        ss = GetComponent<SwordSwing>();

        // Disable agent's auto-movement so Rigidbody handles the physics
        agent.updatePosition = false;
        agent.updateRotation = true;
    }



    void Update()
    {
        LookAtPlayer();

        float distance = Vector3.Distance(transform.position, player.position);
        float heightDifference = player.position.y - transform.position.y;

        string currentState = "";

        if (distance <= chaseRange)
        {
            if (distance <= attackRange)
            {
                currentState = "ATTACKING";
            }
            else
            {
                currentState = "CHASING";
            }
        }
        else
        {
            currentState = "IDLE";
        }

        if (currentState != lastState)
        {
            Debug.Log($"AI State Changed to: {currentState}. Distance: {distance}");
            lastState = currentState;
        }

        if (currentState == "ATTACKING")
        {
            AttackState();
        }
        else if (currentState == "CHASING")
        {
            ChasePlayer(distance, heightDifference);
        }
        else
        {
            IdleState();
        }
    }

    [Header("AI Timing")]
    public float dashCooldown = 3f;
    private float dashTimer;

    void ChasePlayer(float dist, float height)
    {
        agent.SetDestination(player.position);

        agent.nextPosition = transform.position;

        Vector3 direction = (agent.nextPosition - transform.position).normalized;
        pm.externalV = Vector3.Dot(pm.orientation.forward, direction);
        pm.externalH = Vector3.Dot(pm.orientation.right, direction);

        //Debug.Log($"Inputs - V: {pm.externalV} | H: {pm.externalH}");

        pm.isSprinting = true;

        // If player is on seats or high ground, tell pm to jump
        if (height > 1.5f && pm.isGrounded)
        {
            pm.externalJump = true;
        }
        else
        {
            pm.externalJump = false;
        }

        // Update the timer
        if (dashTimer > 0) dashTimer -= Time.deltaTime;

        // Only dash if far away AND the timer is ready
        if (dist > 7f && dashTimer <= 0 && !pm.dashing)
        {
            if (Random.value < dashChance)
            {
                pd.ExternalDashTrigger();
                dashTimer = dashCooldown; // Reset the timer
                Debug.Log("AI Dashing!");
            }
        }
    }

    void LookAtPlayer()
    {
        // Calculate the direction to the player
        Vector3 lookPos = player.position - transform.position;
        lookPos.y = 0; // Keep the enemy upright so it doesn't tilt up/down

        // Rotate the enemy object
        if (lookPos != Vector3.zero)
        {
            Quaternion rotation = Quaternion.LookRotation(lookPos);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 5f);
        }
    }

    void AttackState()
    {
        StopMovement();

        // Trigger the same SwordSwing logic the player uses
        // Since RDDSlash is an IEnumerator, we check if it's already swinging
        StartCoroutine(ss.RDDSlash());
    }

    void IdleState()
    {
        StopMovement();
    }

    void StopMovement()
    {
        pm.externalV = 0;
        pm.externalH = 0;
        pm.isSprinting = false;
    }
}
