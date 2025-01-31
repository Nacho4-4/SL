using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemySLAI : MonoBehaviour
{
    public Transform target;  // El objetivo que se va a seguir
    private NavMeshAgent agent;  // El componente NavMeshAgent

    public float followRange = 10f;  // Rango dentro del cual el agente seguirá al objetivo
    public float wanderMinRange = 5f;  // Rango mínimo para generar puntos aleatorios
    public float wanderMaxRange = 20f;  // Rango máximo para generar puntos aleatorios
    public float wanderTime = 3f;  // Tiempo que el agente permanece en un punto aleatorio antes de buscar uno nuevo
    public float waitTimeAtPoint = 2f;  // Tiempo que el agente espera en el punto aleatorio antes de moverse a otro

    private float timeToNextWander = 0f;  // Tiempo restante antes de buscar otro punto aleatorio
    private float waitTime = 0f;  // Tiempo restante para esperar en el punto actual
    private bool isWaiting = false;  // Indica si el agente está esperando en un punto

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        WanderToRandomPoint();  // Iniciar moviéndose a un punto aleatorio
    }

    void Update()
    {
        // Verificar la distancia entre el agente y el objetivo
        float distanceToTarget = Vector3.Distance(transform.position, target.position);

        if (distanceToTarget <= followRange)  // Si el objetivo está dentro del rango, sigue al objetivo
        {
            agent.SetDestination(target.position);
            timeToNextWander = 0f;  // Dejar de buscar puntos aleatorios cuando siga al objetivo
            isWaiting = false;  // Si está siguiendo, no espera
        }
        else  // Si el objetivo está fuera del rango
        {
            // Si no estamos esperando, podemos buscar un nuevo punto aleatorio
            if (!isWaiting && timeToNextWander <= 0f)
            {
                WanderToRandomPoint();  // Buscar un punto aleatorio
                timeToNextWander = wanderTime;  // Establecer el tiempo para el próximo "wander"
                isWaiting = true;  // Comenzamos a esperar en el nuevo punto
            }
            else if (isWaiting)
            {
                // Si estamos esperando, esperamos un tiempo antes de elegir otro punto
                if (waitTime <= 0f)
                {
                    WanderToRandomPoint();  // Buscar un nuevo punto
                    waitTime = waitTimeAtPoint;  // Restablecer el tiempo de espera
                    isWaiting = false;  // Dejamos de esperar y volvemos a movernos
                }
                else
                {
                    waitTime -= Time.deltaTime;  // Reducir el tiempo de espera
                }
            }
            else
            {
                timeToNextWander -= Time.deltaTime;  // Reducir el contador de tiempo para la próxima búsqueda de puntos aleatorios
            }
        }
    }

    // Función para hacer que el agente se mueva a un punto aleatorio dentro de un rango del NavMesh
    void WanderToRandomPoint()
    {
        // Generar una distancia aleatoria dentro de los rangos mínimo y máximo
        float wanderRange = Random.Range(wanderMinRange, wanderMaxRange);

        Vector3 randomDirection = Random.insideUnitSphere * wanderRange;  // Generar una dirección aleatoria
        randomDirection += transform.position;  // Asegurarse de que el punto esté en torno a la posición actual

        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, wanderRange, NavMesh.AllAreas))  // Asegurarse de que el punto es válido
        {
            agent.SetDestination(hit.position);  // Mover al agente al punto aleatorio
        }
    }

    // Dibuja el rango de seguimiento en el Editor
    void OnDrawGizmosSelected()
    {
        // Establecer el color del Gizmo
        Gizmos.color = Color.green;

        // Dibujar una esfera (círculo 3D) en la posición del objeto con el radio del rango de seguimiento
        Gizmos.DrawWireSphere(transform.position, followRange);

        // Dibujar el rango mínimo y máximo para los puntos aleatorios
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, wanderMinRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, wanderMaxRange);
    }
}