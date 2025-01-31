using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class GrainEffectControl : MonoBehaviour
{
    // Referencia al PostProcessVolume
    public PostProcessVolume postProcessVolume;
    private Grain grainEffect;

    // Valores de oscilaci�n
    public float lerpSpeed = 1f;  // Velocidad del lerp
    public float maxLuminance = 1f; // Valor m�ximo de luminance contribution
    private float targetLuminance;

    private float time;

    void Start()
    {
        // Obtener el efecto Grain desde el PostProcessVolume
        if (postProcessVolume.profile.TryGetSettings(out grainEffect))
        {
            // Inicializar el valor de luminance (puedes configurar un valor inicial si lo prefieres)
            grainEffect.lumContrib.value = 0f;
            targetLuminance = maxLuminance;
        }
        else
        {
            Debug.LogError("No se ha encontrado el efecto Grain en el perfil de PostProcess.");
        }
    }

    void Update()
    {
        if (grainEffect != null)
        {
            // Incrementar o disminuir el valor de luminance dependiendo del objetivo (ascendente o descendente)
            grainEffect.lumContrib.value = Mathf.Lerp(grainEffect.lumContrib.value, targetLuminance, lerpSpeed * Time.deltaTime);

            // Detectar si alcanzamos el valor objetivo y cambiar la direcci�n
            if (Mathf.Approximately(grainEffect.lumContrib.value, targetLuminance))
            {
                // Invertir el objetivo (subir o bajar)
                targetLuminance = (targetLuminance == maxLuminance) ? 0f : maxLuminance;
            }
        }
    }
}