using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SoundBarUI : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Image soundBarImage;  // La imagen que representa la barra de sonido
    [SerializeField] private TextMeshProUGUI alertText;  // El texto que muestra el mensaje de alerta

    [Header("Pulse Effect Settings")]
    [SerializeField][Range(0, 5)] private float pulseSpeed = 2f;  // Velocidad de oscilación (pulso)

    [Header("Enemy Settings")]
    [SerializeField][Range(0, 1)] private float Increasemultipler = .02f;
    [SerializeField][Range(0, 1)] private float Decreasemultipler = .01f;

    private SoundLevelManager soundLevelManager;
    private Vector3 enlargedScale = new(1.5f, 1.5f, 1); // Escala cuando el sonido es mayor a la mitad
    private Vector3 normalScale = new(1, 1, 1);  // Escala original de la imagen
    private bool isSoundHigh = false;  // Variable para comprobar si el sonido está por encima de la mitad

    private EnemyAI enemyAI;

    private void Awake()
    {
        // Obtén la referencia al SoundLevelManager (asegúrate de que está en el mismo objeto o en otro)
        soundLevelManager = FindAnyObjectByType<SoundLevelManager>();

        enemyAI = FindAnyObjectByType<EnemyAI>();

        if (soundBarImage == null)
        {
            Debug.LogError("SoundBarUI: No se ha asignado la imagen de la barra de sonido.");
        }

        if (alertText == null)
        {
            Debug.LogError("SoundBarUI: No se ha asignado el Text para mostrar el mensaje.");
        }
    }

    private void Update()
    {
        // Llamamos al método para actualizar la UI
        UpdateSoundUI();
    }

    // Método separado que actualiza el fillAmount y el texto
    private void UpdateSoundUI()
    {
        if (soundLevelManager != null && soundBarImage != null && alertText != null)
        {
            // Normalizamos el valor de currentSoundLevel a un valor entre 0 y 1
            float normalizedSoundLevel = Mathf.InverseLerp(0f, soundLevelManager.GetMaxSoundLevel(), soundLevelManager.CurrentSoundLevel);
            soundBarImage.fillAmount = normalizedSoundLevel;

            // Si el nivel de sonido es mayor a la mitad, hacemos que la barra pulse
            if (soundLevelManager.CurrentSoundLevel > soundLevelManager.GetMaxSoundLevel() / 2)
            {
                if (!isSoundHigh)  // Si el sonido acaba de superar la mitad, mostramos el mensaje y activamos el pulso
                {
                    alertText.text = "¡Cuidado! El sonido está muy alto.";
                    isSoundHigh = true;
                    
                }

                // Pulsar la barra de sonido entre normal y aumentada usando una función seno para hacer el pulso continuo
                float pulse = Mathf.Sin(Time.time * pulseSpeed); // Esto crea un valor que oscila entre -1 y 1
                float scaleFactor = Mathf.Lerp(1f, 1.5f, (pulse + 1f) / 2f);  // Convertir de [-1, 1] a [0, 1] y hacer el Lerp
                soundBarImage.transform.localScale = normalScale * scaleFactor;

                enemyAI.followRange += enemyAI.followRange * Increasemultipler * Time.deltaTime;
                enemyAI.followRange = Mathf.Clamp(enemyAI.followRange, enemyAI.OrigenRangeFollow, 500);
            }
            else
            {
                if (isSoundHigh)  // Si el sonido baja de la mitad, paramos el pulso y reseteamos el mensaje
                {
                    alertText.text = "";
                    isSoundHigh = false;
                }

                enemyAI.followRange -= enemyAI.followRange * Decreasemultipler * Time.deltaTime;
                enemyAI.followRange = Mathf.Max(enemyAI.followRange, enemyAI.maxRange);

                // Mantener la barra de sonido con su escala normal
                soundBarImage.transform.localScale = normalScale;
            }
        }
    }
}