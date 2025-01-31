using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.AI;

public class NotesManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI noteMessageText; // Referencia al componente de texto UI
    [SerializeField] private GameObject panelWin;
    [SerializeField][Range(0, 5)] private float messageDuration; // Duración del mensaje en segundos
    [SerializeField][Range(0, 1)] private float followRangeMultiplier = 0.2f; // Factor que aumentará el followRange cada vez que se recoja una nota
    [SerializeField][Range(0, 1)] private float speedMultiplier = 0.2f; // Factor que aumentará la vecloidad cada vez que se recoja una nota
    
    public static NotesManager Instance; // Instancia única del gestor
    private bool isMessage;
    private AudioSource audioSource;
    public static bool isWin;
    private AudioSource[] audioSources;
    public bool iswww;

    private void Awake()
    {
        iswww = isWin;

        // Asegurarnos de que haya solo una instancia de NoteManager
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        // Establecer la cantidad total de notas en la escena
        Note.totalNotes = FindObjectsOfType<Note>().Length;

        // Guardar el número total de notas en PlayerPrefs para el menú principal
        PlayerPrefs.SetInt("TotalNotes", Note.totalNotes);
        PlayerPrefs.Save();

        audioSource = GetComponent<AudioSource>();
        audioSources = FindObjectsOfType<AudioSource>();

        isWin = false;
    }

    private void Start()
    {
        // Verificar datos y establecer el mensaje adecuado
        VerifyData();

        // Cargar el valor de followRange y velocidad desde PlayerPrefs
        LoadFollowRange();
        LoadSpeed();

        foreach (var note in FindObjectsOfType<Note>())
        {
            if (PlayerPrefs.GetInt("Note_" + note.noteIndex) == 1)
            {
                note.gameObject.SetActive(false);  // Desactivar la nota que ya fue recogida
            }
            else
            {
                note.gameObject.SetActive(true);
            }
        }
    }

    // Verifica los datos guardados en PlayerPrefs y actualiza el mensaje en consecuencia
    private void VerifyData()
    {
        // Cargar el número de notas recogidas previamente
        for (int i = 0; i < Note.totalNotes; i++)
        {
            if (PlayerPrefs.GetInt("Note_" + i) == 1)
            {
                Note.collectedNotes++;  // Incrementamos el contador de notas recogidas
            }
        }

        // Si no se han recogido notas, mostramos el mensaje "Busca las X notas"
        if (Note.collectedNotes == 0)
        {
            if (Note.totalNotes == 1)
            {
                ShowMessage($"Busca la {Note.totalNotes} nota.");
            }
            else
            {
                ShowMessage($"Busca las {Note.totalNotes} notas.");
            }
        }
        else if (Note.collectedNotes > 0)
        {
            // Si hay notas recogidas, mostramos cuántas quedan por recoger
            ShowMessage($"Busca las {Note.totalNotes - Note.collectedNotes} notas restantes.");
        }

        if (Note.collectedNotes == Note.totalNotes)
        {
            ShowMessage("Has recogido todas las notas");
            panelWin.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            Time.timeScale = 0;
            isWin = true;
            SetVolume();
        }
    }

    // Actualiza el texto de la UI con la cantidad restante de notas
    public void UpdateNoteMessage()
    {
        int remainingNotes = Note.totalNotes - Note.collectedNotes;

        if (remainingNotes > 0)
        {
            // Mostrar cuántas notas quedan por recoger
            ShowMessage($"Nota recogida, quedan {remainingNotes} restantes.");
            isMessage = true;
        }
        else
        {
            // Si ya se han recogido todas, mostrar el mensaje correspondiente
            ShowMessage("Has recogido todas las notas.");
            panelWin.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            Time.timeScale = 0;
            isWin = true;
            SetVolume();
        }

        audioSource.PlayOneShot(audioSource.clip);

        // Iniciar la corutina para desaparecer el mensaje después de la duración
        StartCoroutine(HideMessageAfterTime());

        // Aumentar el followRange cuando se recoge una nueva nota
        IncreaseFollowRange();

        // Aumentar la velocidad
        IncreaseSpeed();
    }

    // Función para aumentar el followRange con cada nota recogida
    private void IncreaseFollowRange()
    {
        // Si hay un agente de movimiento (suponiendo que está en algún objeto en la escena)
        var agent = FindObjectOfType<EnemyAI>();
        if (agent != null)
        {
            // Incrementar el followRange multiplicando por el multiplicador
            agent.followRange += agent.followRange * followRangeMultiplier;
            agent.maxRange = Mathf.Max(agent.maxRange, agent.followRange);

            // Guardar el nuevo valor de followRange en PlayerPrefs para la próxima sesión
            PlayerPrefs.SetFloat("FollowRange", agent.followRange);
            PlayerPrefs.Save();

            Debug.Log($"Nuevo Follow Range: {agent.followRange}");
        }
    }

    private void IncreaseSpeed()
    {
        var agent = FindObjectOfType<EnemyAI>();
        if (agent != null)
        {
            // Incrementar el followRange multiplicando por el multiplicador
            agent.speed += agent.speed * speedMultiplier;
            agent.speed = Mathf.Clamp(agent.speed, agent.speed, agent.MaxSpeed);

            // Guardar el nuevo valor de followRange en PlayerPrefs para la próxima sesión
            PlayerPrefs.SetFloat("Speed", agent.speed);
            PlayerPrefs.Save();

            Debug.Log($"Nueva velocidad: {agent.speed}");
        }
    }

    // Función para cargar el valor de followRange desde PlayerPrefs al inicio
    private void LoadFollowRange()
    {
        // Verificar si ya existe un valor guardado de FollowRange
        if (PlayerPrefs.HasKey("FollowRange"))
        {
            // Cargar el valor guardado de followRange
            var agent = FindObjectOfType<EnemyAI>();
            if (agent != null)
            {
                agent.followRange = PlayerPrefs.GetFloat("FollowRange");
                Debug.Log($"FollowRange cargado: {agent.followRange}");
            }
        }
        else
        {
            Debug.Log("No se ha encontrado FollowRange guardado. Usando valor predeterminado.");
        }
    }

    private void LoadSpeed()
    {
        // Verificar si ya existe un valor guardado de FollowRange
        if (PlayerPrefs.HasKey("Speed"))
        {
            // Cargar el valor guardado de followRange
            var agent = FindObjectOfType<EnemyAI>();
            if (agent != null)
            {
                agent.speed = PlayerPrefs.GetFloat("Speed");
                Debug.Log($"Velocidad cargada: {agent.speed}");
            }
        }
        else
        {
            Debug.Log("No se ha encontrado Speed guardado. Usando valor predeterminado.");
        }
    }

    // Función para mostrar el mensaje cuando se pasa el mouse por encima de la nota
    public void ShowPickupMessage(string message)
    {
        if (!isMessage)
        {
            noteMessageText.text = message; // Mostrar el mensaje recibido
        }
    }

    // Mostrar un mensaje y ocultarlo después de un tiempo
    private void ShowMessage(string message)
    {
        isMessage = true;
        noteMessageText.text = message;
        StartCoroutine(HideMessageAfterTime());
    }

    // Corutina que oculta el mensaje después de un tiempo
    private IEnumerator HideMessageAfterTime()
    {
        yield return new WaitForSeconds(messageDuration);
        noteMessageText.text = ""; // Borrar el mensaje después de la duración
        isMessage = false;
    }

    private void SetVolume()
    {
        foreach (var audioSource in audioSources)
        {
            audioSource.mute = isWin;
        }
    }
}