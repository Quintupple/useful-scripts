using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class AIWithFOV : MonoBehaviour
{
    private FOVDetection fov;

    [Header("Patrol Points")]
    [SerializeField]
    private Transform[] points;
    [SerializeField]
    private float minRemainingDistance = 0.5f;
    private int destinationPoint = 0;

    [Header("NavMeshAgent Stuff")]
    private NavMeshAgent agent;

    [Header("Ranges")]
    public float attackRange = 1.2f;

    [Header("Bools, Floats and Stuff")]
    public bool facePlayer;
	public bool alreadyAttacked;
    public bool isInAttack;
    public float waitTime;
    public float timeBetweenAttacks = 3f;

    [Header("Assign Stuff")]
	public GameObject projectile;
    public LayerMask whatIsPlayer;

    private void Start()
    {
        fov = GetComponent<FOVDetection>();
        agent = GetComponent<NavMeshAgent>();

        attackRange = 0f;
    }

    private void Update()
    {
        isInAttack = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        if (!isInAttack && !agent.pathPending && agent.remainingDistance < 0.5f)
        {
            Patrol();
        }

        if(isInAttack)
        {
            Attack();
        }

        if (fov.isInFov)
        {
            attackRange = 6f;
            ChasePlayer();
            if(isInAttack)
            {
                agent.SetDestination(transform.position);
            }
        }
    }
	
    void Attack()
    {
        agent.SetDestination(transform.position);

        transform.LookAt(fov.player);

        if (!alreadyAttacked)
        {
            Rigidbody rb = Instantiate(projectile, transform.position, Quaternion.identity).GetComponent<Rigidbody>();
            rb.AddForce(transform.forward * 32f, ForceMode.Impulse);
            rb.AddForce(transform.up * 8f, ForceMode.Impulse);

            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

	void ResetAttack()
	{
		alreadyAttacked = false;
	}
	
    void ChasePlayer()
    {
        StopCoroutine(StopForSeconds());
        StartCoroutine(Chase());
        agent.SetDestination(fov.player.position);
        Vector3 currentRotation = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));

        if (currentRotation != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(currentRotation, Vector3.up);

            transform.rotation = Quaternion.Lerp(transform.rotation, lookRotation, 0.5f * Time.deltaTime);
        }
    }

    IEnumerator Chase()
    {
        agent.SetDestination(fov.player.position);
        yield return null;
    }

    void Patrol()
    {
        attackRange = 0f;
        agent.ResetPath();
        destinationPoint = (destinationPoint + 1) % points.Length;
        agent.SetDestination(points[destinationPoint].position);

        agent.velocity = Vector3.zero;
        StartCoroutine(StopForSeconds());
    }

    IEnumerator StopForSeconds()
    {
        if (points.Length == 0)
        {
            Debug.LogError("You must set points");
            enabled = false;
        }

        agent.SetDestination(transform.position);
        yield return new WaitForSeconds(waitTime);
        agent.SetDestination(points[destinationPoint].position);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}