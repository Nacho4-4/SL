using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundLevelManager : MonoBehaviour
{
    [Header("Sound Level Settings")]
    [SerializeField] private float soundMaxValue = 100f; // Valor m�ximo para el "sonido"
    [SerializeField][Range(0, 2)] private float soundIncreaseRateWalk = 1f; // Tasa de aumento del sonido al caminar
    [SerializeField][Range(0, 4)] private float soundIncreaseRateRun = 2f; // Tasa de aumento del sonido al correr
    [SerializeField][Range(0, 1)] private float soundDecreaseRate = 0.5f; // Tasa de disminuci�n del sonido cuando no te mueves

    private float currentSoundLevel = 0f; // Nivel actual de sonido

    public float CurrentSoundLevel => currentSoundLevel; // Propiedad para acceder al nivel de sonido
    public float GetMaxSoundLevel() => soundMaxValue; // M�todo para obtener el valor m�ximo

    private EnemyAI enemyAI;

    private void Awake()
    {
        enemyAI = FindAnyObjectByType<EnemyAI>();
    }

    // Llamar a este m�todo desde el PlayerMovement para actualizar el nivel de sonido
    public void UpdateSoundLevel(Vector3 move, bool isRunning)
    {
        if (move.sqrMagnitude > 0.01f) // Si el jugador se est� moviendo
        {
            // Si el jugador est� corriendo o caminando, aumentamos el sonido
            if (isRunning)
            {
                currentSoundLevel += soundIncreaseRateRun * Time.deltaTime;
            }
            else
            {
                currentSoundLevel += soundIncreaseRateWalk * Time.deltaTime;
            }
        }
        else // Si el jugador no se mueve, disminuimos el sonido
        {
            currentSoundLevel -= soundDecreaseRate * Time.deltaTime;
        }

        // Limitar el nivel de sonido al m�ximo
        currentSoundLevel = Mathf.Clamp(currentSoundLevel, 0f, soundMaxValue);

        // Comprobar si el nivel de sonido ha alcanzado su m�ximo
        if (currentSoundLevel >= soundMaxValue)
        {
            OnSoundMaxed();
        }
    }

    // Acci�n cuando el sonido llega al m�ximo
    private void OnSoundMaxed()
    {
        // Este m�todo puede desencadenar eventos como hacer al enemigo m�s hostil
        Debug.Log("�Sonido m�ximo alcanzado! El enemigo se vuelve m�s hostil.");
        enemyAI.followRange = 500;
        enemyAI.maxRange = enemyAI.followRange;
    }
}