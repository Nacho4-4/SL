using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float baseSpeed;
    [SerializeField][Range(0, 2)] private float multiplierSpeed;
    [SerializeField] private float jumpForce;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius;
    [SerializeField] private LayerMask groundLayer;

    [Header("Stamina Settings")]
    [SerializeField] private float staminaMaxValue;
    [SerializeField][Range(0, 2)] private float staminaDecreaseRate;
    [SerializeField][Range(0, 2)] private float staminaIncreaseRate;
    [SerializeField] private AudioSource stamineSource;

    [Header("Camera Settings")]
    [SerializeField] private Transform cameraTransform;

    [Header("Audio Settings")]
    [SerializeField] private AudioClip walkSound;
    [SerializeField] private AudioClip runSound;
    [SerializeField] private AudioClip agitationSound;

    public float CurrentSamina => currentStamina;
    public float GetStaminaMaxValue() => staminaMaxValue;
    public bool IsRunning => isRunning;

    private Rigidbody rb;
    private AudioSource audioSource;
    private AudioSource[] audioSources;
    private float moveHorizontal;
    private float moveVertical;
    private bool isRunning;
    private bool isGrounded;
    private float currentStamina;
    private SoundLevelManager soundLevelManager; // Referencia al script de sonido

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
        soundLevelManager = GetComponent<SoundLevelManager>();
        audioSources = GetComponents<AudioSource>();

        currentStamina = staminaMaxValue;
        stamineSource.clip = agitationSound;
    }

    private void Update()
    {
        HandleInputs();
        Jump();
        isGrounded = IsGrounded();
        HandleStaminaSound();

        if (EnemyAI.isGameOver || NotesManager.isWin)
        {
            if (audioSources.Length > 0)
            {
                audioSources[0].Stop();
                audioSources[1].Stop();
            }
        }
    }

    private void FixedUpdate()
    {
        Movement();
    }

    private void HandleInputs()
    {
        moveHorizontal = Input.GetAxis("Horizontal");
        moveVertical = Input.GetAxis("Vertical");

        if (Input.GetKey(KeyCode.LeftShift) && isGrounded)
        {
            if (currentStamina > 0)
            {
                isRunning = true;
                DecreaseStamina();
            }
            else
            {
                isRunning = false;
            }
        }
        else
        {
            isRunning = false;
            IncreaseStamina();
        }
    }

    private void Movement()
    {
        Vector3 move = new(moveHorizontal, 0, moveVertical);

        float increaseSpeed = baseSpeed * multiplierSpeed;

        float currentSpeed = isRunning ? baseSpeed + increaseSpeed : baseSpeed;

        move = move.normalized * currentSpeed;

        // Si la cámara está mirando hacia otro ángulo, el jugador debe moverse en la dirección de la cámara
        if (cameraTransform != null)
        {
            // Obtenemos la rotación de la cámara y solo consideramos el plano horizontal
            Quaternion cameraRotation = Quaternion.Euler(0, cameraTransform.eulerAngles.y, 0);

            // Aplicamos la rotación de la cámara al vector de movimiento
            move = cameraRotation * move;
        }

        // Actualizamos el nivel de sonido en el otro script
        soundLevelManager.UpdateSoundLevel(move, isRunning);

        PlaySounds(move);

        rb.velocity = new Vector3(move.x, rb.velocity.y, move.z);
    }

    private void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    bool IsGrounded()
    {
        return Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer);
    }

    private void OnDrawGizmos()
    {
        if (groundCheck != null)
        {
            Gizmos.color = isGrounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }

    private void PlaySounds(Vector3 move)
    {
        AudioClip currentClip = isRunning ? runSound : walkSound;

        if (EnemyAI.isGameOver == false && NotesManager.isWin == false)
        {
            if (move.sqrMagnitude > 0.01f && isGrounded)
            {
                if (audioSource.clip != currentClip || !audioSource.isPlaying)
                {
                    audioSource.clip = currentClip;
                    audioSource.Play();
                }
            }
            else
            {
                audioSource.Stop();
            }
        }
    }

    private void DecreaseStamina()
    {
        currentStamina -= staminaDecreaseRate * Time.deltaTime;
        currentStamina = Mathf.Clamp(currentStamina, 0, staminaMaxValue);
    }

    private void IncreaseStamina()
    {
        if (currentStamina < staminaMaxValue)
        {
            currentStamina += staminaIncreaseRate * Time.deltaTime;
            currentStamina = Mathf.Clamp(currentStamina, 0, staminaMaxValue);
        }
    }

    private void HandleStaminaSound()
    {
        // Si el jugador está corriendo y la estamina está disminuyendo
        if (isRunning && currentStamina > 0)
        {
            if (!stamineSource.isPlaying) // Reproducir solo si no está sonando
            {
                stamineSource.Play();
            }
        }
        // Si la estamina está siendo recuperada
        else if (currentStamina < staminaMaxValue && !stamineSource.isPlaying)
        {
            stamineSource.Play();
        }
        // Detener el sonido cuando la estamina esté al máximo
        else if (currentStamina >= staminaMaxValue)
        {
            if (stamineSource.isPlaying)
            {
                stamineSource.Stop();
            }
        }
    }
}