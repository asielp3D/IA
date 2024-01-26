using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class IAEnemy : MonoBehaviour
{
    enum State
    {
        Patrolling,

        Chasing
    }

    State currentState;

    NavMeshAgent enemyAgent;

    Transform playerTransform;

    [SerializeField] Transform patrolArenaCenter;
    [SerializeField] Vector2 patrolAreaSize;

    [SerializeField] float visionRange = 15;

    void Awake()
    {
        enemyAgent = GetComponent<NavMeshAgent>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Start is called before the first frame update
    void Start()
    {
        currentState = State.Patrolling;
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
            SetRabdomPoint();
        }
    }

    void Chase()
    {
        enemyAgent.destination = playerTransform.position;
    }

    void SetRabdomPoint()
    {
        float randomX = Random.Range (-patrolAreaSize.x / 2, patrolAreaSize.x / 2);
        float randomZ = Random.Range (-patrolAreaSize.y / 2, patrolAreaSize.y / 2);
        Vector3 randomPoint = new Vector3(randomX, 0f, randomZ) + patrolArenaCenter.position;

        enemyAgent.destination = randomPoint;
    }

    bool OnRange()
    {
        if(Vector3.Distance(transform.position, playerTransform.position)<= visionRange)
        {
            return true;
        }

        return false;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(patrolArenaCenter.position, new Vector3(patrolAreaSize.x, 0, patrolAreaSize.y));

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, visionRange);

    }
}
