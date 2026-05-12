using System.Runtime.InteropServices;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class EnemyAI : MonoBehaviour
{
    [Header("References")]
    public NavMeshAgent agent;
    public Transform player;
    public Rigidbody rb;
    public GameObject sword;
    public GameObject swordHitbox;

    [Header("Behavior Settings")]
    public float chaseRange = 15f;
    public float attackRange = 4f;
    public float dashChance = 0.05f; // Chance per frame to dash if in range
    public float moveSpeed = 0.5f;

    private string lastState = "";
    public float swingSpeed = 0;

    void Start()
    {
        // player = GetComponent<Transform>Player;
        // ss = GetComponent<SwordSwing>();
        swordHitbox.SetActive(false);        

        // Disable agent's auto-movement so Rigidbody handles the physics
        agent.updatePosition = false;
        agent.updateRotation = true;
    }



    void Update()
    {
        // LookAtPlayer();
        SpeedControl();

        float distance = Vector3.Distance(transform.position, player.position);
        float heightDifference = player.position.y - transform.position.y;

        if (distance > attackRange)
        {
            ChasePlayer(distance, heightDifference);
        }
        else if (distance < attackRange)
        {
            StartCoroutine(RDDSlash());
        }
    }

    void FixedUpdate()
    {
        LookAtPlayer();
    }

    [Header("AI Timing")]
    public float dashCooldown = 3f;
    private float dashTimer;

    void ChasePlayer(float dist, float height)
    {
        agent.SetDestination(player.position);

        agent.nextPosition = transform.position;

        Vector3 direction = (agent.destination - transform.position).normalized;

        rb.AddForce(direction * moveSpeed, ForceMode.Force);

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
    private void SpeedControl()
    {

        Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        if (flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.linearVelocity = new Vector3(limitedVel.x, rb.linearVelocity.y, limitedVel.z);
        }
    
        // if (maxYSpeed != 0 && rb.linearVelocity.y > maxYSpeed) // limit y speed
        // {
        //     rb.linearVelocity = new Vector3(rb.linearVelocity.x, maxYSpeed, rb.linearVelocity.z);
        // }
    }
    void AttackPlayer()
    {
        StartCoroutine(RDDSlash());
    }

    private IEnumerator RDDSlash()
    {
        enableHitbox();
        sword.GetComponent<Animator>().Play("RDDSlash");
        yield return new WaitForSeconds(swingSpeed);
        sword.GetComponent<Animator>().Play("SwordIdle");
        disableHitbox();
    }
        public void enableHitbox()
    {
        swordHitbox.SetActive(true);
    }

    public void disableHitbox()
    {
        swordHitbox.SetActive(false);
    }
}