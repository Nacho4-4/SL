using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LightNotes : MonoBehaviour
{
    [SerializeField] private Light[] lights;

    private void Awake()
    {
        lights = FindObjectsOfType<Light>().Where(luz => luz.type == LightType.Point).ToArray();
    }

    private void Start()
    {
        // Comienza la corrutina que hace el ciclo de cambio de intensidad
        StartCoroutine(CicloDeIntensidad());
    }

    private IEnumerator CicloDeIntensidad()
    {
        float t = Random.Range(0, 5);
        while (true)
        {
            // Aumenta la intensidad de las luces de 0 a 1
            yield return StartCoroutine(CambiarIntensidad(0f, 5f, 2));
            // Espera un tiempo antes de bajar la intensidad
            yield return new WaitForSeconds(t);

            // Baja la intensidad de las luces de 1 a 0
            yield return StartCoroutine(CambiarIntensidad(5f, 0f, 2));
            // Espera un tiempo antes de aumentar la intensidad nuevamente
            yield return new WaitForSeconds(t);
        }
    }

    private IEnumerator CambiarIntensidad(float valorInicial, float valorFinal, float duracion)
    {
        float tiempoTranscurrido = 0f;

        // Mientras el tiempo no haya alcanzado la duración
        while (tiempoTranscurrido < duracion)
        {
            // Interpolamos suavemente entre el valor inicial y final usando Lerp
            float intensidad = Mathf.Lerp(valorInicial, valorFinal, tiempoTranscurrido / duracion);

            // Aplicamos la intensidad interpolada a todas las luces
            foreach (var luz in lights)
            {
                luz.intensity = intensidad;
            }

            // Aumentamos el tiempo transcurrido
            tiempoTranscurrido += Time.deltaTime;

            yield return null;  // Esperamos el siguiente frame
        }

        // Aseguramos que la intensidad final sea exacta
        foreach (var luz in lights)
        {
            luz.intensity = valorFinal;
        }
    }
}