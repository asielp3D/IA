using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

//IMPORTANTE PONER EL UnityEngine.AI;!!

//Hay que asignar cosas en el inspector

//En el examen la IA tendra que patrullar y podremos elegir de que forma (aleatoria o por puntos como el ejercicio)

public class Script_Examen : MonoBehaviour
{
    enum State
    {
        Patrolling,

        Chasing,

        Attacking
    }

    private State currentState; //Almacena estado actual de la IA.

    private NavMeshAgent agent; //El NavMeshAgent es el componente que le tendremos que poner a la capsula de la IA

    private Transform player; //Posicion del jugador

    [SerializeField] private Transform[] patrolPoints;//Array para los puntos (Los puntos son emptys que hay que crear y ponerlos donde quiera)

    [SerializeField] private float detectionRange = 15; //Rango de deteccion de la IA

    [SerializeField] private float attackRange = 5; //Rango de ataque de la IA

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindWithTag("Player").transform; //Poner el Tag de "Player" al player
    }
    
    // Start is called before the first frame update
    void Start()
    {
        SetRandomPoint(); 
        currentState = State.Patrolling;
    }

    // Update is called once per frame
    void Update()
    {
        //Controla el codigo segun el estado
        switch(currentState) 
        {
            case State.Patrolling:
                Patrol();
            break;

            case State.Chasing:
                Chase();
            break;

            case State.Attacking:
                Attack();
            break;
        }
    }

    void Patrol()
    {
        if(IsInRange(detectionRange) == true) //Este if nos detecta si el player esta dentro del rango de la IA y si entra cambiara al estado de Chasing
        {
            currentState = State.Chasing;
        }

        if(agent.remainingDistance < 0.5f)
        {
            SetRandomPoint();
        }
    }

    void Chase()
    {
        if(IsInRange(detectionRange) == true) //Con este if la IA detectara que el player esta fuera del rango y buscara otro punto aleatorio para ponerse de nuevo a patrullar
        {
            SetRandomPoint();
            currentState = State.Patrolling;
        }

        if(IsInRange(attackRange) == true) //Con este if detectara si el personaje esta dentro del rango de ataque y si esta dentro pasara a estado de ataque
        {
            currentState = State.Attacking;
        }

        agent.destination = player.position; //Con esto la IA persigue al personaje
    }

    void Attack()
    {
        Debug.Log("Atacando");

        currentState = State.Chasing;
    }

    void SetRandomPoint()
    {
        agent.destination = patrolPoints[Random.Range(0, patrolPoints.Length - 1)].position;//Eleccion de un punto de destino aleatorio
    }

    bool IsInRange(float range)
    {
        if(Vector3.Distance(transform.position, player.position) < range) //Esto nos sirve para comprobar si el player esta en los diferentes rangos
        {
            return true;
        }

        else
        {
            return false;
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;

        foreach(Transform point in patrolPoints)
        {
            Gizmos.DrawWireSphere(point.position, 0.5f); //Con esto dibujaremos una esfera en los puntos de patrulla
        }

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectionRange); //Con esto dibujaremos el area de deteccion de la IA

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange); //Con esto dibujaremos el area de ataque de la IA

    }
}
