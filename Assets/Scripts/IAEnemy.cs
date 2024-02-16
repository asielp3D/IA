using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class IAEnemy : MonoBehaviour
{
    enum State
    {
        Patrolling,

        Chasing,

        Searching,

        Waiting,

        Attacking,
    }

    public Transform[] points;

    private int destPoint = 0;

    State currentState;

    NavMeshAgent enemyAgent;

    Transform playerTransform;

    [SerializeField] Transform patrolArenaCenter;
    [SerializeField] Vector2 patrolAreaSize;

    [SerializeField] float visionRange = 15;
    [SerializeField] float visionAngle = 90;

    Vector3 lastTargetPosition;

    [SerializeField]float searchTimer;
    [SerializeField]float searchWaitTime = 15;
    [SerializeField]float searchRadius = 30;

    

    void Awake()
    {
        enemyAgent = GetComponent<NavMeshAgent>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Start is called before the first frame update
    void Start()
    {
        currentState = State.Patrolling;
        GoToNextPoint();
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentState)
        {
            case State.Patrolling:
                 Patrol();
            break;
            case State.Chasing:
                 Chase();
            break;
            case State.Searching:
                 Search();
            break;
            case State.Waiting:
                 Wait();
            break;
            case State.Attacking:
                 Attack();
            break;
        }

        if (enemyAgent.remainingDistance < 0.5f)
        {
            GoToNextPoint();
        }
    }

    void Patrol()
    {
        
        if(OnRange() == true)
        {
            currentState = State.Chasing;
        }
        
        if(enemyAgent.remainingDistance < 0.5f)
        {
            //SetRandomPoint();
        }
    }

    void Chase()
    {
        enemyAgent.destination = playerTransform.position;

        if(OnRange() == false)
        {
            searchTimer = 0;
            currentState = State.Searching;
        }
    }

    void Search()
    {
        if(OnRange() == true)
        {
            currentState = State.Chasing;
        }
        
        searchTimer += Time.deltaTime;

        if(searchTimer < searchWaitTime)
        {
            if(enemyAgent.remainingDistance < 0.5F)
            {
                Debug.Log("Buscando punto aleatorio");

                Vector3 randomSearchPoint = lastTargetPosition + Random.insideUnitSphere * searchRadius;
                randomSearchPoint.y = lastTargetPosition.y;
                enemyAgent.destination = randomSearchPoint;
            }
        }
        else
        {
            currentState = State.Patrolling;
        }
    }

    void Wait()
    {
        
    }

    void Attack()
    {
        enemyAgent.destination = playerTransform.position;
        Debug.Log("Te pego zorra");
    }

    void GoToNextPoint()
    {
        if (points.Length == 0)
        {
            return;
        }

        enemyAgent.destination = points[destPoint].position;
        destPoint = (destPoint + 1) % points.Length;
    }


    /*void SetRandomPoint()
    {
        float randomX = Random.Range (-patrolAreaSize.x / 2, patrolAreaSize.x / 2);
        float randomZ = Random.Range (-patrolAreaSize.y / 2, patrolAreaSize.y / 2);
        Vector3 randomPoint = new Vector3(randomX, 0f, randomZ) + patrolArenaCenter.position;

        enemyAgent.destination = randomPoint;
    }*/

    bool OnRange()
    {
        /*if(Vector3.Distance(transform.position, playerTransform.position)<= visionRange)
        {
            return true;
        }*/

        return false;

        Vector3 directionToPlayer = playerTransform.position - transform.position;
        float distanceToPlayer = directionToPlayer.magnitude;
        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);
        
        if(distanceToPlayer <= visionRange && angleToPlayer < visionAngle * 0.5f)
        {
            if(playerTransform.position == lastTargetPosition)
            {
                return true;
            }

            RaycastHit hit;
            if(Physics.Raycast(transform.position, directionToPlayer, out hit, distanceToPlayer))
            {
                if(hit.collider.CompareTag("Player"))
                {
                    lastTargetPosition = playerTransform.position;

                    return true;
                }
            }

            return false;
        }

        return false;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(patrolArenaCenter.position, new Vector3(patrolAreaSize.x, 0, patrolAreaSize.y));

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, visionRange);

        Gizmos.color = Color.green;
        Vector3 fovLine1 = Quaternion.AngleAxis(visionAngle *0.5f, transform.up) * transform.forward * visionRange;
        Vector3 fovLine2 = Quaternion.AngleAxis(-visionAngle *0.5f, transform.up) * transform.forward * visionRange;
        Gizmos.DrawLine (transform.position, transform.position + fovLine1);
        Gizmos.DrawLine (transform.position, transform.position + fovLine2);
    }
}
