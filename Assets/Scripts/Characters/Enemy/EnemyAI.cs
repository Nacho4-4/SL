using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public GameObject panelGM;
    public Transform target; // El objeto que el agente debe seguir
    public float speed; // velocidad
    public float followRange = 50f;  // Rango en el que el agente sigue al objeto
    public float minDistance = 100f;  // Distancia m�nima entre puntos aleatorios
    public float maxDistance = 200f;  // Distancia m�xima entre puntos aleatorios
    public float waitTime = 2f;  // Tiempo de espera antes de moverse a otro punto
    public float maxRange;
    public float MaxSpeed {  get; private set; }
    public float OrigenRangeFollow {  get; private set; }

    private NavMeshAgent agent;
    private AudioSource audioSource;
    private float timer;
    private Vector3 lastPosition;
    public static bool isGameOver;
    private AudioSource[] audioSources;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        audioSource = GetComponent<AudioSource>();
        audioSources = FindObjectsOfType<AudioSource>();
        timer = waitTime;
        MoveToRandomPoint();  // Comienza movi�ndose a un punto aleatorio
        agent.speed = speed;
        MaxSpeed = 10;
        OrigenRangeFollow = followRange;
        maxRange = followRange;

        audioSource.spatialBlend = 1;
        audioSource.rolloffMode = AudioRolloffMode.Linear;
        isGameOver = false;
    }

    private void Update()
    {
        // Verifica si el objetivo est� dentro del rango de seguimiento
        if (target != null && Vector3.Distance(transform.position, target.position) <= followRange)
        {
            // Si est� dentro del rango de seguimiento, sigue al objetivo
            agent.SetDestination(target.position);
        }
        else
        {
            // Si no est� dentro del rango de seguimiento, sigue con el movimiento aleatorio
            if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
            {
                // Espera un tiempo antes de elegir otro destino
                timer -= Time.deltaTime;
                if (timer <= 0)
                {
                    timer = waitTime;
                    MoveToRandomPoint();
                }
            }
        }

        // Control de la velocidad y reproducci�n del audio
        if (agent.velocity.sqrMagnitude > 0.1f) // Si el agente est� en movimiento (usamos sqrMagnitude para evitar c�lculos costosos de ra�z cuadrada)
        {
            // Reproducir audio si no est� sonando
            if (!audioSource.isPlaying)
            {
                audioSource.Play();  // Reproducir el audio
            }
        }
        else
        {
            // Detener el audio si el agente est� parado
            if (audioSource.isPlaying)
            {
                audioSource.Stop();  // Detener el audio
            }
        }

        // Si el jugador est� asignado, ajusta el volumen en funci�n de la distancia
        if (target != null && audioSource != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, target.position);
            float maxDistance = 50f;  // M�xima distancia en la que el sonido es audible
            float minDistance = 10f;  // Distancia m�nima en la que el sonido es a volumen m�ximo

            // Calcula el volumen en funci�n de la distancia
            float volume = Mathf.Clamp01(1 - (distanceToPlayer - minDistance) / (maxDistance - minDistance));

            // Ajusta el volumen del AudioSource
            audioSource.volume = volume;
        }

        if (agent.speed != speed)
        {
            agent.speed = speed;
        }

        if (isGameOver)
        {
            audioSource.Stop();
        }
    }

    private void MoveToRandomPoint()
    {
        // Encuentra un punto aleatorio dentro de los l�mites del NavMesh
        Vector3 randomDirection = Random.insideUnitSphere * maxDistance;

        // Aseg�rate de que el punto est� dentro del NavMesh
        randomDirection += transform.position;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, maxDistance, NavMesh.AllAreas))
        {
            // Aseg�rate de que el nuevo punto est� a una distancia m�nima del anterior
            if (Vector3.Distance(hit.position, lastPosition) >= minDistance)
            {
                lastPosition = hit.position;
                agent.SetDestination(hit.position);
            }
            else
            {
                // Si el punto es demasiado cercano, elige otro
                MoveToRandomPoint();
            }
        }
    }

    // Dibujar los rangos en el editor con Gizmos
    private void OnDrawGizmos()
    {
        // Si hay un objetivo asignado
        if (target != null)
        {
            // Rango de seguimiento (rojo)
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, followRange);
        }

        // Rango m�ximo de puntos aleatorios (azul)
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, maxDistance);

        // Rango m�nimo de puntos aleatorios (verde)
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, minDistance);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            panelGM.SetActive(true);
            Time.timeScale = 0;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            isGameOver = true;
            SetVolume();
        }
    }

    private void SetVolume()
    {
        foreach (var audioSource in audioSources)
        {
            audioSource.mute = isGameOver;
        }
    }
}