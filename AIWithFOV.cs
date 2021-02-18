using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class AIWithFOV : MonoBehaviour
{
    private FOVDetection fov;
    private Door door;

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
    public float doorRange;
    public float timeRemaining = 3f;

    [Header("Bools, Floats and Stuff")]
    public bool facePlayer;
    public bool isInAttack;
    Collider[] isInDoorRange;
    public float waitTime;
    public bool breathe = false;
    public bool timerIsRunning = false;

    [Header("Assign Stuff")]
    public LayerMask whatIsPlayer;
    public LayerMask whatIsDoor;
    public EnemyPostEffect post;
    public Animator anim;
    public GameObject go;

    private void Start()
    {
        timerIsRunning = true;

        fov = GetComponent<FOVDetection>();
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();

        attackRange = 0f;
    }

    private void Update()
    {
        isInAttack = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);
        isInDoorRange = Physics.OverlapSphere(transform.position, doorRange, whatIsDoor);

        for(int i = 0; i < isInDoorRange.Length; i++)
        {
            GameObject door = isInDoorRange[i].transform.gameObject;
            Door doorScript = door.GetComponent<Door>();

            if (!doorScript.doorOpen && !doorScript.pauseInteraction)
            {
                doorScript.PlayAnim();
                anim.SetBool("isIdle", false);
            }
        }

        if (isInAttack)
        {
            SceneManager.LoadScene("Basement");
        }

        if (!anim.GetBool("isIdle") && !agent.hasPath && agent.pathStatus == NavMeshPathStatus.PathComplete)
        {
            Debug.Log("Character stuck");
            agent.enabled = false;
            agent.enabled = true;
            Debug.Log("navmesh re enabled");
            // navmesh agent will start moving again
        }

        if (!isInAttack && !agent.pathPending && agent.remainingDistance < 0.5f)
        {
            Patrol();
        }

        if (fov.isInFov)
        {
            attackRange = 1.2f;
            post.ChangeEnemyEffect();
            ChasePlayer();
        }
        else
        {
            attackRange = 0f;
            post.ChangePostNormal();
            if (timerIsRunning)
            {
                if (timeRemaining > 0)
                {
                    timeRemaining -= Time.deltaTime;
                }
                else
                {
                    Debug.Log("Time has run out!");
                    facePlayer = true;
                    StartCoroutine(FacePlayer());
                    timeRemaining = 0;
                    timerIsRunning = false;
                }
            }
        }
    }

    void ChasePlayer()
    {
        StopCoroutine(StopForSeconds());
        StartCoroutine(Chase());
        timeRemaining = 3;
        timerIsRunning = true;
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
        anim.SetBool("isIdle", false);
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

    IEnumerator FacePlayer()
    {
        if(facePlayer)
        {
            go.name = "go";
            go.transform.position = fov.player.position;
            go.transform.rotation = fov.player.rotation;

            anim.SetBool("isIdle", false);
            transform.LookAt(go.transform.position);

            yield return new WaitForSeconds(.5f);
            agent.SetDestination(go.transform.position);
            yield return new WaitForSeconds(15f);

            facePlayer = false;
        }
    }

    IEnumerator StopForSeconds()
    {
        if (points.Length == 0)
        {
            Debug.LogError("You must set points");
            enabled = false;
        }

        anim.SetBool("isIdle", true);
        agent.SetDestination(transform.position);
        yield return new WaitForSeconds(waitTime);
        agent.SetDestination(points[destinationPoint].position);
        anim.SetBool("isIdle", false);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, doorRange);
    }
}
